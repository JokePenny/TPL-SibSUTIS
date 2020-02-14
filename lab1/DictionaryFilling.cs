namespace lab1
{
    class DictionaryFilling : ReadSource
    {
        private const string pathDictTokenKEYWORD = @"\dictionary\keyword.txt";
        private const string pathDictTokenTYPE = @"\dictionary\type.txt";
        private const string pathDictTokenOP = @"\dictionary\op.txt";
        private const string pathDictTokenTWINS = @"\dictionary\twins.txt";
        private const string pathDictForbiddenSymbolsTokenID = @"\dictionary\forbiddensymbols.txt";
        private const string pathDictSpacebetween = @"\dictionary\spacebetween.txt";

        public static void FillDictionary()
        {
            Lexer.dictTokenKEYWORD = GetString(pathDictTokenKEYWORD);
            Lexer.dictTokenTYPE = GetString(pathDictTokenTYPE);
            Lexer.dictTokenOP = GetString(pathDictTokenOP);
            Lexer.dictTokenTWINS = GetChar(pathDictTokenTWINS);
            Lexer.dictForbiddenSymbolsTokenID = GetChar(pathDictForbiddenSymbolsTokenID);
            StringTreatment.dictSpaceBetween = GetChar(pathDictSpacebetween);
        }

        private static char[] GetChar(string path)
        {
            string deletenewLine = ReadFile(path).Replace("\r\n", null);
            return deletenewLine.ToCharArray();
        }

        private static string[] GetString(string path)
        {
            return ReadFile(path).Split("\r\n");
        }
    }
}
