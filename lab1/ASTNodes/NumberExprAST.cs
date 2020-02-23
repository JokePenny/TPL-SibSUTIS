using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    // NumberExprAST - Класс узла выражения для числовых литералов (Например, "1.0").
    class NumberExprAST : ASTNode
    {
        double Val;
        public NumberExprAST(double Val)
        {
            this.Val = Val;
        }
    }
}
