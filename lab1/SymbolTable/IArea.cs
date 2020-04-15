using System;
using System.Collections.Generic;
using System.Text;
using lab1.ASTNodes;

namespace lab1.SymbolTable
{
    interface IArea
    {
        void ViewMemberArea();
        SymTableUse GetSymTable(string areaName, Dictionary<string, ASTNode> symTable);
    }
}
