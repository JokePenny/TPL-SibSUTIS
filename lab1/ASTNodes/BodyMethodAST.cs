using System;
using System.Collections.Generic;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class BodyMethodAST : ASTNode, IArea
    {
        private List<ASTNode> memberMethod;

        public BodyMethodAST(List<ASTNode> memberMethod)
        {
            this.memberMethod = memberMethod;
        }

        public List<ASTNode> GetMemberMethod()
        {
            return memberMethod;
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

		public override void PrintASM(bool isNewLine)
		{
			for (int i = 0; i < memberMethod.Count; i++)
			{
				memberMethod[i].PrintASM(true);
			}
		}
	}
}
