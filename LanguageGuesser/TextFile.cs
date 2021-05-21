using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LanguageGuesser
{
    class TextFile
    {
        public static int versionDigits = 3;   //MyTextFile001 -> 001
        public static string fileFormat = ".tsv";
        public string path;
        public string defaultTryPath = @"H:\Documents\VisualStudioProjects\LanguageTextFiles\myTextFile";
        
        public TextFile()
        {
            path = generateFile(defaultTryPath);
        }
        public TextFile(string path)
        {
            this.path = generateFile(path + "_001" + fileFormat);
        }

        //Returns path of generated file
        public string generateFile(string tryPath)
        {
            if (!File.Exists(tryPath))
            {
                // Create a file to write to.
                Console.WriteLine("Text file generated. Path: " + tryPath);
                return tryPath;
            }
            else
            {
                //Note: always starts from version 001 and loops up to smallest empty number

                //Gets number version
                string mainFileName = tryPath.Substring(0, tryPath.Length - (versionDigits+fileFormat.Length));
                string s_fileNumber = tryPath.Substring(tryPath.Length- (versionDigits+fileFormat.Length), versionDigits);    //Gets last 3 characters
                
                int i_fileNum = Int32.Parse(s_fileNumber);
                int i_newFileNum = i_fileNum + 1;

                string s_newFileNum = "";

                if (i_newFileNum < 100)
                {
                    s_newFileNum += "0";

                    if (i_newFileNum < 10)
                    {
                        s_newFileNum += "0";
                    }
                }

                s_newFileNum += i_newFileNum;

                return generateFile(mainFileName + s_newFileNum + fileFormat);
            }
        }

        public void write(string toWrite)
        {
            //Note: CreateText overwrites everything previously on file. AppendText writes to already existing
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(toWrite);
            }
        }
    }
}