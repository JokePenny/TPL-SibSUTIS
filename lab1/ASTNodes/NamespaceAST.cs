using System;
using System.Collections.Generic;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    public class NamespaceAST : ASTNode, IArea
    {
        private readonly List<ASTNode> members;
        private readonly string idNamespace;

        public NamespaceAST(List<ASTNode> members, string idNamespace)
        {
            this.members = members;
            this.idNamespace = idNamespace;
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>();
            List<SymTableUse> nestedArea = new List<SymTableUse>();
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i] is IStorage)
                    (members[i] as IStorage).AddAllSymbolIn(symTable);
                if (members[i] is IArea)
                    nestedArea.Add((members[i] as IArea).GetSymTable("", symTable));
            }
            return new SymTableUse(idNamespace, symTable, nestedArea);
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[NAMESPACE] " + idNamespace);
            for (int i = 0; i < members.Count; i++)
            {
                members[i].Print(level + "\t");
            }
        }

        public void ViewMemberArea()
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i] is ISemantics)
                    (members[i] as ISemantics).GetTypeMember();
                else if (members[i] is IArea)
                    (members[i] as IArea).ViewMemberArea();
            }
        }

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			for (int i = 0; i < members.Count; i++)
			{
				members[i].PrintASM(levelTabulatiion, true);
			}
		}
	}
}
