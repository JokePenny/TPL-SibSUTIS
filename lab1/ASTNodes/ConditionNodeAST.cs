using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ConditionNodeAST : ASTNode, IArea
    {
        private readonly ASTNode bodyCondition;
        private readonly ASTNode body;

        public ConditionNodeAST(ASTNode bodyCondition, ASTNode body)
        {
            this.bodyCondition = bodyCondition;
            this.body = body;
        }

        public override void Print(string level)
        {
            if (bodyCondition != null)
            {
                Console.WriteLine(level + "[CONDITION]");
                bodyCondition.Print(level + "\t");
            }
            body.Print(level);
        }

        public SymTableUse GetSymTable(string areaName, Dictionary<string, ASTNode> symTable)
        {
            if (bodyCondition is IStorage bodyConditionbodyCondition)
                bodyConditionbodyCondition.AddAllSymbolIn(symTable);
            if (body is IStorage bodyIStorage)
                bodyIStorage.AddAllSymbolIn(symTable);
           if (body is IArea bodyIArea)
                return bodyIArea.GetSymTable(areaName, symTable);
            return new SymTableUse(areaName, symTable, null);
        }

        public void ViewMemberArea()
        {
            if (bodyCondition is ISemantics bodyConditionISemantics)
                bodyConditionISemantics.GetTypeMember();
            else if (bodyCondition is IArea bodyConditionIArea)
                bodyConditionIArea.ViewMemberArea();

            if (body is ISemantics bodyISemantics)
                bodyISemantics.GetTypeMember();
            else if (body is IArea bodyIArea)
                bodyIArea.ViewMemberArea();
        }

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			bool isConditionBelongToCicle = ASMregisters.isContitionBelongsToCicle;
			ASMregisters.isContitionBelongsToCicle = false;
            string markerJumpPrevBody = "";
            string markerJumpAfterBody = "";
            if (bodyCondition != null)
            {
                bodyCondition.PrintASM(levelTabulatiion);
                markerJumpPrevBody = ASMregisters.MarkerJumpPrevBody;
                markerJumpAfterBody = ASMregisters.MarkerJumpAfterBody;
                ASMregisters.ClearMarkerks();
            }
            BinaryExprAST.ClearStaicFlags();

            bool isNextNodeElse = IsNextNodeElse(markerJumpPrevBody, markerJumpAfterBody);

            if (markerJumpPrevBody != null && markerJumpPrevBody != "")
			{
				ASM.WriteASMCode(levelTabulatiion + markerJumpPrevBody + ":");
			}
			
			(body as BodyMethodAST).PrintASM(levelTabulatiion + "\t", isNewLine);

			if (!isNextNodeElse)
			{
                if (markerJumpAfterBody != null && markerJumpAfterBody != "" && !isConditionBelongToCicle)
                {
                    ASM.WriteASMCode(levelTabulatiion + markerJumpAfterBody + ":");
                }
            }
		}

		private bool IsNextNodeElse(string markerJumpPrevBody, string markerJumpAfterBody)
		{
            if(parent == null)
			{
                ASTNode node = AbstractSyntaxTree.GetParentNode(this);
                if (node != null)
                {
                    ASTNode nodeNext = (node as IArea).GetNextNode(parent);
                    if(nodeNext != null && nodeNext is ElseAST nodeNextElseAST)
					{
                        nodeNextElseAST.SetMarkersJump(markerJumpPrevBody, markerJumpAfterBody);
                        return true;
                    }
                }
            }
			else
			{
                ASTNode nodeNext = (parent.parent as IArea).GetNextNode(parent);
                if (nodeNext != null && nodeNext is ElseAST nodeNextElseAST)
                {
                    nodeNextElseAST.SetMarkersJump(markerJumpPrevBody, markerJumpAfterBody);
                    return true;
                }
            }
            return false;
        }

		public ASTNode GetNextNode(ASTNode node)
        {
            throw new NotImplementedException();
        }

        public ASTNode GetParentNode(ASTNode node, ASTNode prevNode = null)
        {
            body.parent = this;
            return (body as IArea).GetParentNode(node);
        }
    }
}
