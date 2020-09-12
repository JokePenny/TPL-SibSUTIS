using lab1.SemAnalyz;
using System;

namespace lab1.ASTNodes
{
    class CharAST : ASTNode, ISemantics
    {
        private readonly string str;

        public CharAST(string str, Point point)
        {
            this.str = str;
            this.point = point;
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
