using System;
using System.Collections.Generic;

namespace LanguageGuesser
{
    //Ideas:
    //Guess language
    //Guess how old
    //Guess text type (articles, novel (what genre))
    //See if sentence is grammatically correct?
    //Complete sentence?

    class Program
    {   
        public static webScraper webText;
        static void Main(string[] args)
        {

            Console.WriteLine("Starting program");

            //Go on gutenberg book page. select "Read this book online: HTML"

            string book1 = "https://www.gutenberg.org/files/64317/64317-h/64317-h.htm"; //Great Gatsby            
            string book2 = "https://www.gutenberg.org/files/64002/64002-h/64002-h.htm"; //The Great Accident     
            string article1 = "https://www.cbc.ca/news/canada/edmonton/lumber-mills-wood-covid-19-pandemic-supply-demand-fence-posts-decks-edmonton-1.6024190";


            string basePath = @"H:\Documents\VisualStudioProjects\LanguageTextFiles\";

            webText = new webScraper(book1, "English");

            
            webText.scrapeURL();
            webText.convertForMLLanguage();
            
            string path = "";
            path = webText.saveIntoTextFile(basePath + "book1");

            if (path != "")
                Console.WriteLine("Finished writing text document");
            
        }
    }
}
