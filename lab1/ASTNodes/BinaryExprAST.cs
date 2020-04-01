using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    // BinaryExprAST - Класс узла выражения для бинарных операторов.
    class BinaryExprAST : ASTNode
    {
        private string typeExpr;
        private string op;
        private ASTNode LeftNode;
        private ASTNode RightNode;

        public BinaryExprAST(string op, ASTNode LeftNode, ASTNode RightNode)
        {
            this.op = op;
            this.LeftNode = LeftNode;
            this.RightNode = RightNode;
        }

        public BinaryExprAST(ASTNode LeftNode)
        {
            this.LeftNode = LeftNode;
        }

        public ASTNode GetLeftNode()
        {
            return LeftNode;
        }

        public ASTNode GetRightNode()
        {
            return RightNode;
        }

        public string GetOp()
        {
            return op;
        }

        public string GetTypeExp()
        {
            return typeExpr;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[OP] " + op);
            LeftNode.Print(level + "\t");
            RightNode.Print(level + "\t");
        }
    }
}
