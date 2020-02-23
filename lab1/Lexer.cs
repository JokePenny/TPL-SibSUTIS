using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace lab1
{
    public class TokenNode : Lexer
    {
        public TokenNode(Token token, string subString, int y, int x)
        {
            this.token = token;
            this.subString = subString;
            this.y = y + 1;
            this.x = x + 1;
        }

        public Token token;
        public string subString;
        public int y;
        public int x;
    }

    public class Lexer : Dictionary
    {
        private static List<TokenNode> listTokens = new List<TokenNode>();

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

        static int lineMemmory = 0;
        static int indexMemmory = 0;
        static string[] line = { };
        static string[] stringWithoutComm;

        public static void StartLexer(string str)
        {
            // заполнение словарей
            ReadSource.FillDictionary();
            // удаление комментариев и табуляции
            stringWithoutComm = StringTreatment.DeleteCommentsAndTab(str);
            line = StringTreatment.FormatStroke(stringWithoutComm[lineMemmory]);
            Console.WriteLine(stringWithoutComm[lineMemmory]);
            AbstractSyntaxTree.CreateAST();
        }

        public static TokenNode GetToken()
        {
            for (; lineMemmory < stringWithoutComm.Length;)
            {
                // обработка слов
                for (; indexMemmory < line.Length;)
                {
                    // определения вида токена
                    Token tokenType = FindToken(line[indexMemmory]);
                    // создание токена
                    return CreateNodeToken(tokenType, line[indexMemmory]);
                }
            }
            return null;
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

        private static TokenNode CreateNodeToken(Token token, string subStr)
        {
            TokenNode tokenNode = new TokenNode(token, subStr, lineMemmory, indexMemmory);
            indexMemmory++;
            if (indexMemmory == line.Length)
            {
                indexMemmory = 0;
                lineMemmory++;
                if (lineMemmory != stringWithoutComm.Length)
                {
                    Console.WriteLine(stringWithoutComm[lineMemmory]);
                    line = StringTreatment.FormatStroke(stringWithoutComm[lineMemmory]);
                }
            }
            listTokens.Add(tokenNode);
            return tokenNode;
        }
        
        public static void ViewTokens()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("|Tokens|\t\t|Lexema|");
            Console.ResetColor();
            Console.WriteLine("-----------------------------------------");
            for (int i = 0; i < listTokens.Count; i++)
            {
                TokenNode tokenNode = listTokens[i];
                bool isFail = tokenNode.token == Token.FAILED;
                if (isFail) Console.ForegroundColor = ConsoleColor.Red;
                else Console.ForegroundColor = ConsoleColor.Green;

                string lineInfoToken = tokenNode.token + " <" + tokenNode.y + ":" + tokenNode.x + ">";
                string tabulation = lineInfoToken.ToString().Length > 15 ? "\t" : "\t\t";

                Console.WriteLine(lineInfoToken + tabulation + "'" + tokenNode.subString + "'");
                Console.ResetColor();
            }
            Console.WriteLine("-----------------------------------------");
        }
    }
}
