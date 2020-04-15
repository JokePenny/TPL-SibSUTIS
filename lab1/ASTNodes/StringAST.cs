using lab1.SemAnalyz;
using System;

namespace lab1.ASTNodes
{
    class StringAST : ASTNode, ISemantics
    {
        private string str;

        public StringAST(string str)
        {
            this.str = str;
        }

        public string GetTypeMember()
        {
            return "string";
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[STRING] " + str);
        }
    }
}
