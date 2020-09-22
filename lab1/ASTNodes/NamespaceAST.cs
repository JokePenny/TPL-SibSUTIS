using System;
using System.Collections.Generic;
using lab1.Asm;
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

        public ASTNode GetParentNode(ASTNode node, ASTNode prevNode = null)
        {
            for(int i = 0; i < members.Count; i++)
			{
                members[i].parent = this;
                if (node == members[i]) return this;

                if (members[i] is IArea memberIArea)
                {
                    ASTNode findParentNode = memberIArea.GetParentNode(node);
                    if (findParentNode != null) return findParentNode;
                }
            }
            return null;
        }

        public ASTNode GetNextNode(ASTNode node)
        {
            int index = members.FindIndex(obj => obj == node);
            index++;
            if (index > members.Count) return null;
            return members[index];
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
            ASM.WriteASMCode
			(
                "format PE Console 4.0\n"
                + "entry Start\n"
				+ "include 'INCLUDE\\WIN32AX.inc'\n"
				+ "section '.data' data readable writable\n"
				+ "\tshowString db '%d', 0\n"
                + "\tspaceString db ' ', 0\n"
                + "\tnewString db 10, 13\n"
                + "section '.idata' import data readable\n"
				+ "\tlibrary kernel, 'kernel32.dll',\\\n"
				+ "\t\tmsvcrt, 'msvcrt.dll'\n"
				+ "\timport kernel,\\\n"
				+ "\t\tExitProcess, 'ExitProcess'\n"
				+ "\timport msvcrt,\\\n"
				+ "\t\tprintf, 'printf',\\\n"
				+ "\t\tscanf, 'scanf',\\\n"
				+ "\t\tgetch, '_getch'\n"
				+ "Start:\n"
                + "\t\tpush\teax"
            );

			for (int i = 0; i < members.Count; i++)
			{
				members[i].PrintASM(levelTabulatiion, true);
			}

            ASM.WriteASMCode
			(
				"Exit:\n"
				+ "\tpush\teax\n"
                + "\tcall\t[getch]\n"
			);
		}
	}
}
