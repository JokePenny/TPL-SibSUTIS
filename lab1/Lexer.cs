﻿using lab1.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace lab1
{
    public class TokenNode : Lexer
    {
        public TokenNode(Tokens.Token token, string subString, int y, int x)
        {
            this.token = token;
            this.subString = subString;
            this.y = y + 1;
            this.x = x + 1;
        }

        public Tokens.Token token;
        public string subString;
        public int y;
        public int x;
    }

    public class Lexer
    {
        private static List<TokenNode> listTokens = new List<TokenNode>();

        private static int lineMemmory = 0;
        private static int indexMemmory = 0;
        private static int startTokenIndex = 0;
        private static string line;
        private static string[] stringWithoutComm;
        private static Tokens.Token tokenTypeMemory = Tokens.Token.EMPTY;
        private static string tokenBuilderMemory = "";

        public static void StartLexer(string str)
        {
            // удаление комментариев и табуляции
            stringWithoutComm = StringTreatment.DeleteComments(str);
            ConsoleHelper.WriteDefault(stringWithoutComm[lineMemmory]);
            line = stringWithoutComm[lineMemmory];
            AbstractSyntaxTree.CreateAST();
        }

        public static TokenNode GetToken()
        {
            StringBuilder tokenBuilder = new StringBuilder();
            bool isFindString = false;
            bool isFindChar = false;
            startTokenIndex = indexMemmory;
            // обработка слов
            for (; lineMemmory < stringWithoutComm.Length; lineMemmory++, line = stringWithoutComm[lineMemmory])
            {
                for (; indexMemmory < line.Length; indexMemmory++)
                {
                    if (line[indexMemmory] == ' ' && !isFindString)
                    {
                        if (tokenBuilderMemory != null)
                        {
                            indexMemmory++;
                            return CreateNodeToken(tokenTypeMemory, tokenBuilderMemory);
                        }
                        else continue;
                    }
                    tokenBuilder.Append(line[indexMemmory]);
                    if(isFindString && line[indexMemmory] == '"')
                    {
                        isFindString = !isFindString;
                    }
                    else if (isFindString || line[indexMemmory] == '"')
                    {
                        isFindString = true;
                        continue;
                    }

                    if (isFindChar && line[indexMemmory] == '\'')
                    {
                        isFindChar = !isFindChar;
                    }
                    else if (isFindChar || line[indexMemmory] == '\'')
                    {
                        isFindChar = true;
                        continue;
                    }

                    // определения вида токена
                    Tokens.Token tokenType = FindToken(tokenBuilder.ToString());
                    if (Tokens.IsErrorToken(ref tokenType, tokenTypeMemory, tokenBuilder.Length) && indexMemmory != line.Length - 1)
                    {
                        tokenTypeMemory = tokenType;
                        tokenBuilderMemory = tokenBuilder.ToString();
                        continue;
                    }

                    if (indexMemmory == line.Length - 1 && tokenType != Tokens.Token.FAILED)
                    {
                        tokenBuilderMemory = tokenBuilder.ToString();
                        tokenTypeMemory = tokenType;
                        indexMemmory++;
                    }
                    return CreateNodeToken(tokenTypeMemory, tokenBuilderMemory);
                }
                ConsoleHelper.WriteDefault(stringWithoutComm[lineMemmory]);
            }
            return null;
        }

        public static Tokens.Token FindToken(string buildToken)
        {
            if (Regex.IsMatch(buildToken, Tokens.String)) return Tokens.Token.STRING;
            if (Regex.IsMatch(buildToken, Tokens.Char)) return Tokens.Token.CHAR;
            if (Regex.IsMatch(buildToken, Tokens.DoubleVal)) return Tokens.Token.DOUBLE_VALUE;
            if (Regex.IsMatch(buildToken, Tokens.IntVal)) return Tokens.Token.INT_VALUE;
            if (Regex.IsMatch(buildToken, Tokens.X16)) return Tokens.Token.X16_VALUE;
            if (Regex.IsMatch(buildToken, Tokens.X8)) return Tokens.Token.X8_VALUE;
            if (Regex.IsMatch(buildToken, Tokens.X2)) return Tokens.Token.X2_VALUE;
            Tokens.Token token = Tokens.Token.EMPTY;
            if (Tokens.DictionaryWord.TryGetValue(buildToken, out token)) return token;
            if (Regex.IsMatch(buildToken, Tokens.Id)) return Tokens.Token.ID;
            return Tokens.Token.FAILED;
        }

        private static TokenNode CreateNodeToken(Tokens.Token token, string subStr)
        {
            TokenNode tokenNode = new TokenNode(token, subStr, lineMemmory, startTokenIndex);
            if (indexMemmory == line.Length)
            {
                indexMemmory = 0;
                lineMemmory++;
                if (lineMemmory < stringWithoutComm.Length)
                {
                    ConsoleHelper.WriteDefault(stringWithoutComm[lineMemmory]);
                    line = stringWithoutComm[lineMemmory];
                }
            }
            listTokens.Add(tokenNode);
            tokenTypeMemory = Tokens.Token.EMPTY;
            tokenBuilderMemory = null;
            return tokenNode;
        }
        
        public static void ViewTokens()
        {
            ConsoleHelper.WriteHeader("|Tokens|\t\t|Lexema|");
            ConsoleHelper.WriteSeparator();
            for (int i = 0; i < listTokens.Count; i++)
            {
                TokenNode tokenNode = listTokens[i];
                string tokenInfo = GetTokenInfo(tokenNode);

                if (tokenNode.token == Tokens.Token.FAILED)
                {
                    ConsoleHelper.WriteError(tokenInfo);
                }
                else
                {
                    ConsoleHelper.WriteSuccess(tokenInfo);
                }
            }
            ConsoleHelper.WriteSeparator();
        }

        private static string GetTokenInfo(TokenNode tokenNode)
        {
            string lineInfoToken = tokenNode.token + " <" + tokenNode.y + ":" + tokenNode.x + ">";
            string tabulation = lineInfoToken.Length > 15 ? "\t" : "\t\t";
            return lineInfoToken + tabulation + "'" + tokenNode.subString + "'";
        }
    }
}
