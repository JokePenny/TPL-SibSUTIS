using lab1.SemAnalyz;
using System;

namespace lab1.ASTNodes
{
    class CharAST : ASTNode, ISemantics
    {
        private string str;

        public CharAST(string str)
        {
            this.str = str;
        }

        public string GetTypeMember()
        {
            return "char";
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[CHAR] " + str);
        }
    }
}
