using System;
using System.Collections.Generic;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class BodyMethodAST : ASTNode, IArea
    {
        private readonly List<ASTNode> memberMethod;

        public BodyMethodAST(List<ASTNode> memberMethod)
        {
            this.memberMethod = memberMethod;
        }

        public ASTNode GetParentNode(ASTNode node, ASTNode prevNode = null)
        {
            for (int i = 0; i < memberMethod.Count; i++)
            {
                memberMethod[i].parent = this;

                if (node == memberMethod[i]) return this;

                if(memberMethod[i] is IArea memberIArea)
				{
                    ASTNode findParentNode = memberIArea.GetParentNode(node, this);
                    if (findParentNode != null) return findParentNode;
                }
            }
            return null;
        }

        public ASTNode GetNextNode(ASTNode nodePrev)
        {
            int index = memberMethod.FindIndex(obj => obj == nodePrev);
            if (index == -1) return null;
            index++;
            if (index >= memberMethod.Count) return null;
            return memberMethod[index];
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[BODY]");
            
            for(int i = 0; i < memberMethod.Count; i++)
            {
                memberMethod[i].Print(level + "\t");
            }
        }

        public SymTableUse GetSymTable(string areaName, Dictionary<string, ASTNode> parentTable)
        {
            List<SymTableUse> nestedArea = new List<SymTableUse>();
            for (int i = 0; i < memberMethod.Count; i++)
            {
                if (memberMethod[i] is IStorage)
                    (memberMethod[i] as IStorage).AddAllSymbolIn(parentTable);
                else if (memberMethod[i] is IArea)
                    nestedArea.Add((memberMethod[i] as IArea).GetSymTable("", parentTable));
            }
            return new SymTableUse(areaName, parentTable, nestedArea);
        }

        public void ViewMemberArea()
        {
            for (int i = 0; i < memberMethod.Count; i++)
            {
                if (memberMethod[i] is ISemantics)
                    (memberMethod[i] as ISemantics).GetTypeMember();
                else if (memberMethod[i] is IArea)
                    (memberMethod[i] as IArea).ViewMemberArea();
            }
        }

		public override void PrintASM(string levelTabulation, bool isNewLine)
		{
			for (int i = 0; i < memberMethod.Count; i++)
			{
				memberMethod[i].PrintASM(levelTabulation, true);
			}
		}
	}
}
