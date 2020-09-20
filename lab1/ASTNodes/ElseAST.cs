using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ElseAST : ASTNode, IArea
    {
        private readonly ASTNode expr;
        private string markerPrevBody;
        private string markerAfterBody;

        public ElseAST(ASTNode expr)
        {
            this.expr = expr;
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>(parentTable);
            if (expr is IArea)
                return (expr as IArea).GetSymTable("else", symTable);
            else if (expr is IStorage)
                (expr as IStorage).AddAllSymbolIn(symTable);
            return new SymTableUse("else", symTable, null);
        }

        public void SetMarkersJump(string markerJumpPresvBody, string markerJumpAfterBody)
		{
            markerPrevBody = markerJumpPresvBody;
            markerAfterBody = markerJumpAfterBody;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[ELSE]");
            expr.Print(level + "\t");
        }

        public void ViewMemberArea()
        {
            if (expr is ISemantics)
                (expr as ISemantics).GetTypeMember();
            else if (expr is IArea)
                (expr as IArea).ViewMemberArea();
        }

        public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
            ASM.WriteASMCode(levelTabulatiion + "jmp\t" + markerPrevBody);
            ASM.WriteASMCode(levelTabulatiion + markerAfterBody + ":");
            (expr as BodyMethodAST).PrintASM(levelTabulatiion, isNewLine);

            ASM.WriteASMCode(levelTabulatiion + markerPrevBody + ":");

            //ASM.WriteASMCode(levelTabulatiion + "jmp\t" + markerAfterBody);
            //ASM.WriteASMCode(levelTabulatiion + markerPrevBody + ":");
            //(expr as BodyMethodAST).PrintASM(levelTabulatiion, isNewLine);

            //ASM.WriteASMCode(levelTabulatiion + markerAfterBody + ":");
        }

        public ASTNode GetNextNode(ASTNode node)
        {
            throw new NotImplementedException();
        }

        public ASTNode GetParentNode(ASTNode node, ASTNode prevNode = null)
        {
            expr.parent = parent;
            if (expr is IArea exprIArea)
                return exprIArea.GetParentNode(node);
            return null;
        }

    }
}
