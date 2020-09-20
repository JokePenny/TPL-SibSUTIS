using System;
using System.Collections.Generic;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ClassAST : ASTNode, IArea, IStorage
    {
        private readonly List<ASTNode> members;
        private readonly string idClass;

        public ClassAST(List<ASTNode> members, string idClass)
        {
            this.members = members;
            this.idClass = idClass;
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

		public ASTNode GetNextNode(ASTNode node)
		{
			throw new NotImplementedException();
		}

        public ASTNode GetParentNode(ASTNode node, ASTNode prevNode = null)
        {
            for (int i = 0; i < members.Count; i++)
            {
                members[i].parent = this;
                if (node == members[i]) return this;

                if (members[i] is IArea memberMethodAST)
                {
                    ASTNode findParentNode = memberMethodAST.GetParentNode(node);
                    if (findParentNode != null) return findParentNode;
                }
            }
            return null;
        }

    }
}
