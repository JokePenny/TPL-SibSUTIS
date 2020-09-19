using System;
using System.Collections.Generic;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class NewAST : ASTNode, IStorage, ISemantics
    {
        private readonly ASTNode storageType;

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

        public string GetTypeMember()
        {
            string typeStorage = (storageType as ISemantics).GetTypeMember();
            return typeStorage;
        }

		public int GetSizeArray()
		{
			if(storageType is TypeAST)
			{
				return (storageType as TypeAST).GetSizeArray();
			}
			else if (storageType is BracketsAST)
			{
				return (storageType as BracketsAST).GetSizeArrayInt();
			}
			return 0;
		}
	}
}
