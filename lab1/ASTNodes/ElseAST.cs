using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ElseAST : ASTNode, IArea
    {
        private ASTNode expr;

        public ElseAST(ASTNode expr)
        {
            this.expr = expr;
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>(parentTable);
            if (expr is IArea)
                return (expr as IArea).GetSymTable("else", symTable);
            else if (expr is IStorage)
                (expr as IStorage).AddAllSymbolIn(symTable);
            return new SymTableUse("else", symTable, null);
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[ELSE]");
            expr.Print(level + "\t");
        }
    }
}
