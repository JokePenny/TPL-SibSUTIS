using lab1.ASTNodes;
using lab1.SymbolTable;
using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.SemAnalyz
{
    class SemAnalyzer
    {
        public static void StartSemAnalyzer(ASTNode head, SymTableUse symTabls)
        {
            if (head is NamespaceAST) (head as NamespaceAST).ViewMemberArea();
        }
    }
}
