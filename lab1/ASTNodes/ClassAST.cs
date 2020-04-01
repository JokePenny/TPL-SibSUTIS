using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class ClassAST : ASTNode
    {
        private List<ASTNode> members; // члены области имен (только классы)
        private string idClass; // имя области имен

        public ClassAST(List<ASTNode> members, string idClass)
        {
            this.members = members;
            this.idClass = idClass;
        }

        public string GetName()
        {
            return idClass;
        }

        public List<ASTNode> GetMembers()
        {
            return members;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[CLASS] " + idClass);
            for (int i = 0; i < members.Count; i++)
            {
                members[i].Print(level + "\t");
            }
        }
    }
}
