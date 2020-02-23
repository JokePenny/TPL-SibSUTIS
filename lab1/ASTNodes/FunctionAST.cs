using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    // FunctionAST - Представляет определение самой функции
    class FunctionAST : ASTNode
    {
        PrototypeAST Proto;
        ASTNode Body;
        public FunctionAST(PrototypeAST Proto, ASTNode Body)
        {
            this.Proto = Proto;
            this.Body = Body;
        }
    }
}
