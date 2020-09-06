﻿using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class TypeAST : ASTNode, IStorage, ISemantics
    {
        private string typeName;
        private List<ASTNode> memberBrackets;
        public TypeAST(string typeName, List<ASTNode> memberBrackets)
        {
            this.typeName = typeName;
            this.memberBrackets = memberBrackets;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[TYPE] " + typeName);
            if(memberBrackets != null)
            {
                Console.WriteLine(level + "[ARRAY-INIT]");
                for (int i = 0; i < memberBrackets.Count; i++)
                {
                    memberBrackets[i].Print(level + "\t");
                }
            }
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (memberBrackets == null) return;
            for (int i = 0; i < memberBrackets.Count; i++)
            {
                if (memberBrackets[i] is IStorage)
                    (memberBrackets[i] as IStorage).AddAllSymbolIn(symTable);
            }
        }

        public string GetTypeMember()
        {
            return typeName;
        }

        public void ViewStorage()
        {
            for(int i = 0; i < memberBrackets.Count; i++)
            {
                if (memberBrackets[i] is ISemantics) (memberBrackets[i] as ISemantics).GetTypeMember();
            }
        }

		public int GetSizeArray()
		{
			if (memberBrackets is IEject)
			{
				return Convert.ToInt32((memberBrackets as IEject).GetValue());
			}
			return 0;
		}
	}
}
