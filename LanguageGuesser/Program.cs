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

        public static string[,] resources = new string[,]
        {
            { "Romanian", "https://www.gutenberg.org/files/64597/64597-h/64597-h.htm"},

            { "Italian", "https://www.lanotiziagiornale.it/biden-intelligence-covid19/"},
            { "Italian", "https://www.lanotiziagiornale.it/contro-codice-degli-appalti/"},
            { "Italian", "https://www.gutenberg.org/files/23297/23297-h/23297-h.htm"},

            { "German", "https://www.sueddeutsche.de/politik/schwexit-schweiz-laesst-vertrag-mit-eu-platzen-1.5305067"},
            { "German", "https://www.gutenberg.org/files/24571/24571-h/24571-h.htm"},
            { "German", "https://www.gutenberg.org/files/7205/7205-h/7205-h.htm"},

            { "English", "https://www.gutenberg.org/files/64317/64317-h/64317-h.htm" },
            { "English", "https://www.gutenberg.org/files/64002/64002-h/64002-h.htm"},
            { "English", "https://www.cbc.ca/news/canada/edmonton/lumber-mills-wood-covid-19-pandemic-supply-demand-fence-posts-decks-edmonton-1.6024190" },
            { "English", "https://www.gutenberg.org/files/65446/65446-h/65446-h.htm"},

            { "French", "https://www.gutenberg.org/files/54873/54873-h/54873-h.htm"},
            { "French", "https://ici.radio-canada.ca/nouvelle/1796167/jason-kenney-assouplissement-covid-19-alberta-reouverture"},
            { "French", "https://ici.radio-canada.ca/nouvelle/1796177/fusillade-san-jose-gare-triage-californie"},

            { "Spanish", "https://www.gutenberg.org/files/2000/2000-h/2000-h.htm"},
            { "Spanish", "https://es.wikipedia.org/wiki/Protestas_en_Colombia_de_2021"}
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Starting program");
            
            string basePath = @"H:\Documents\VisualStudioProjects\LanguageTextFiles\";
            string path = basePath + "Final Document1";

            TextFile textFile = new TextFile(path, true);

            //for (int i = 0; i < resources.Length; i++)
            //{

            int i = 0;
                webText = new webScraper(resources[i, 1], resources[i, 0]);

                webText.scrapeURL();
                webText.convertForMLLanguage();

                bool complete = webText.saveIntoTextFile(textFile);

                if (complete)
                    Console.WriteLine("Finished writing resource " + i + " to " + path);
            //}

            Console.WriteLine("Program complete");

        }

    }
}
