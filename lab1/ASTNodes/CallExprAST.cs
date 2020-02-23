using System;
using System.Collections.Generic;

namespace lab1.ASTNodes
{
    // CallExprAST - Класс узла выражения для вызова функции.
    class CallExprAST : ASTNode
    {
        string Callee;
        List<ASTNode> Args;
        public CallExprAST(string Callee, List<ASTNode> Args)
        {
            this.Callee = Callee;
            this.Args = Args;
        }
    }
}
