using lab1.SemAnalyz;
using System;

namespace lab1.ASTNodes
{
    class NumAST : ASTNode, ISemantics
    {
        private string typeValue;
        private string value;

        public NumAST(Tokens.Token typeValue, string value)
        {
            this.typeValue = DeterminateType(typeValue);
            this.value = value;
        }

        private string DeterminateType(Tokens.Token typeValue)
        {
            switch (typeValue)
            {
                case Tokens.Token.INT_VALUE:
                    return "int";
                case Tokens.Token.DOUBLE_VALUE:
                    return "double";
                case Tokens.Token.X16_VALUE:
                    return "16x";
                case Tokens.Token.X8_VALUE:
                    return "8x";
                case Tokens.Token.X2_VALUE:
                    return "2x";
                default:
                    return "";
            }
        }

        public string GetTypeValue()
        {
            return typeValue;
        }

        public string GetValue()
        {
            return value;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[NUM] " + typeValue + " " + value);
        }

        public string GetTypeMember()
        {
            return typeValue;
        }
    }
}
