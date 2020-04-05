using System;
using System.Collections.Generic;
using lab1.Helpers;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ClassAST : ASTNode, IArea, IStorage
    {
        private List<ASTNode> members;
        private string idClass;

        public ClassAST(List<ASTNode> members, string idClass)
        {
            this.members = members;
            this.idClass = idClass;
        }

        public string GetName()
        {
            return idClass;
        }

        public List<ASTNode> GetMembers()
        {
            return members;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[CLASS] " + idClass);
            for (int i = 0; i < members.Count; i++)
            {
                members[i].Print(level + "\t");
            }
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>(parentTable);
            List<SymTableUse> nestedArea = new List<SymTableUse>();
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i] is IStorage)
                    (members[i] as IStorage).AddAllSymbolIn(symTable);
                if (members[i] is IArea)
                    nestedArea.Add((members[i] as IArea).GetSymTable("", symTable));
            }
            return new SymTableUse(idClass, symTable, nestedArea);
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (symTable.ContainsKey(idClass))
            {
                ConsoleHelper.WriteError(idClass + " - Variable is redeclared");
            }
            else symTable.Add(idClass, this);
        }
    }
}
