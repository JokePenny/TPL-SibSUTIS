using System;
using System.Collections.Generic;
using System.Text;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ClassAST : ASTNode, IArea
    {
        private List<ASTNode> members; // члены области имен (только классы)
        private string idClass; // имя области имен

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
                    (members[i] as IStorage).SetNewSymbolIn(symTable);
                else if (members[i] is IArea)
                    nestedArea.Add((members[i] as IArea).GetSymTable("", symTable));
            }
            return new SymTableUse(idClass, symTable, nestedArea);
        }
    }
}
