using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    // BinaryExprAST - Класс узла выражения для бинарных операторов.
    class BinaryExprAST : ASTNode
    {
        public string Op;
        public ASTNode LeftNode;
        public ASTNode RightNode;
        public BinaryExprAST(string Op, ASTNode LeftNode, ASTNode RightNode)
        {
            this.Op = Op;
            this.LeftNode = LeftNode;
            this.RightNode = RightNode;
        }
    }
}
