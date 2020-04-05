using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class CrementAST : ASTNode, IStorage
    {
        private string crement;
        private ASTNode id;

        public CrementAST(string crement, ASTNode id)
        {
            this.crement = crement;
            this.id = id;
        }

        public string GetCrement()
        {
            return crement;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[CREMENT] " + crement);
            id.Print(level + "\t");
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (id is IStorage)
                (id as IStorage).AddAllSymbolIn(symTable);
        }
    }
}
