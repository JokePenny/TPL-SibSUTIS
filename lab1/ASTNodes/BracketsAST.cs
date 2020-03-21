using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class BracketsAST : ASTNode
    {
        private ASTNode expr;

        public BracketsAST(ASTNode expr)
        {
            this.expr = expr;
        }

        public BracketsAST() {}
    }
}
