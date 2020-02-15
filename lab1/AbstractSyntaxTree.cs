using System;
using System.Collections.Generic;

namespace lab1
{
    class ASTNode : AbstractSyntaxTree
    {
        public ASTNode(Nonterm nonterm, string stroke)
        {
            this.stroke = stroke;
            this.nonterm = nonterm;
        }

        public void SetAllChild(ASTNode left, ASTNode right)
        {
            this.left = left;
            this.right = right;
        }

        public void SetChild(ASTNode child, bool isLeft)
        {
            if (isLeft)
                left = child;
            else
                right = child;
        }

        private string stroke;
        private Nonterm nonterm;
        private ASTNode left;
        private ASTNode right;
    }

    class AbstractSyntaxTree
    {
        public enum Nonterm
        {
            EXP = 0,
            BOOL_EXP, // L < R, L <= R, L >= R, L == R, L ! R, L & R, L | R
            OP,     // L + R, L - R, L * R, L / R, L % R, L = R
            ID, // 
            END_STROKE, // 
            ERROR, // 
        }

        private string alphabet;

        private static int[,] matrixStateMachine = { };

        private static ASTNode headATS;

        public static void CreateAST(List<Lexer.TokenNode> listTokens)
        {
            if(listTokens.Count > 1)
            {
                Nonterm nonterm = GetNonterm((int)listTokens[0].token);
                ASTNode astNode = new ASTNode(nonterm, listTokens[0].subStroke);

                nonterm = GetNonterm((int)listTokens[1].token);
                ASTNode astNodeRight = new ASTNode(nonterm, listTokens[1].subStroke);

                astNode.SetChild(astNodeRight, false);

                for (int i = 0; i < listTokens.Count - 1; i++)
                {

                }
            }
            else if(listTokens.Count > 0)
            {
                Nonterm nonterm = GetNonterm((int)listTokens[0].token);
                ASTNode astNode = new ASTNode(nonterm, listTokens[0].subStroke);
            }
        }

        private static Nonterm GetNonterm(int tokenType)
        {
            switch (tokenType)
            {
                case 0:
                case 1:
                    return Nonterm.ID;
                case 2:
                    return Nonterm.OP;

            }
            return Nonterm.ERROR;
        }

        protected void CheckNonterm(Nonterm parent, Nonterm left, Nonterm right)
        {

        }

        protected void CheckNonterm(Nonterm parent, Nonterm side, bool isChildLeft)
        {

        }

        private void RuleTransition(int rule)
        {
            switch (rule)
            {
                case 0:
                    break;
            }
        }
    }
}
