using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    // VariableExprAST - Класс узла выражения для переменных (например, "a").
    class IdentificatorExprAST : ASTNode
    {
        public string nameID;
        public IdentificatorExprAST(string nameID)
        {
            this.nameID = nameID;
        }
    }
}
