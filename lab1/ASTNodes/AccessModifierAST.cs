using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class AccessModifierAST : ASTNode
    {
        private string modifier;

        public AccessModifierAST(string modifier)
        {
            this.modifier = modifier;
        }
        public AccessModifierAST()
        {
            modifier = "private";
        }
    }
}
