using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;

namespace LanguageGuesser
{
    class webScraper
    {
        public string url;
        public List<string> textContent = new List<string>();
        public List<string> textContentSorted = new List<string>();
        public enum TextType { Article, Book};
        public TextType thisTextType;

        public enum Language { English, French, Spanish};

        public static Dictionary<TextType, int> typeToMinLengthToConsider = new Dictionary<TextType, int>();
        private int minLengthToConsider = 100;

        private string[] titles = { "Mr", "Miss", "Mrs", "Dr", "Jr"};

        public webScraper(string url, TextType type)
        {
            typeToMinLengthToConsider.Add(TextType.Article, 1);
            typeToMinLengthToConsider.Add(TextType.Book, 1);

            thisTextType = type;
            minLengthToConsider = typeToMinLengthToConsider[thisTextType];

            this.url = url;
        }

        public void scrapeURL()
        {
            Console.WriteLine("Starting scrape");

            List<string> scrapedText = new List<string>();

            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc = web.Load(url);

            Console.WriteLine(doc.GetType().ToString());
            Console.WriteLine("Page Loaded");

            var nodes = doc.DocumentNode.SelectSingleNode("//body");

            int n = 0;  //Counter so doesnt go through whole book so I can see top in console

            foreach (var nNode in nodes.DescendantsAndSelf())   //Goes through all nodes
            {
                //Console.WriteLine("Node " + n);
                n++;

                if (nNode.NodeType != HtmlNodeType.Element)
                    continue;

                if (nNode.Name.Equals("p"))  //Parse the text
                {
                    string text = nNode.InnerText;
                    if (text.Length > minLengthToConsider)
                    {
                        //Console.WriteLine(nNode.InnerText);
                        scrapedText.Add(text);
                    }
                }
            }

            textContent = scrapedText;
        }


        public void convertForMLLanguage(Language language)
        {
            Console.WriteLine("Sorting to sentences");

            List<string> allSentences = new List<string>();

            foreach (string paragraph in textContent)
            {
                List<string> sentencesInParagraph = getSentences(paragraph);
                foreach (string sentence in sentencesInParagraph)
                    allSentences.Add(sentence); 
            }

            textContentSorted = allSentences;
        }
        public List<string> getSentences(string paragraph)
        {            
            List<string> sentences = new List<string>();

            int index;

            bool isForTitle;
            bool nextCharIsPeriod;
            bool previousCharIsUpper;

            int startIndex = 0;

            while (true)
            {
                index = paragraph.IndexOf(".", startIndex);

                if (index == -1)
                    break;

                //isForTitle = true;
                isForTitle = periodIsForTitle(paragraph, index);
                nextCharIsPeriod = (paragraph.Substring(index + 1).Length > 1 && paragraph[index + 1] == '.');
                previousCharIsUpper = index > 0 && Char.IsUpper(paragraph[index - 1]);

                if (!previousCharIsUpper && !nextCharIsPeriod && !isForTitle)
                {
                    //Cut sentence out
                    string sentence = paragraph.Substring(0, index + 1);
                    sentences.Add(sentence);
                    paragraph = paragraph.Substring(index + 1);

                    startIndex = 0;
                }
                else {
                    //Move on to next period
                    
                }


            }

            return sentences;

            /*

            //Finds next period (end of sentence)
            int index = paragraph.IndexOf(".");
            bool isForTitle = periodIsForTitle(paragraph, index);
            bool nextCharIsPeriod = (paragraph.Substring(index + 1).Length > 1 && paragraph[index + 1] == '.');
            bool previousCharIsUpper = index > 0 && Char.IsUpper(paragraph[index - 1]);
            
            while ((previousCharIsUpper || nextCharIsPeriod || isForTitle) && paragraph.Length > index + 1)
            {
                index = paragraph.IndexOf(".", index+1);
                nextCharIsPeriod = (paragraph.Substring(index + 1).Length > 1 && paragraph[index + 1] == '.');
                previousCharIsUpper = index > 0 && Char.IsUpper(paragraph[index - 1]);


                isForTitle = periodIsForTitle(paragraph, index);
                if (isForTitle) Console.WriteLine("Period is for title");
            }

            while (index != -1)
            {
                //If there is quotation mark after . increase index to include quotation mark
                //Note: UTF-8 has several characters that look like quotation marks
                if (paragraph.Length > index + 1 && paragraph[index + 1] == '”')
                    index++;

                //Get the sentence and remove it from paragraph
                string sentence = paragraph.Substring(0, index+1);
                paragraph = paragraph.Substring(index+1);

                sentence = sentence.Replace(Environment.NewLine, " ");

                //Eliminate space at begining of sentence
                if (sentence[0] == ' ') 
                    sentence = sentence.Substring(1);

                sentence = removeCurlyBraces(sentence);
                sentence = changeApostrophes(sentence);
                sentence = changeQuotes(sentence);
                sentences.Add(sentence);
                
                //Finds next period
                index = paragraph.IndexOf(".");

                //Skips to next period if this period doesnt mark end of sentence
                isForTitle = periodIsForTitle(sentence, index);
                if (isForTitle) Console.WriteLine("Period is for title");
                nextCharIsPeriod = (paragraph.Substring(index + 1).Length > 1 && paragraph[index + 1] == '.');
                previousCharIsUpper = index > 0 && Char.IsUpper(paragraph[index - 1]);
                while ((previousCharIsUpper || nextCharIsPeriod || isForTitle) && paragraph.Length > index + 1)
                {
                    index = paragraph.IndexOf(".", index + 2);
                    nextCharIsPeriod = (paragraph.Substring(index + 1).Length > 1 && paragraph[index + 1] == '.');
                    previousCharIsUpper = index > 0 && Char.IsUpper(paragraph[index - 1]);
                    isForTitle = periodIsForTitle(sentence, index);
                    if (isForTitle) Console.WriteLine("Period is for title");
                }
            }

            */
        }

