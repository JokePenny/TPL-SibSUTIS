using lab1.Helpers;
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

    public class Lexer
    {
        private static List<TokenNode> listTokens = new List<TokenNode>();

        // виды токенов
        public enum Token : int
        {
            KEYWORD = 0,
            K_FOREACH,
            K_FOR,
            K_IF,
            K_WHILE,
            K_DO,
            K_STRUCT,
            K_CLASS,
            K_NAMESPACE,
            K_ENUM,
            K_NEW,
            K_ELSE,
            K_RETURN,
            COMM,
            BRACE_L,
            BRACE_R,
            PARENTHESIS_L,
            PARENTHESIS_R,
            BRACKET_L,
            BRACKET_R,
            INCREMENT,
            DECREMENT,
            ASSIGNMENT,
            ASSIGNMENT_PLUS,
            ASSIGNMENT_MINUS,
            ASSIGNMENT_MULTY,
            ASSIGNMENT_DIVISION,
            ACCESS_MODIFIER,
            TYPE,
            BOOL,
            OP,
            NUM,
            NUM_REAL,
            NUM_16X,
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
        static string[] line;
        static string[] stringWithoutComm;

        public static void StartLexer(string str)
        {
            // заполнение словарей
            ReadSource.FillTokens();
            // удаление комментариев и табуляции
            stringWithoutComm = StringTreatment.DeleteCommentsAndTab(str);
            line = StringTreatment.FormatStroke(stringWithoutComm[lineMemmory]);
            ConsoleHelper.WriteDefault(stringWithoutComm[lineMemmory]);
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
            if (Regex.IsMatch(buildToken, Tokens.String)) return Token.STRING;
            if (Regex.IsMatch(buildToken, Tokens.Type)) return Token.TYPE;
            if (Regex.IsMatch(buildToken, Tokens.Keyword)) return Token.KEYWORD;
            if (Regex.IsMatch(buildToken, Tokens.Operator)) return Token.OP;
            if (Regex.IsMatch(buildToken, Tokens.Twins)) return Token.TWINS;
            if (Regex.IsMatch(buildToken, Tokens.Num)) return Token.NUM;
            if (Regex.IsMatch(buildToken, Tokens.NumReal)) return Token.NUM_REAL;
            if (Regex.IsMatch(buildToken, Tokens.Char)) return Token.CHAR;
            if (Regex.IsMatch(buildToken, Tokens.Id)) return Token.ID;
            if (Regex.IsMatch(buildToken, Tokens.ForbiddenSymbolsId)) return Token.FAILED;
            if (buildToken[0] == Tokens.Semicolon) return Token.SEMILICON;
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
                    ConsoleHelper.WriteDefault(stringWithoutComm[lineMemmory]);
                    line = StringTreatment.FormatStroke(stringWithoutComm[lineMemmory]);
                }
            }
            listTokens.Add(tokenNode);
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

                if (tokenNode.token == Token.FAILED)
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
