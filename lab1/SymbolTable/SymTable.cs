using System;
using System.Collections.Generic;
using lab1.ASTNodes;

namespace lab1.SymbolTable
{
    public static class SymTable
    {
        public static SymTableUse symTabls;
        public static void CreateSymTable(ASTNode head)
        {
            if(head is NamespaceAST)
                symTabls = (head as NamespaceAST).GetSymTable("", null);
            symTabls.Print();
        }
    }
}
