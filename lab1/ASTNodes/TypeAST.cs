using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class TypeAST : ASTNode
    {
        private string typeName;
        private List<ASTNode> memberBrackets;
        public TypeAST(string typeName, List<ASTNode> memberBrackets)
        {
            this.typeName = typeName;
            this.memberBrackets = memberBrackets;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[TYPE] " + typeName);
            if(memberBrackets != null)
            {
                Console.WriteLine(level + "[ARRAY-INIT]");
                for (int i = 0; i < memberBrackets.Count; i++)
                {
                    memberBrackets[i].Print(level + "\t");
                }
            }
        }
    }
}
