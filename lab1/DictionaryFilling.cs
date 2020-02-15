namespace lab1
{
    class DictionaryFilling : ReadSource
    {
        private const string pathDictTokenKEYWORD = @"\dictionary\keyword.txt";
        private const string pathDictTokenTYPE = @"\dictionary\type.txt";
        private const string pathDictSpacebetween = @"\dictionary\spacebetween.txt";

        public static void FillDictionary()
        {
            Lexer.dictTokenKEYWORD = GetString(pathDictTokenKEYWORD);
            Lexer.dictTokenTYPE = GetString(pathDictTokenTYPE);
            StringTreatment.dictSpaceBetween = GetStringWitchSpace(pathDictSpacebetween);
        }

        private static string GetString(string path)
        {
            return ReadFile(path).Replace("\r\n", "");
        }

        private static string GetStringWitchSpace(string path)
        {
            return ReadFile(path).Replace("\r\n", " ");
        }
    }
}
