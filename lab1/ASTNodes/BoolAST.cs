using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class BoolAST : ASTNode, IStorage
    {
        private ASTNode exp;
        private string result;

        public BoolAST(string result, ASTNode exp)
        {
            this.result = result;
            this.exp = exp;
        }

        public BoolAST(ASTNode exp)
        {
            this.exp = exp;
        }

        public BoolAST(string result)
        {
            this.result = result;
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
    }
}
