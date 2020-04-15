﻿using System;
using System.Collections.Generic;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class BinaryExprAST : ASTNode, IStorage, ISemantics
    {
        private string typeExpr;
        private string op;
        private ASTNode leftNode;
        private ASTNode rightNode;

        public BinaryExprAST(string op, ASTNode leftNode, ASTNode rightNode)
        {
            this.op = op;
            this.leftNode = leftNode;
            this.rightNode = rightNode;
        }

        public BinaryExprAST(ASTNode leftNode)
        {
            this.leftNode = leftNode;
        }

        public string GetTypeExp()
        {
            return typeExpr;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[OP] " + op);
            leftNode.Print(level + "\t");
            rightNode.Print(level + "\t");
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (leftNode is IStorage)
                (leftNode as IStorage).AddAllSymbolIn(symTable);
            if (rightNode is IStorage)
                (rightNode as IStorage).AddAllSymbolIn(symTable);
        }

        public string GetTypeMember()
        {
            string typeLeftNode = (leftNode as ISemantics).GetTypeMember();
            string typerightNode = (rightNode as ISemantics).GetTypeMember();
            if (typeLeftNode != typerightNode)
            {
                return typeLeftNode + "' " + op + " '" + typerightNode + "'";
            }
            return typeLeftNode;
        }
    }
}
