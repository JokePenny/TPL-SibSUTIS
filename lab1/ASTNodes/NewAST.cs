using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class NewAST : ASTNode, IStorage
    {
        private ASTNode storageType;

        public NewAST(ASTNode storageType)
        {
            this.storageType = storageType;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[NEW]");
            storageType.Print(level + "\t");
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (storageType is IStorage)
                (storageType as IStorage).AddAllSymbolIn(symTable);
        }
    }
}
