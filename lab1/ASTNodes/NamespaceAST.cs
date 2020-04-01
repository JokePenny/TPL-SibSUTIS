using System;
using System.Collections.Generic;

namespace lab1.ASTNodes
{
    class NamespaceAST : ASTNode
    {
        private List<ASTNode> members; // члены области имен (только классы)
        private string idNamespace; // имя области имен

        public NamespaceAST(List<ASTNode> members, string idNamespace)
        {
            this.members = members;
            this.idNamespace = idNamespace;
        }

        public List<ASTNode> GetMembers()
        {
            return members;
        }

        public string GetName()
        {
            return idNamespace;
        }
        public override void Print(string level)
        {
            Console.WriteLine(level + "[NAMESPACE] " + idNamespace);
            for (int i = 0; i < members.Count; i++)
            {
                members[i].Print(level + "\t");
            }
        }
    }
}
