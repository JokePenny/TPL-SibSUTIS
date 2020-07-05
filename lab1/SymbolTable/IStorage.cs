using lab1.ASTNodes;
using System.Collections.Generic;

namespace lab1.SymbolTable
{
    interface IStorage
    {
        void AddAllSymbolIn(Dictionary<string, ASTNode> symTable);
	}
}
