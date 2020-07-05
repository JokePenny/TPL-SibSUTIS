using System;
using System.Collections.Generic;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class IfAST : ASTNode, IArea
    {
        private ASTNode branching;
        public IfAST(ASTNode branching)
        {
            this.branching = branching;
        }

        public ASTNode GetBranching()
        {
            return branching;
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>(parentTable);
            if (branching is IArea)
                return (branching as IArea).GetSymTable("if", symTable);
            else if (branching is IStorage)
                (branching as IStorage).AddAllSymbolIn(symTable);
            return new SymTableUse("if", symTable, null);
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[IF]");
            if (branching != null) branching.Print(level + "\t");
        }

        public void ViewMemberArea()
        {
            if (branching is ISemantics)
                (branching as ISemantics).GetTypeMember();
            else if (branching is IArea)
                (branching as IArea).ViewMemberArea();
        }
	}
}
