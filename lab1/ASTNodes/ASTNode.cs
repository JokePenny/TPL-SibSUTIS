using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    class ASTNode
    {
        public virtual void Print(string level) { }
        ~ASTNode() { }
    }
}