        public string removeCurlyBraces(string sentence)
        {
            int indexToRemove = sentence.IndexOf("{");
            if (indexToRemove == -1)
                indexToRemove = sentence.IndexOf("}");

            while (indexToRemove != -1)
            {
                sentence = sentence.Remove(indexToRemove, 1);

                indexToRemove = sentence.IndexOf("{");
                if (indexToRemove == -1)
                    indexToRemove = sentence.IndexOf("}");
            }

            return sentence;
        }
        public string changeApostrophes(string sentence)
        {
            int indexToRemove = sentence.IndexOf("&#x27;");

            while (indexToRemove != -1)
            {
                string before = sentence.Substring(0, indexToRemove);
                string after = sentence.Substring(indexToRemove + "&#x27;".Length);
                sentence = before + "'" + after;

                indexToRemove = sentence.IndexOf("&#x27;");
            }

            return sentence;
        }
        public string changeQuotes(string sentence)
        {
            int indexToRemove = sentence.IndexOf("&quot;");

            while (indexToRemove != -1)
            {
                string before = sentence.Substring(0, indexToRemove);
                string after = sentence.Substring(indexToRemove + "&quot;".Length);
                sentence = before + "\"" + after;

                indexToRemove = sentence.IndexOf("&quot;");
            }

            return sentence;
            
        }

        public bool periodIsForTitle(string sentence, int index)    //Mr Miss Jr
        {
            if (index >= sentence.Length || index < 0)
            {
                //Console.WriteLine("Return false : index out of bounds");
                return false;
            }

            for (int i = 0; i < titles.Length; i++)
            {
                string s = sentence.Substring(0, index);
                if (s.Contains(titles[i]))
                {
                    Console.WriteLine("True");
                    return true;
                }
            }
            //Console.WriteLine("Return false : isn't for title");
            return false;
        }

        //Returns path of generated file
        public string saveIntoTextFile(string path = @"H:\Documents\VisualStudioProjects\LanguageTextFiles\myTextFile")
        {
            TextFile textFile = new TextFile(path);

            foreach (string str in textContentSorted)
            {
                textFile.write(str);
            }

            return textFile.path;
        }
    }
}
