using System;
using System.Collections.Generic;

namespace lab1.ASTNodes
{
    class PostconditionForAST : ASTNode
    {
        private List<ASTNode> memberPostcondition;

        public PostconditionForAST(List<ASTNode> memberPostcondition)
        {
            this.memberPostcondition = memberPostcondition;
        }

        public List<ASTNode> GetMemberPostcondition()
        {
            return memberPostcondition;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[FOR_POSTCONDITION]");
            for (int i = 0; i < memberPostcondition.Count; i++)
            {
                memberPostcondition[i].Print(level + "\t");
            }
        }
    }
}
