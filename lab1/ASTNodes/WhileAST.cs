using System;
using System.Collections.Generic;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class WhileAST : ASTNode, IArea
    {
        private readonly ASTNode condition;
        private readonly ASTNode body;

        public WhileAST(ASTNode condition, ASTNode body)
        {
            this.condition = condition;
            this.body = body;
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>(parentTable);
            (condition as IStorage).AddAllSymbolIn(symTable);
            return (body as IArea).GetSymTable("while", symTable);
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[WHILE]");
            condition.Print(level + "\t");
            body.Print(level + "\t");
        }

        public void ViewMemberArea()
        {
            if (condition is ISemantics)
                (condition as ISemantics).GetTypeMember();
            else if (condition is IArea)
                (condition as IArea).ViewMemberArea();

            if (body is ISemantics)
                (body as ISemantics).GetTypeMember();
            else if (body is IArea)
                (body as IArea).ViewMemberArea();
        }
	}
}
