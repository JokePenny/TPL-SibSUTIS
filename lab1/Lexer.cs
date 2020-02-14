using System;
using System.Collections.Generic;
using System.Text;

namespace lab1
{
    class Lexer
    {
        public static string[] dictTokenKEYWORD;
        public static string[] dictTokenTYPE;
        public static string[] dictTokenOP;
        public static char[] dictTokenTWINS;
        public static char[] dictForbiddenSymbolsTokenID;
        public static char[] dictTokenNUM = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static char dotInNUM = '.';
        public static char semicolon = ';';
        private static List<TokenNode> listTokens = new List<TokenNode>();

        public struct TokenNode
        {
            public TokenNode(Token token, string subStroke, int tokenID, int y, int x)
            {
                this.token = token;
                this.subStroke = subStroke;
                this.tokenID = tokenID;
                this.y = y + 1;
                this.x = x + 1;
            }

            public Token token;
            public int tokenID;
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
            ID,
            TWINS,
            STROKE,
            SEMILICON,
            FORBIDEN,
            FAILED
        }

        public static void StartLexer(string stroke)
        {
            DictionaryFilling.FillDictionary();

            StringBuilder stringFinder = new StringBuilder();
            bool isFindStroke = false;

            string[] strokeWithoutComm = StringTreatment.DeleteCommentsAndTab(stroke);

            for (int i = 0; i < strokeWithoutComm.Length; i++)
            {
                string strokeFormat = StringTreatment.FormatStroke(strokeWithoutComm[i]);
                string[] strokeWithoutSpace = StringTreatment.HeavyDeleteSpace(strokeFormat, ref stringFinder, ref isFindStroke);

                for (int j = 0; j < strokeWithoutSpace.Length; j++)
                {
                    int tokenId = 0;
                    Token token = TokenDetermine(strokeWithoutSpace[j], ref tokenId);
                    TokenNode tokenNode = new TokenNode(token, strokeWithoutSpace[j], tokenId, i, j);
                    listTokens.Add(tokenNode);
                }
            }
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

        private static Token TokenDetermine(string subStroke, ref int tokenId)
        {
            // поиск ключевых слов
            if (FindTokenInDictionary(dictTokenKEYWORD, subStroke, ref tokenId)) return Token.KEYWORD;
            // поиск типов
            if (FindTokenInDictionary(dictTokenTYPE, subStroke, ref tokenId)) return Token.TYPE;
            // поиск операндов
            if (FindTokenInDictionary(dictTokenOP, subStroke, ref tokenId)) return Token.OP;
            // поиск цифр
            if (FindFractionTokenInDictionary(dictTokenNUM, subStroke, ref tokenId)) return Token.NUM;
            if(subStroke.Length == 1)
            {
                // поиск блочных символов
                if (FindTokenInDictionary(dictTokenTWINS, subStroke[0], ref tokenId)) return Token.TWINS;
                // поиск точки с запятой
                if (semicolon == subStroke[0]) return Token.SEMILICON;
            }
            // поиск запрещенных символов в переменных
            if (FindForbiddenTokenID(dictForbiddenSymbolsTokenID, subStroke)) return Token.ID;
            return Token.FAILED;
        }

        private static bool FindForbiddenTokenID(char[] dictionary, string subStroke)
        {
            for (int i = 0; i < subStroke.Length; i++)
            {
                for (int j = 0; j < dictionary.Length; j++)
                {
                    if (subStroke[i] == dictionary[j]) return false;
                }
            }
            return true;
        }

        private static bool FindTokenInDictionary(string[] dictionary, string subStroke, ref int tokenId)
        {
            for (int i = 0; i < dictionary.Length; i++)
            {
                if (subStroke == dictionary[i])
                {
                    tokenId = i;
                    return true;
                }
            }
            return false;
        }

        private static bool FindTokenInDictionary(char[] dictionary, char tween, ref int tokenId)
        {
            for (int i = 0; i < dictionary.Length; i++)
            {
                if (tween == dictionary[i])
                {
                    tokenId = i;
                    return true;
                }
            }
            return false;
        }

        private static bool FindFractionTokenInDictionary(char[] dictionary, string subStroke, ref int tokenId)
        {
            bool isNumFind = false;
            bool isDotFirst = true;
            bool isStartToken = false;
            for (int i = 0; i < subStroke.Length; i++)
            {
                for (int j = 0; j < dictionary.Length; j++)
                {
                    if (subStroke[i] == dictionary[j])
                    {
                        isNumFind = true;
                        if (!isStartToken)
                        {
                            isStartToken = true;
                            tokenId = j;
                        }
                        break;
                    }
                    if (subStroke[i] == dotInNUM)
                    {
                        tokenId = j;
                        if (!isDotFirst) return false;
                        else isDotFirst = false;
                    }
                }
                if (!isNumFind) return false;
            }
            return true;
        }
    }
}
