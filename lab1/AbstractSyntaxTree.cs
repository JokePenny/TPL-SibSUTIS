using lab1.ASTNodes;
using System;
using System.Collections;

namespace lab1
{
    class AbstractSyntaxTree
    {
        private static Hashtable hashtable = new Hashtable();
        private static TokenNode CurTok;
        private static ASTNode headAST;
        public static void CreateAST()
        {
            //headAST = ParseExpression();
            ViewAST();
        }

        public static void ViewAST()
        {
            DescentSide(headAST);
        }

        private static void getNextToken()
        {
            CurTok = Lexer.GetToken();
        }

        private static ASTNode ParsePrimary()
        {
            getNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Lexer.Token.CHAR:
                    return null;
                case Lexer.Token.OP:
                    return null;
                case Lexer.Token.NUM:
                    return null;
                case Lexer.Token.NUM_REAL:
                    return null;
                case Lexer.Token.ID:
                    return null;
                case Lexer.Token.TWINS:
                    return null;
                case Lexer.Token.SEMILICON:
                    return null;
                case Lexer.Token.STRING:
                    return null;
                case Lexer.Token.NULL:
                    return null;
                case Lexer.Token.TYPE:
                    return null;
                case Lexer.Token.FAILED:
                    return null;
                default:
                    Error("unknown token when expecting an expression");
                    return null;
            }
        }

        private static void Error(string str)
        {
            Console.WriteLine("Error: {str}\n");
        }

        private static void DescentSide(ASTNode nodeAST)
        {
            if(nodeAST != null)
            {
                Console.WriteLine(nodeAST);
                if (nodeAST is BinaryExprAST)
                {
                    DescentSide((nodeAST as BinaryExprAST).LeftNode);
                    Console.WriteLine((nodeAST as BinaryExprAST).Op);
                    DescentSide((nodeAST as BinaryExprAST).RightNode);
                    return;
                }
                if(nodeAST is IdentificatorExprAST)
                {
                    Console.WriteLine((nodeAST as IdentificatorExprAST).nameID);
                }
            }
        }
    }
}
