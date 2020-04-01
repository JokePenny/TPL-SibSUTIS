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

        public List<ASTNode> GetMemberMethod()
        {
            return memberMethod;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[BODY]");
            
            for(int i = 0; i < memberMethod.Count; i++)
            {
                memberMethod[i].Print(level + "\t");
            }
        }
    }
}
