using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class BodyMethodAST : ASTNode
    {
        private List<ASTNode> memberMethod;

        public BodyMethodAST(List<ASTNode> memberMethod)
        {
            this.memberMethod = memberMethod;
        }
    }
}
