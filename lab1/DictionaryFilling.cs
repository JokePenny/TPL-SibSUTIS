using System;
using System.IO;
using System.Text;

namespace lab1
{
    class DictionaryFilling
    {
        private const string pathDictTokenKEYWORD = @"\dictionary\keyword.txt";
        private const string pathDictTokenTYPE = @"\dictionary\type.txt";
        private const string pathDictTokenOP = @"\dictionary\op.txt";
        private const string pathDictTokenTWINS = @"\dictionary\twins.txt";
        private const string pathDictForbiddenSymbolsTokenID = @"\dictionary\forbiddensymbols.txt";

        public static void FillDictionary()
        {
            Lexer.dictTokenKEYWORD = ReadFileString(pathDictTokenKEYWORD);
            Lexer.dictTokenTYPE = ReadFileString(pathDictTokenTYPE);
            Lexer.dictTokenOP = ReadFileString(pathDictTokenOP);
            Lexer.dictTokenTWINS = ReadFileChar(pathDictTokenTWINS);
            Lexer.dictForbiddenSymbolsTokenID = ReadFileChar(pathDictForbiddenSymbolsTokenID);
        }

        private static string[] ReadFileString(string path)
        {
            FileStream fstream = File.OpenRead(Environment.CurrentDirectory + path);
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            string dictionary = Encoding.Default.GetString(array);
            return dictionary.Split("\r\n");
        }

        private static char[] ReadFileChar(string path)
        {
            
            FileStream fstream = File.OpenRead(Environment.CurrentDirectory + path);
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            string dictionary = Encoding.Default.GetString(array);
            string deletenewLine = dictionary.Replace("\r\n", null);
            return deletenewLine.ToCharArray();
        }
    }
}
