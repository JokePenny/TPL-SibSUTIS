using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class IdentificatorAST : ASTNode, IStorage, ISemantics
    {
        private string type = "";
        private ASTNode storage;
        private string nameID;

		//ASM
		private bool isArray;
		private int startInStack;
		private string takeRegister = "";


        public IdentificatorAST(string type, string nameID, Point point, bool isArray)
        {
            this.nameID = nameID;
            this.type = type;
            this.point = point;
			this.isArray = isArray;

		}

        public IdentificatorAST(string type, string nameID, ASTNode storage, Point point, bool isArray)
        {
            this.nameID = nameID;
            this.type = type;
            this.storage = storage;
            this.point = point;
			this.isArray = isArray;
		}

        public IdentificatorAST(string nameID, ASTNode storage, Point point)
        {
            this.nameID = nameID;
            this.storage = storage;
            this.point = point;
        }

        public IdentificatorAST(string nameID, Point point)
        {
            this.nameID = nameID;
            this.point = point;
        }

        public string GetTypeId()
        {
            return type;
        }

		public int GetAddresInStack()
		{
			return startInStack;
		}

		public ASTNode GetStorage()
        {
            return storage;
        }

        public string GetName()
        {
            return nameID;
        }

        public override void Print(string level)
        {
            if (type != "")
            {
                Console.WriteLine(level + "[TYPE] " + type);
                level = level + "\t";
            }
            Console.WriteLine(level + "[ID] " + nameID);
            if (storage != null)
            {
                Console.WriteLine(level + "[STORAGE] =");
                storage.Print(level + "\t");
            }
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (symTable.ContainsKey(nameID))
            {
                if (type != "") ConsoleHelper.WriteError(nameID + " - Variable is redeclared");
                type = (symTable[nameID] as IdentificatorAST).GetTypeId();
            }
            else symTable.Add(nameID, this);

            if (storage is IStorage)
                (storage as IStorage).AddAllSymbolIn(symTable);
        }

        public string GetTypeMember()
        {
            if(storage is ISemantics && !(storage is BracketsAST))
            {
                string typeStorage = (storage as ISemantics).GetTypeMember();
                if (typeStorage != type) ConsoleHelper.WriteError("<" + point.y + "," + point.x + ">: Wrong type storage in '" + type + "' " + nameID + " = '" + typeStorage + "'");
            }
            return type;
        }

		public override void PrintASM(bool isNewLine)
		{
			if (storage == null) return;

			startInStack = ASMregisters.stepByte;
			int step = ASMregisters.GetSizeStep(type);
			ASMregisters.stepByte += step;
			startInStack = ASMregisters.stepByte;
			if (isArray)
			{
				if (storage != null)
				{
					if (storage is NewAST)
					{
						ConsoleHelper.WriteDefault("\t\tmov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], ");
					}
					else
					{
						if(isNewLine)
						{

						}
						else
						{

						}
					}
				}
			}
			else
			{
				if(storage != null)
				{
					if (storage is NewAST)
					{
						ConsoleHelper.WriteDefault("\t\tmov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], 0");
					}
					else if(storage is BinaryExprAST || storage is ParenthesisExprAST)
					{
						storage.PrintASM();
						string register = ASMregisters.GetFreeRegisterData();
						ConsoleHelper.WriteDefault("\t\tpop\t" + register);
						ConsoleHelper.WriteDefault("\t\tmov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + register);
					}
					else
					{
						if (isNewLine)
						{
							string elementStorage;
							if (storage is IEject)
								elementStorage = (storage as IEject).GetValue();
							else
							{
								storage.PrintASM();
								elementStorage = ASMregisters.GetFreeRegisterData();
							}

							ConsoleHelper.WriteDefault("\t\tmov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + elementStorage);
						}
						else
						{
							takeRegister = ASMregisters.GetFreeRegisterData();
							ConsoleHelper.WriteDefault("\t\tmov\t" + takeRegister + ", " + ASMregisters.GetNameType(type) + " [ebp-" + (startInStack + step) + "]");
						}
					}
					ASMregisters.stepByte += step;
				}
			}
		}

		public void FreeRegister()
		{
			if(takeRegister != "") ASMregisters.SetStateRegisterData(takeRegister, true);
		}
	}
}
