using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    // []
    class BracketsAST : ASTNode, IStorage
    {
        private string type = "";
        private ASTNode expr;
        private ASTNode storage;

        public BracketsAST(string type, ASTNode expr)
        {
            this.type = type;
            this.expr = expr;
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

        public ASTNode GetNode()
        {
            return expr;
        }
        public override void Print(string level)
        {
            if (storage != null) storage.Print(level);
            if (type != "") Console.WriteLine(level + "[TYPE] " + type);
            Console.WriteLine(level + "[BRACKET_L] [");
            expr.Print(level + "\t");
            Console.WriteLine(level + "[BRACKET_R] ]");
        }

        public void SetNewSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            throw new NotImplementedException();
        }
    }
}
