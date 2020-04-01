using System;

namespace lab1.ASTNodes
{
    class WhileAST : ASTNode
    {
        private ASTNode condition;
        private ASTNode body;

        public WhileAST(ASTNode condition, ASTNode body)
        {
            this.condition = condition;
            this.body = body;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[WHILE]");
            condition.Print(level + "\t");
            body.Print(level + "\t");
        }
    }
}
