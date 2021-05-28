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
using System.Web;

namespace LanguageGuesser
{
    class webScraper
    {
        public string url;
        public List<string> textContent = new List<string>();
        public List<string> textContentSorted = new List<string>();

        public string language;

        private string[] titles = { "Mr", "Miss", "Mrs", "Dr", "Jr", "Fr", "Sr", "Mlle", "Mme"};

        public webScraper(string url, string language)
        {
            this.language = language;
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
                    scrapedText.Add(text);
                }
            }

            textContent = scrapedText;
        }

        public void convertForMLLanguage()
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

        public bool periodIsForTitle(string sentence, int startIndex, int endIndex)    //Mr Miss Jr
        {
            if ((endIndex >= sentence.Length || endIndex < 0) && ((startIndex >= sentence.Length || startIndex < 0)))
            {
                return false;
            }

            for (int i = 0; i < titles.Length; i++)
            {
                string s = sentence.Substring(startIndex, endIndex-startIndex);
                if (s.Contains(titles[i]))
                    return true;
            }
            return false;
        }

        public List<string> getSentences(string paragraph)
        {            
            List<string> sentences = new List<string>();

            int index;
            int startIndex = 0;

            bool isForTitle;
            bool nextCharIsPeriod;
            bool previousCharIsUpper;

            while (true)
            {
                index = paragraph.IndexOf(".", startIndex);

                if (index == -1)
                    break;

                //isForTitle = true;
                isForTitle = periodIsForTitle(paragraph, startIndex, index);
                nextCharIsPeriod = (paragraph.Substring(index + 1).Length > 1 && paragraph[index + 1] == '.');
                previousCharIsUpper = index > 0 && Char.IsUpper(paragraph[index - 1]);

                if (!previousCharIsUpper && !nextCharIsPeriod && !isForTitle)
                {
                    //If there is quotation mark after . increase index to include quotation mark
                    //Note: UTF-8 has several characters that look like quotation marks
                    if (paragraph.Length > index + 1 && paragraph[index + 1] == '”')
                        index++;

                    //Cut sentence out
                    string sentence = paragraph.Substring(0, index + 1);
                    sentence = fixSentence(sentence);

                    if (sentence.Length > 3 + language.Length)
                        sentences.Add(sentence);

                    paragraph = paragraph.Substring(index + 1);

                    startIndex = 0;
                }
                else {
                    //Move on to next period
                    startIndex = index + 1;
                }
            }
            return sentences;
        }

        public string fixSentence(string sentence)
        {

            sentence = sentence.Replace(Environment.NewLine, " ");
            sentence = sentence.Replace("\t", "");

            sentence = removeCurlyBraces(sentence);
            sentence = removeSquareBraces(sentence);
            sentence = HttpUtility.HtmlDecode(sentence);

            //Eliminate space at begining of sentence
            while (sentence[0] == ' ')
                sentence = sentence.Substring(1);

            sentence += "\t" + language;

            return sentence;
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

        public string removeSquareBraces(string sentence)
        {
            int first = sentence.IndexOf("[");
            int second = sentence.IndexOf("]");


            while (first != -1 && second != -1)
            {
                string before = sentence.Substring(0, first);
                string after = sentence.Substring(second+1);
                sentence = before + after;

                first = sentence.IndexOf("[");
                second = sentence.IndexOf("]");
            }

            return sentence;
        }

        public string replacePartOfSentence(string sentence, string unwanted, string newPiece)
        {
            int indexToRemove = sentence.IndexOf(unwanted);

            while (indexToRemove != -1)
            {
                string before = sentence.Substring(0, indexToRemove);
                string after = sentence.Substring(indexToRemove + unwanted.Length);
                sentence = before + newPiece + after;

                indexToRemove = sentence.IndexOf(unwanted);
            }

            return sentence;
        }

        //Returns when complete
        public bool saveIntoTextFile( TextFile file)
        {
            foreach (string str in textContentSorted)
            {
                file.write(str);
            }

            return true;
        }
    }
}
