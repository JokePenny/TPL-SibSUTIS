using System;

namespace lab1.ASTNodes
{
    class CharAST : ASTNode
    {
        private string str;

        public CharAST(string str)
        {
            this.str = str;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[CHAR] " + str);
        }
    }
}
