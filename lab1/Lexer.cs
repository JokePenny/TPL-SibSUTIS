using System;
using System.Collections.Generic;
using System.Text;

namespace lab1
{
    class Lexer
    {
        struct TokenNode
        {
            public TokenNode(Token token, int idPosition)
            {
                this.token = token;
                this.idPosition = idPosition;
            }

            private Token token;
            private int idPosition;
        }

        public enum Token : int
        {
            ID = 0,
            OP,
            NUM,
            VAR,
            FAILED
        }

        public void StartLexer(string stroke)
        {
            string[] strokeWithoutEmpty = DeleteEmpty(stroke);

            List<TokenNode> listTokens = new List<TokenNode>();

            for (int i = 0; i < strokeWithoutEmpty.Length; i++)
            {
                Token token = TokenDetermine(strokeWithoutEmpty[i]);

                if(token == Token.FAILED)
                {
                    throw new Exception("Токен " + strokeWithoutEmpty[i] + " по индексу - " + i + " не определился");
                }
                else
                {
                    TokenNode tokenNode = new TokenNode(token, i);
                    listTokens.Add(tokenNode);
                }
            }
        }

        private string[] DeleteEmpty(string stroke)
        {
            string deleteTabulation = stroke.Replace("\t", "");
            string deleteNewline = deleteTabulation.Replace("\n", "");
            string[] deleteSpace = deleteNewline.Split(" ");
            return deleteSpace;
        }

        public static Token TokenDetermine(string subStroke)
        {
            if (FindTokenID(subStroke)) return Token.ID;
            if (FindTokenOP(subStroke)) return Token.OP;
            if (FindTokenNUM(subStroke)) return Token.NUM;
            if (FindTokenVAR(subStroke)) return Token.VAR;
            return Token.FAILED;
        }


        private static bool FindTokenID(string subStroke)
        {
            switch (subStroke.ToLower())
            {
                case "break":
                case "while":
                case "else":
                case "if":
                case "for":
                case "string":
                case "var":
                case "char":
                case "byte":
                case "Sbyte":
                case "int":
                case "uint":
                case "double":
                case "float":
                case "long":
                case "ulong":
                case "decimal":
                    return true;
            }
            return false;
        }

        private static bool FindTokenOP(string subStroke)
        {
            switch (subStroke)
            {
                case "<":
                case "<=":
                case "=":
                case "=>":
                case "==":
                case "!=":
                case "+":
                case "-":
                case "/":
                case "%":
                case "*":
                case "^":
                    return true;
            }
            return false;
        }

        private static bool FindTokenNUM(string subStroke)
        {
            for (int i = 0; i < subStroke.Length; i++)
            {
                switch (subStroke[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        continue;
                    default:
                        return false;
                }
            }
            return true;
        }

        private static bool FindTokenVAR(string subStroke)
        {
            return true;
        }
    }
}
