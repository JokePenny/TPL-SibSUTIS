using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lab1
{
    public class Lexer : Dictionary
    {
        private static List<TokenNode> listTokens = new List<TokenNode>();

        public struct TokenNode
        {
            public TokenNode(Token token, string subStr, int y, int x)
            {
                this.token = token;
                this.subStr = subStr;
                this.y = y + 1;
                this.x = x + 1;
            }

            public Token token;
            public string subStr;
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
            SEMILICON,
            STRING,
            CHAR,
            NULL,
            FAILED
        }

        public static void StartLexer(string str)
        {
            // заполнение словарей
            ReadSource.FillDictionary();

            // удаление комментариев и табуляции
            string[] strokeWithoutComm = StringTreatment.DeleteCommentsAndTab(str);

            // обработка строк
            for (int i = 0; i < strokeWithoutComm.Length; i++)
            {
                Console.WriteLine(strokeWithoutComm[i]);

                /* 
                форматирование строки, например:
                вход:  int[]=new int[8];
                --------
                выход: int [ ] = new int [ 8 ] ;
                */
                string[] line = StringTreatment.FormatStroke(strokeWithoutComm[i]);

                // обработка слов
                for (int j = 0; j < line.Length; j++)
                {
                    // определения вида токена
                    Token tokenType = FindToken(line[j]);
                    // создание токена
                    CreateNodeToken(tokenType, line[j], i, j);
                }
            }
        }

        public static Token FindToken(string buildToken)
        {
            if (Regex.IsMatch(buildToken, dictTokenSTRING)) return Token.STRING;
            if (Regex.IsMatch(buildToken, dictTokenTYPE)) return Token.TYPE;
            if (Regex.IsMatch(buildToken, dictTokenKEYWORD)) return Token.KEYWORD;
            if (Regex.IsMatch(buildToken, dictTokenOP)) return Token.OP;
            if (Regex.IsMatch(buildToken, dictTokenTWINS)) return Token.TWINS;
            if (Regex.IsMatch(buildToken, dictTokenNUM)) return Token.NUM;
            if (Regex.IsMatch(buildToken, dictTokenNUM_REAL)) return Token.NUM_REAL;
            if (Regex.IsMatch(buildToken, dictTokenCHAR)) return Token.CHAR;
            if (Regex.IsMatch(buildToken, dictTokenID)) return Token.ID;
            if (Regex.IsMatch(buildToken, dictForbiddenSymbolsTokenID)) return Token.FAILED;
            if (buildToken[0] == semicolon) return Token.SEMILICON;
            return Token.FAILED;
        }

        private static void CreateNodeToken(Token token, string subStr, int y, int x)
        {
            TokenNode tokenNode = new TokenNode(token, subStr, y, x);
            listTokens.Add(tokenNode);
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
                else Console.ForegroundColor = ConsoleColor.Green;

                string lineInfoToken = tokenNode.token + " <" + tokenNode.y + ":" + tokenNode.x + ">";
                string tabulation = lineInfoToken.ToString().Length > 15 ? "\t" : "\t\t";

                Console.WriteLine(lineInfoToken + tabulation + "'" + tokenNode.subStr + "'");
                Console.ResetColor();
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
