using lab1.Asm;
using System;

namespace lab1.ASTNodes
{
    class BreakAST : ASTNode
    {
        public override void Print(string level)
        {
            Console.WriteLine(level + "[BREAK]");
        }

		public override void PrintASM(string leveltabulation, bool isNewLine = false)
		{
            if(parent == null) AbstractSyntaxTree.GetParentNode(this);
            SetJumpMark(leveltabulation, parent);
        }

        private void SetJumpMark(string leveltabulation, ASTNode parent)
		{
            if(parent is ForAST parentForAST)
			{
                ASM.WriteASMCode(leveltabulation + "jmp\t" + parentForAST.MarkerJumpAfterBody);
                return;
            }

            if(parent is WhileAST parentWhileAST)
			{
                ASM.WriteASMCode(leveltabulation + "jmp\t" + parentWhileAST.MarkerJumpAfterBody);
                return;
            }

            SetJumpMark(leveltabulation, parent.parent);
        }
	}
}
