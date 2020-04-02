using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class ElseAST : ASTNode
    {
        private ASTNode expr;

        public ElseAST(ASTNode expr)
        {
            this.expr = expr;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[ELSE]");
            expr.Print(level + "\t");
        }
    }
}
