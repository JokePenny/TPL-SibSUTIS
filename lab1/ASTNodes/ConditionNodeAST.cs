﻿using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ConditionNodeAST : ASTNode, IArea
    {
        private ASTNode bodyCondition;
        private ASTNode body;
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
            if (bodyCondition is IStorage)
                (bodyCondition as IStorage).AddAllSymbolIn(symTable);
            if (body is IStorage)
                (body as IStorage).AddAllSymbolIn(symTable);
           if (body is IArea)
                return (body as IArea).GetSymTable(areaName, symTable);
            return new SymTableUse(areaName, symTable, null);
        }

        public void ViewMemberArea()
        {
            if (bodyCondition is ISemantics)
                (bodyCondition as ISemantics).GetTypeMember();
            else if (bodyCondition is IArea)
                (bodyCondition as IArea).ViewMemberArea();

            if (body is ISemantics)
                (body as ISemantics).GetTypeMember();
            else if (body is IArea)
                (body as IArea).ViewMemberArea();
        }

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			bool isConditionBelongToCicle = ASMregisters.isContitionBelongsToCicle;
			ASMregisters.isContitionBelongsToCicle = false;
			bodyCondition.PrintASM(levelTabulatiion);
			string markerJumpPrevBody = ASMregisters.MarkerJumpPrevBody;
			string markerJumpAfterBody = ASMregisters.MarkerJumpAfterBody;
			ASMregisters.ClearMarkerks();

			if (markerJumpPrevBody != null && markerJumpPrevBody != "")
			{
				ASM.WriteASMCode(levelTabulatiion + markerJumpPrevBody + ":");
			}
			
			(body as BodyMethodAST).PrintASM(levelTabulatiion + "\t", isNewLine);

			if (markerJumpAfterBody != null && markerJumpAfterBody != "" && !isConditionBelongToCicle)
			{
				ASM.WriteASMCode(levelTabulatiion + markerJumpAfterBody + ":");
			}
		}
	}
}
