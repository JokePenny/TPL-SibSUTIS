using System;
using System.Collections.Generic;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ForAST : ASTNode, IArea
    {
        private List<ASTNode> declaredVar; // int a = 5, b = 7
        private ConditionNodeAST conditionWithBody; // a < b {body}
        private ASTNode postcondition; // a++

        public ForAST(List<ASTNode> declaredVar, ConditionNodeAST conditionWithBody, ASTNode postcondition) // if(a > b) || if((a > s) > (b && s))
        {
            this.declaredVar = declaredVar;
            this.conditionWithBody = conditionWithBody;
            this.postcondition = postcondition;
        }

        public List<ASTNode> GetDeclaredVar()
        {
            return declaredVar;
        }

        public ConditionNodeAST GetCondWithBody()
        {
            return conditionWithBody;
        }

        public ASTNode GetPostcondition()
        {
            return postcondition;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[FOR]");
            string levelUp = level + "\t";
            Console.WriteLine(levelUp + "[DECLARED]");
            for (int i = 0; i < declaredVar.Count; i++)
            {
                declaredVar[i].Print(levelUp + "\t");
            }
            conditionWithBody.Print(level + "\t");
            postcondition.Print(level + "\t");
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>(parentTable);
            for (int i = 0; i < declaredVar.Count; i++)
            {
                if (declaredVar[i] is IStorage)
                    (declaredVar[i] as IStorage).AddAllSymbolIn(symTable);
            }
            if (postcondition is IStorage)
                (postcondition as IStorage).AddAllSymbolIn(symTable);
            return (conditionWithBody as IArea).GetSymTable("for", symTable);
        }

        public void ViewMemberArea()
        {
            for (int i = 0; i < declaredVar.Count; i++)
            {
                if (declaredVar[i] is ISemantics)
                    (declaredVar[i] as ISemantics).GetTypeMember();
            }

            if (conditionWithBody is ISemantics)
                (conditionWithBody as ISemantics).GetTypeMember();
            else if (conditionWithBody is IArea)
                (conditionWithBody as IArea).ViewMemberArea();

            if (postcondition is ISemantics)
                (postcondition as ISemantics).GetTypeMember();
            else if (postcondition is IArea)
                (postcondition as IArea).ViewMemberArea();
        }

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			for(int i = 0; i < declaredVar.Count; i++)
			{
				declaredVar[i].PrintASM(levelTabulatiion, true);
			}

			string markerJumpPrevBody = ASMregisters.GetNewMarkerJumpPrevBody();
			string markerJumpAfterBody = ASMregisters.GetNewMarkerJumpAfterBody();
			ASMregisters.ClearMarkerks();

			ConsoleHelper.WriteDefault(levelTabulatiion + markerJumpPrevBody + ":");

			conditionWithBody.PrintASM(levelTabulatiion + "\t", false);
			postcondition.PrintASM(levelTabulatiion + "\t", false);
			ConsoleHelper.WriteDefault(levelTabulatiion + "\t" + "jmp" + "\t" + markerJumpPrevBody);

			ConsoleHelper.WriteDefault(levelTabulatiion + markerJumpAfterBody + ":");
		}
	}
}
