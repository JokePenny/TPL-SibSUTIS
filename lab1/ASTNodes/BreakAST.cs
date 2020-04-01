using System;

namespace lab1.ASTNodes
{
    class BreakAST : ASTNode
    {
        public override void Print(string level)
        {
            Console.WriteLine(level + "[BREAK]");
        }
    }
}
