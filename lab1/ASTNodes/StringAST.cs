using System;

namespace lab1.ASTNodes
{
    class StringAST : ASTNode
    {
        private string str;

        public StringAST(string str)
        {
            this.str = str;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[STRING] " + str);
        }
    }
}
