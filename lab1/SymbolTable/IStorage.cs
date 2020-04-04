using lab1.ASTNodes;
using System.Collections.Generic;

namespace lab1.SymbolTable
{
    interface IStorage
    {
        void SetNewSymbolIn(Dictionary<string, ASTNode> symTable);
    }
}
