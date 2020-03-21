using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class PostconditionForAST : ASTNode
    {
        private List<ASTNode> memberPostcondition;

        public PostconditionForAST(List<ASTNode> memberPostcondition)
        {
            this.memberPostcondition = memberPostcondition;
        }
    }
}
