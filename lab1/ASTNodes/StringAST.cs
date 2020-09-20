using lab1.Asm;
using lab1.SemAnalyz;
using System;

namespace lab1.ASTNodes
{
    class StringAST : ASTNode, ISemantics
    {
        private readonly string stringContainer;

        public StringAST(string str)
        {
            stringContainer = str;
        }

        public string GetTypeMember()
        {
            return "string";
        }

        public string GetString()
		{
            //убирются кавычки по боками "asd" => asd
            return stringContainer.Remove(0, 1).Remove(stringContainer.Length - 2);
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[STRING] " + stringContainer);
        }

        public void PrintSringArray(string levelTabulatiion, int locateStack)
        {
            int startInStack = locateStack;
            string type = "string";
            string cutsSting = GetString();
            ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + locateStack + "], '" + cutsSting[0] + "'");
            for (int i = 1; i < cutsSting.Length; i++)
            {
                locateStack = startInStack + (ASMregisters.GetSizeStep(type) * i);
                ASMregisters.stepByte += ASMregisters.GetSizeStep(type);
                ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + locateStack + "], '" + cutsSting[i] + "'");
            }
        }
    }
}
