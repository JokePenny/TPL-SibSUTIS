using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class ClassAST : ASTNode
    {
        private AccessModifierAST modifier;
        private List<ASTNode> members; // члены области имен (только классы)
        private string idClass; // имя области имен

        public ClassAST(List<ASTNode> members, string idClass)
        {
            this.members = members;
            this.idClass = idClass;
            modifier = new AccessModifierAST("private");
        }

        public ClassAST(AccessModifierAST modifier, List<ASTNode> members, string idClass)
        {
            this.members = members;
            this.idClass = idClass;
            this.modifier = modifier;
        }
    }
}
