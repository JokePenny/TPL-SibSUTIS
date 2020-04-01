using System;

namespace lab1.ASTNodes
{
    class ParenthesisExprAST : ASTNode
    {
        private ASTNode node;

        public ParenthesisExprAST(ASTNode node)
        {
            this.node = node;
        }

        public ASTNode GetNode()
        {
            return node;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[PARENTHESIS_L] (");
            node.Print(level + "\t");
            Console.WriteLine(level + "[PARENTHESIS_R] )");
        }
    }
}
