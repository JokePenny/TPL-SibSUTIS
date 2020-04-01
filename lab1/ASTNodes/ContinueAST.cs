using System;

namespace lab1.ASTNodes
{
    class ContinueAST : ASTNode
    {
        public override void Print(string level)
        {
            Console.WriteLine(level + "[CONTINUE]");
        }
    }
}
