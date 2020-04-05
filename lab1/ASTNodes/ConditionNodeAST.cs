using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ConditionNodeAST : ASTNode, IArea
    {
        private ASTNode bodyCondition;
        private ASTNode body;
        public ConditionNodeAST(ASTNode bodyCondition, ASTNode body)
        {
            this.bodyCondition = bodyCondition;
            this.body = body;
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
            if (bodyCondition is IStorage)
                (bodyCondition as IStorage).AddAllSymbolIn(symTable);
            if (body is IStorage)
                (body as IStorage).AddAllSymbolIn(symTable);
           if (body is IArea)
                return (body as IArea).GetSymTable(areaName, symTable);
            return new SymTableUse(areaName, symTable, null);
        }
    }
}
