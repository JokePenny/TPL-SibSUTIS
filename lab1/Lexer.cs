using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace lab1
{
    class Lexer
    {
        public static string dictTokenKEYWORD;
        public static string dictTokenTYPE;
        public static string dictTokenNUM_REAL = @"[0-9]+\.[0-9]*";
        public static string dictTokenNUM = @"[0-9]+";
        public static string dictTokenSTRING = @"'[^']*'";
        public static string dictTokenOP = @"[+\-\*/.=<>]";
        public static string dictTokenTWINS = @"[\[\](\)\{\}]";
        public static string dictForbiddenSymbolsTokenID = @"T:\\\\";
        public static string dictTokenID = @"[A-Za-z][A-Za-z0-9]*";

        public static char semicolon = ';';
        private static List<TokenNode> listTokens = new List<TokenNode>();

        public struct TokenNode
        {
            public TokenNode(Token token, string subStroke, int y, int x)
            {
                this.token = token;
                this.subStroke = subStroke;
                this.y = y + 1;
                this.x = x + 1;
            }

            public Token token;
            public string subStroke;
            public int y;
            public int x;
        }

        // виды токенов
        public enum Token : int
        {
            KEYWORD = 0,
            TYPE,
            OP,
            NUM,
            NUM_REAL,
            ID,
            TWINS,
            STROKE,
            SEMILICON,
            STRING,
            FAILED
        }

        public static void StartLexer(string stroke)
        {
            DictionaryFilling.FillDictionary();

            string[] strokeWithoutComm = StringTreatment.DeleteCommentsAndTab(stroke);

            for (int i = 0, j = 0; i < strokeWithoutComm.Length; i++)
            {
                string line = StringTreatment.FormatStroke(strokeWithoutComm[i]);
                Console.WriteLine(line);
                List<char> buildToken = new List<char>();

                Token tokenType = Token.FAILED;
                string findToken = "";
                for (; j < line.Length; j++)
                {
                    if(line[j] != ' ')
                    {
                        buildToken.Add(line[j]);
                        findToken = new string(buildToken.ToArray());
                        tokenType = FindToken(findToken);
                    }
                    else if(buildToken.Count != 0)
                    {
                        TokenNode tokenNode = new TokenNode(tokenType, findToken, i, j);
                        listTokens.Add(tokenNode);
                        buildToken.Clear();
                    }
                }
                j = 0;
            }
        }

        private static Token FindToken(string buildToken)
        {
            if (Regex.IsMatch(buildToken, dictTokenTYPE)) return Token.TYPE;
            if (Regex.IsMatch(buildToken, dictTokenKEYWORD)) return Token.KEYWORD;
            if (Regex.IsMatch(buildToken, dictTokenOP)) return Token.OP;
            if (Regex.IsMatch(buildToken, dictTokenTWINS)) return Token.TWINS;
            if (Regex.IsMatch(buildToken, dictTokenNUM)) return Token.NUM;
            if (Regex.IsMatch(buildToken, dictTokenNUM_REAL)) return Token.NUM_REAL;
            if (Regex.IsMatch(buildToken, dictTokenSTRING)) return Token.STRING;
            if (Regex.IsMatch(buildToken, dictTokenID)) return Token.ID;
            if (Regex.IsMatch(buildToken, dictForbiddenSymbolsTokenID)) return Token.FAILED;
            if (buildToken[0] == semicolon) return Token.SEMILICON;
            return Token.FAILED;
        }

        public static void ViewTokens()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("|Tokens|\t\t|Lexema|");
            Console.WriteLine("-----------------------------------------");
            Console.ResetColor();
            for (int i = 0; i < listTokens.Count; i++)
            {
                TokenNode tokenNode = listTokens[i];
                bool isFail = tokenNode.token == Token.FAILED;
                if (isFail) Console.ForegroundColor = ConsoleColor.Red;

                string lineInfoToken = tokenNode.token + " <" + tokenNode.y + ":" + tokenNode.x + ">";
                string tabulation = lineInfoToken.ToString().Length > 15 ? "\t" : "\t\t";
                string end = tokenNode.subStroke.ToString().Length > 5 ? "\t|" : "\t\t|";

                Console.WriteLine(lineInfoToken + tabulation + "'" + tokenNode.subStroke + "'" + end);
                if (isFail) Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----------------------------------------");
            Console.ResetColor();
        }

        public static void FreeTokens()
        {
            listTokens.Clear();
        }
    }
}
