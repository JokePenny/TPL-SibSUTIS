using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class TypeAST : ASTNode
    {
        private string typeName;

        public TypeAST(string typeName)
        {
            this.typeName = typeName;
        }
    }
}
