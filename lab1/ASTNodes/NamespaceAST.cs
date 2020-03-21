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
    }
}
