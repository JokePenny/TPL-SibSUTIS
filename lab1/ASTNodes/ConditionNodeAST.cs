using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ConditionNodeAST : ASTNode, IArea
    {
        private ASTNode bodyCondition; // условие
        private ASTNode body; // тело
        public ConditionNodeAST(ASTNode bodyCondition, ASTNode body)
        {
            this.bodyCondition = bodyCondition;
            this.body = body;
        }

        public ASTNode GetCondition()
        {
            return bodyCondition;
        }

        public ASTNode GetBody()
        {
            return body;
        }

        public override void Print(string level)
        {
            if (bodyCondition != null)
            {
                Console.WriteLine(level + "[CONDITION]");
                bodyCondition.Print(level + "\t");
            }
            Console.WriteLine(level + "[BODY]");
            body.Print(level + "\t");
        }

        public SymTableUse GetSymTable(string areaName, Dictionary<string, ASTNode> symTable)
        {
            throw new NotImplementedException();
        }
    }
}
