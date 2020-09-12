using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    // []
    class BracketsAST : ASTNode, IStorage, ISemantics
    {
        private readonly string type = "";
        private readonly ASTNode expr;
        private readonly ASTNode storage;

        public BracketsAST(string type, ASTNode expr, Point point)
        {
            this.type = type;
            this.expr = expr;
            this.point = point;
        }

        public BracketsAST(ASTNode storage, ASTNode expr)
        {
            this.storage = storage;
            this.expr = expr;
        }

        public BracketsAST(ASTNode expr)
        {
            this.expr = expr;
        }

        public BracketsAST() {}

        public override void Print(string level)
        {
            if (storage != null) storage.Print(level);
            if (type != "") Console.WriteLine(level + "[TYPE] " + type);
            Console.WriteLine(level + "[BRACKET_L] [");
            expr.Print(level + "\t");
            Console.WriteLine(level + "[BRACKET_R] ]");
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (expr is IStorage)
                (expr as IStorage).AddAllSymbolIn(symTable);
            if (storage is IStorage)
                (storage as IStorage).AddAllSymbolIn(symTable);
        }

        public string GetTypeMember()
        {
            string typeExpr = (expr as ISemantics).GetTypeMember();
            if (typeExpr != "int")
            {
                ConsoleHelper.WriteError("<" + point.y + "," + point.x + ">: Wrong type in brackets, expected 'int'");
                return "['" + typeExpr + "']";
            }
            return "int";
        }

		public int GetSizeArray()
		{
			if (expr is IEject)
			{
				return Convert.ToInt32((expr as IEject).GetValue());
			}
			return 0;
		}

		//public override void PrintASM(string levelTabulation, bool isNewLine)
		//{
		//	int sizeArray = Convert.ToInt32((storage as IEject).GetValue());
		//	(expr as IdentificatorAST).PrintASM(levelTabulation, sizeArray);
		//}
	}
}
