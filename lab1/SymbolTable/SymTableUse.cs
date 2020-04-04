using System;
using System.Collections.Generic;
using lab1.ASTNodes;

namespace lab1.SymbolTable
{
    class SymTableUse
    {
        private string areaName;
        private Dictionary<string, ASTNode> symUse = new Dictionary<string, ASTNode>();
        private List<SymTableUse> nestedArea = new List<SymTableUse>();

        public SymTableUse(string areaName, Dictionary<string, ASTNode> symUse, List<SymTableUse> nestedArea)
        {
            this.areaName = areaName;
            this.symUse = symUse;
            this.nestedArea = nestedArea;
        }

        public void Print()
        {
            Console.WriteLine("[AREA] " + areaName + "\n----------\nsymbol\n----------");
            foreach(string name in symUse.Keys) Console.WriteLine(name);
            for(int i = 0; i < nestedArea.Count; i++) nestedArea[i].Print();
        }
    }
}
