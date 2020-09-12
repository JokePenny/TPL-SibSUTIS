using System;
using System.Collections.Generic;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class BoolAST : ASTNode, IStorage, ISemantics
    {
        private readonly ASTNode exp;
        private readonly string result = "";

        public BoolAST(string result, ASTNode exp, Point point)
        {
            this.result = result;
            this.exp = exp;
            this.point = point;
        }

        public BoolAST(ASTNode exp, Point point)
        {
            this.exp = exp;
            this.point = point;
        }

        public BoolAST(string result, Point point)
        {
            this.result = result;
            this.point = point;
        }

		public ASTNode GetExpression()
		{
			return exp;
		}

        public override void Print(string level)
        {
            if(result != "") Console.WriteLine(level + "[BOOL] " + result);
            if (exp != null) exp.Print(level + "\t");
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (exp is IStorage)
                (exp as IStorage).AddAllSymbolIn(symTable);
        }

        public string GetTypeMember()
        {
            if(exp != null)
            {
                (exp as ISemantics).GetTypeMember();
            }
            return "bool";
        }

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			(exp as BinaryExprAST).PrintASM(levelTabulatiion, isNewLine);
		}
	}
}
