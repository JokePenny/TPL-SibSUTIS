using System;
using System.Collections.Generic;
using lab1.ASTNodes;

namespace lab1.SymbolTable
{
    public class SymTableUse
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
            Console.WriteLine("----------\n[AREA] " + areaName + "\n----------");
            foreach(string name in symUse.Keys) Console.WriteLine(name);
            if (nestedArea == null) return;
            for (int i = 0; i < nestedArea.Count; i++)
            {
                if(nestedArea[i] != null) nestedArea[i].Print();
            }
        }
    }
}
