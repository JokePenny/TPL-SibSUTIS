using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class WhileAST : ASTNode, IArea
    {
        public string MarkerJumpPrevBody => markerJumpPrevBody;
        public string MarkerJumpAfterBody => markerJumpAfterBody;

        private readonly ASTNode condition;
        private readonly ASTNode body;

        private string markerJumpPrevBody;
        private string markerJumpAfterBody;

        public WhileAST(ASTNode condition, ASTNode body)
        {
            this.condition = condition;
            this.body = body;
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>(parentTable);
            (condition as IStorage).AddAllSymbolIn(symTable);
            return (body as IArea).GetSymTable("while", symTable);
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[WHILE]");
            condition.Print(level + "\t");
            body.Print(level + "\t");
        }

        public void ViewMemberArea()
        {
            if (condition is ISemantics)
                (condition as ISemantics).GetTypeMember();
            else if (condition is IArea)
                (condition as IArea).ViewMemberArea();

            if (body is ISemantics)
                (body as ISemantics).GetTypeMember();
            else if (body is IArea)
                (body as IArea).ViewMemberArea();
        }

        public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
        {
            markerJumpPrevBody = ASMregisters.GetNewMarkerJumpPrevBody();
            markerJumpAfterBody = ASMregisters.GetNewMarkerJumpAfterBody();
            ASMregisters.ClearMarkerPrevBody();

            ASM.WriteASMCode(levelTabulatiion + markerJumpPrevBody + ":");
            ASMregisters.isContitionBelongsToCicle = true;

            PrintCondition(levelTabulatiion + "\t", false);

            ASM.WriteASMCode(levelTabulatiion + "\t" + "jmp" + "\t" + markerJumpPrevBody);

            ASM.WriteASMCode(levelTabulatiion + markerJumpAfterBody + ":");
        }

        private void PrintCondition(string levelTabulatiion, bool isNewLine)
		{
            bool isConditionBelongToCicle = ASMregisters.isContitionBelongsToCicle;
            ASMregisters.isContitionBelongsToCicle = false;
            string markerJumpPrevBody = "";
            string markerJumpAfterBody = "";
            if (condition != null)
            {
                condition.PrintASM(levelTabulatiion);
                markerJumpPrevBody = ASMregisters.MarkerJumpPrevBody;
                markerJumpAfterBody = ASMregisters.MarkerJumpAfterBody;

                ASMregisters.ClearMarkerks();
            }
            BinaryExprAST.ClearStaicFlags();

            if (markerJumpPrevBody != null && markerJumpPrevBody != "")
            {
                ASM.WriteASMCode(levelTabulatiion + markerJumpPrevBody + ":");
            }

            body.PrintASM(levelTabulatiion + "\t", isNewLine);

            if (markerJumpAfterBody != null && markerJumpAfterBody != "" && !isConditionBelongToCicle)
            {
                ASM.WriteASMCode(levelTabulatiion + markerJumpAfterBody + ":");
            }
        }

        public ASTNode GetParentNode(ASTNode node, ASTNode prevNode = null)
        {
            body.parent = this;
            return (body as IArea).GetParentNode(node);
        }

        public ASTNode GetNextNode(ASTNode nodePrev)
        {
            return (body as IArea).GetNextNode(nodePrev);
        }
    }
}
