using lab1.SemAnalyz;
using System;

namespace lab1.ASTNodes
{
    class StringAST : ASTNode, ISemantics
    {
        public readonly string stringContainer;

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
	}
}
