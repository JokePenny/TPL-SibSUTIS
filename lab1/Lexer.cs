using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lab1
{
    sealed class Lexer : Dictionary
    {
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
            SEMILICON,
            STRING,
            CHAR,
            NULL,
            FAILED
        }

        public static void StartLexer(string stroke)
        {
            ReadSource.FillDictionary();

            string[] strokeWithoutComm = StringTreatment.DeleteCommentsAndTab(stroke);

            bool isFindString = false;
            int countQuotes = 0;

            List<char> buildToken = new List<char>();
            for (int i = 0, j = 0; i < strokeWithoutComm.Length; i++)
            {
                string line = StringTreatment.FormatStroke(strokeWithoutComm[i]);
                Console.WriteLine(line);

                Token tokenType = Token.NULL;
                string findToken = "";
                countQuotes = 0;
                isFindString = false;

                for (j = 0; j < line.Length; j++)
                {
                    if (line[j] == '"' || isFindString)// обработка строки в кавычках
                    {
                        isFindString = true;
                        buildToken.Add(line[j]);
                        if(line[j] == '"') countQuotes++;
                        if (countQuotes == 2)
                        {
                            countQuotes = 0;
                            isFindString = false;
                            findToken = new string(buildToken.ToArray());
                            CreateNodeToken(Token.STRING, findToken, i, j);
                            buildToken.Clear();
                        }
                    }
                    else if (line[j] != ' ')//обработка токена
                    {
                        buildToken.Add(line[j]);
                        findToken = new string(buildToken.ToArray());
                        tokenType = FindToken(findToken);
                    }
                    else if(buildToken.Count != 0)
                    {
                        CreateNodeToken(!isFindString ? tokenType : Token.FAILED, findToken, i, j);
                        buildToken.Clear();
                    }
                }
                if (isFindString)// проверка на закрытие найденной строки
                {
                    findToken = new string(buildToken.ToArray());
                    CreateNodeToken(Token.FAILED, findToken, i, i);
                    buildToken.Clear();
                }
                if (buildToken.Count != 0)
                {
                    CreateNodeToken(!isFindString ? tokenType : Token.FAILED, findToken, i, j);
                    buildToken.Clear();
                }
            }
        }

        private static void CreateNodeToken(Token token, string subStroke, int y, int x)
        {
            TokenNode tokenNode = new TokenNode(token, subStroke, y, x);
            listTokens.Add(tokenNode);
        }

        private static Token FindToken(string buildToken)
        {
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

                Console.WriteLine(lineInfoToken + tabulation + "'" + tokenNode.subStroke + "'");
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
