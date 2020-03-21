using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class IfNodeAST : ASTNode
    {
        private List<ASTNode> branching;
        public IfNodeAST(List<ASTNode> branching)
        {
            this.branching = branching;
        }
    }
}
