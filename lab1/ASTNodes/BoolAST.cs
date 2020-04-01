using System;

namespace lab1.ASTNodes
{
    class BoolAST : ASTNode
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
    }
}
