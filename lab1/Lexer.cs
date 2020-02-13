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

        static string[] dictTokenKEYWORD = { "break", "while", "else", "if", "for", "new",
                                             "as", "base", "case", ""};

        static string[] dictTokenTYPE = { "string", "var", "char", "byte", "Sbyte",
                                                "int", "uint", "double", "float",
                                                "long", "ulong", "decimal"};

        static string[] dictTokenOP = { "<", "<=", "=", "=>", "==", "!=", "+",
                                              "-", "/", "%", "*", "^"};

        static char[] dictTokenNUM = { '0', '1', '2', '3', '4', '5', '6',
                                              '7', '8', '9'};

        static char dotInNUM = '.';

        static char[] dictTokenTWINS = { '[', ']', '(', ')', '{', '}', '\'', '\"'};

        static char[] dictForbiddenSymbolsTokenID = { '[', '@', ',', '.', ']', '+', '-', '/', '}',
                                                        '|', '\\', '*', '^', '$', '#', '!', '~', '&',
                                                        '\'', '\"'};

        static char semicolon = ';';

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

        public static void ViewTokens(List<TokenNode> listTokens)
        {
            for (int i = 0; i < listTokens.Count; i++)
            {
                Console.WriteLine(listTokens[i].token + " '" + listTokens[i].subStroke + "'\n");
            }
        }

        private static string[] DeleteEmpty(string stroke)
        {
            string deleteTabulation = stroke.Replace("\t", " ");
            string deleteNewline = deleteTabulation.Replace("\n", " ");
            string[] deleteSpace = deleteNewline.Split(" ");
            return deleteSpace;
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
            if (FindFractionTokenID(dictTokenNUM, subStroke, ref tokenId)) return Token.ID;
            return Token.FAILED;
        }

        private static bool FindFractionTokenID(char[] dictTokenNUM, string subStroke, ref int tokenId)
        {
            
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
    }
}
