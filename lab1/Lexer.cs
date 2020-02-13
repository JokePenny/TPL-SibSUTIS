using System;
using System.Collections.Generic;
using System.Text;

namespace lab1
{
    class Lexer
    {
        public struct TokenNode
        {
            public TokenNode(Token token, string subStroke, int tokenID, int idPosition)
            {
                this.token = token;
                this.subStroke = subStroke;
                this.tokenID = tokenID;
                this.idPosition = idPosition;
            }

            public Token token;
            public int tokenID;
            public int idPosition;
            public string subStroke;
        }

        public enum Token : int
        {
            KEYWORD = 0,
            TYPE,
            OP,
            NUM,
            ID,
            TWINS,
            SEMILICON,
            FAILED
        }

        public static string[] dictTokenKEYWORD;
        public static string[] dictTokenTYPE;
        public static string[] dictTokenOP;
        public static char[] dictTokenTWINS;
        public static char[] dictForbiddenSymbolsTokenID;

        public static char[] dictTokenNUM = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        private static char dotInNUM = '.';
        private static char semicolon = ';';

        public static void StartLexer(string stroke)
        {
            string[] strokeWithoutEmpty = DeleteEmpty(stroke);

            List<TokenNode> listTokens = new List<TokenNode>();

            for (int i = 0; i < strokeWithoutEmpty.Length; i++)
            {
                int tokenId = 0;
                Token token = TokenDetermine(strokeWithoutEmpty[i], ref tokenId);
                TokenNode tokenNode = new TokenNode(token, strokeWithoutEmpty[i], tokenId, i);
                listTokens.Add(tokenNode);
            }
            ViewTokens(listTokens);
        }

        private static string[] DeleteEmpty(string stroke)
        {
            string deleteComments = FindComments(stroke);
            string deleteTabulation = deleteComments.Replace("\t", " ");
            string deleteNewline = deleteTabulation.Replace("\n", " ");
            string insertSpaceBetweenKey = InsertSpaceBetween(deleteNewline);
            string[] deleteSpace = insertSpaceBetweenKey.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            return deleteSpace;
        }

        private static string FindComments(string stroke)
        {
            StringBuilder insertingSpace = new StringBuilder();
            StringBuilder multyLineComment = new StringBuilder();
            bool isFindNewLine = false;
            bool isFindMultyLine = false;
            for (int i = 0; i < stroke.Length - 1; i++)
            {
                if (stroke[i] == '/' && stroke[i + 1] == '*')
                {
                    isFindMultyLine = true;
                }
                if (stroke[i] == '/' && stroke[i + 1] == '/')
                {
                    isFindNewLine = true;
                }
                if (stroke[i] == '\n')
                {
                    isFindNewLine = false;
                }

                if (!isFindNewLine && !isFindMultyLine) insertingSpace.Append(stroke[i]);
                else if (isFindMultyLine) multyLineComment.Append(stroke[i]);

                if (stroke[i] == '*' && stroke[i + 1] == '\\')
                {
                    isFindMultyLine = false;
                }
            }
            if (isFindMultyLine) insertingSpace.Append(multyLineComment);
            return insertingSpace.ToString();
        }

        private static string InsertSpaceBetween(string deleteNewline)
        {
            StringBuilder insertingSpace = new StringBuilder(deleteNewline);
            char[] strokeWithSpace = deleteNewline.ToCharArray();
            for (int i = 0, indexInsert = 0; i < strokeWithSpace.Length; i++, indexInsert++)
            {
                for (int j = 0; j < dictTokenTWINS.Length; j++)
                {
                    if (strokeWithSpace[i] == dictTokenTWINS[j])
                    {
                        insertingSpace.Insert(indexInsert, " ");
                        indexInsert++;
                        insertingSpace.Insert(indexInsert + 1, " ");
                        indexInsert++;
                        break;
                    }
                    else if(strokeWithSpace[i] == semicolon)
                    {
                        insertingSpace.Insert(indexInsert, " ");
                        indexInsert++;
                        insertingSpace.Insert(indexInsert + 1, " ");
                        indexInsert++;
                        break;
                    }
                    else if (strokeWithSpace[i] == dotInNUM)
                    {
                        insertingSpace.Insert(indexInsert, " ");
                        indexInsert++;
                        insertingSpace.Insert(indexInsert + 1, " ");
                        indexInsert++;
                        break;
                    }
                }
            }
            return insertingSpace.ToString();
        }

        private static Token TokenDetermine(string subStroke, ref int tokenId)
        {
            if (FindTokenInDictionary(dictTokenKEYWORD, subStroke, ref tokenId)) return Token.KEYWORD;
            if (FindTokenInDictionary(dictTokenTYPE, subStroke, ref tokenId)) return Token.TYPE;
            if (FindTokenInDictionary(dictTokenOP, subStroke, ref tokenId)) return Token.OP;
            if (FindFractionTokenInDictionary(dictTokenNUM, subStroke, ref tokenId)) return Token.NUM;
            if(subStroke.Length == 1)
            {
                if (FindTokenInDictionary(dictTokenTWINS, subStroke[0], ref tokenId)) return Token.TWINS;
                if (semicolon == subStroke[0]) return Token.SEMILICON;
            }
            if (FindForbiddenSymbolsTokenID(dictForbiddenSymbolsTokenID, subStroke)) return Token.ID;
            return Token.FAILED;
        }

        private static bool FindForbiddenSymbolsTokenID(char[] dictionary, string subStroke)
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

        private static bool FindTokenInDictionary(char[] dictionary, char subStroke, ref int tokenId)
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

        private static void ViewTokens(List<TokenNode> listTokens)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("|Tokens\t\tLexema|");
            Console.WriteLine("---------------------------------");
            Console.ResetColor();
            for (int i = 0; i < listTokens.Count; i++)
            {
                bool isFail = listTokens[i].token == Token.FAILED;
                if (isFail) Console.ForegroundColor = ConsoleColor.Red;

                string tabulation = listTokens[i].token.ToString().Length > 6 ? "\t" : "\t\t";
                string end = listTokens[i].subStroke.ToString().Length > 5 ? "\t|" : "\t\t|";


                Console.WriteLine("|" + listTokens[i].token + tabulation + "'" + listTokens[i].subStroke + "'" + end);
                if (isFail) Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("---------------------------------");
            Console.ResetColor();
        }
    }
}
