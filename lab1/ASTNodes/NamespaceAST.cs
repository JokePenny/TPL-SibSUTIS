﻿using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class NamespaceAST : ASTNode, IArea
    {
        private List<ASTNode> members;
        private string idNamespace;

        public NamespaceAST(List<ASTNode> members, string idNamespace)
        {
            this.members = members;
            this.idNamespace = idNamespace;
        }

        public List<ASTNode> GetMembers()
        {
            return members;
        }

        public string GetName()
        {
            return idNamespace;
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>();
            List<SymTableUse> nestedArea = new List<SymTableUse>();
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i] is IStorage)
                    (members[i] as IStorage).AddAllSymbolIn(symTable);
                if (members[i] is IArea)
                    nestedArea.Add((members[i] as IArea).GetSymTable("", symTable));
            }
            return new SymTableUse(idNamespace, symTable, nestedArea);
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[NAMESPACE] " + idNamespace);
            for (int i = 0; i < members.Count; i++)
            {
                members[i].Print(level + "\t");
            }
        }
    }
}
