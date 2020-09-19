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
        private readonly ASTNode storage;
		private readonly BracketsAST brackets;
        private readonly string nameID;

		//for ASM codegen
		private int startInStack;
		private string takeRegister = "";

		//for array
		private int lengthArray;
		private bool isArray;

		public bool IsArray => isArray;

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

		public IdentificatorAST(string type, string nameID, ASTNode storage, BracketsAST brackets, Point point, bool isArray)
		{
			this.nameID = nameID;
			this.type = type;
			this.storage = storage;
			this.brackets = brackets;
			this.point = point;
			this.isArray = isArray;
		}

		public IdentificatorAST(string nameID, Point point, BracketsAST brackets = null, bool isArray = false)
        {
            this.nameID = nameID;
            this.point = point;
			this.brackets = brackets;
			this.isArray = isArray;
		}

        public string GetTypeId()
        {
            return type;
        }

		public int GetAddresInStack()
		{
			return startInStack;
		}

		public string GetName()
        {
            return nameID;
        }

		public int GetOffseIfTtArray()
		{
			return (isArray && brackets != null ? brackets.GetSizeArrayInt() : 0) * ASMregisters.GetSizeStep(type);
		}

		public string GetCodegenASM()
		{
			IdentificatorAST identificatorFindInTable = (IdentificatorAST)SymTable.symTabls.FindNode(GetName());
			return ASMregisters.GetNameType(type) + " [ebp-" + identificatorFindInTable.GetAddresInStack() + "]";
		}

        public override void Print(string level)
        {
            if (type != "")
            {
                Console.WriteLine(level + "[TYPE] " + type);
                level = level + "\t";
            }
            Console.WriteLine(level + "[ID] " + nameID);

			if(brackets != null) brackets.Print(level);

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

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			if (storage == null && brackets == null) return;

			if (startInStack == 0)
			{
				IdentificatorAST identificatorFindInTable = (IdentificatorAST)SymTable.symTabls.FindNode(GetName());
				int offsetInStack = GetOffseIfTtArray();

				if(identificatorFindInTable != null && identificatorFindInTable != this)
				{
					startInStack = identificatorFindInTable.GetAddresInStack() + offsetInStack;
				}
				else
				{
					startInStack = ASMregisters.stepByte;
					ASMregisters.stepByte += ASMregisters.GetSizeStep(type);
				}
			}

			if (isArray)
			{
				if (storage is NewAST newASTArray)
				{
					int sizeArray = newASTArray.GetSizeArray();

					int locateStack = startInStack;
					ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + locateStack + "], 0");
					for (int i = 1; i < sizeArray; i++)
					{
						locateStack = startInStack + (ASMregisters.GetSizeStep(type) * i);
						ASMregisters.stepByte += ASMregisters.GetSizeStep(type);
						ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + locateStack + "], 0");
					}
				}
				else if (storage is BinaryExprAST || storage is ParenthesisExprAST)
				{
					storage.PrintASM(levelTabulatiion);
					PrintArrayIteration(levelTabulatiion);
				}
				else
				{
					if (isNewLine)
					{
						string elementStorage;
						if (storage is StringAST stringAST)
						{
							isArray = true;
							PrintSringArray(levelTabulatiion, stringAST.GetString(), startInStack);
						}
						else if (storage is IEject ejectedStorage)
						{
							elementStorage = ejectedStorage.GetValue();
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + elementStorage);
						}
						else
						{
							storage.PrintASM(levelTabulatiion);
							PrintArrayIteration(levelTabulatiion);
						}
					}
					else
					{
						takeRegister = ASMregisters.GetFreeRegisterData();
						ASM.WriteASMCode(levelTabulatiion + "mov\t" + takeRegister + ", " + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "]");
						ASM.WriteASMCode(levelTabulatiion + "push\t" + takeRegister);
						ASMregisters.SetStateRegisterData(takeRegister, true);
					}
				}
			}
			else
			{
				if (storage is NewAST)
				{
					ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], 0");
				}
				else if (storage is BinaryExprAST || storage is ParenthesisExprAST)
				{
					storage.PrintASM(levelTabulatiion);
					string register = ASMregisters.GetFreeRegisterData();
					ASM.WriteASMCode(levelTabulatiion + "pop\t" + register);
					ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + register);
				}
				else
				{
					if (isNewLine)
					{
						string elementStorage;
						if(storage is StringAST stringAST)
						{
							isArray = true;
							PrintSringArray(levelTabulatiion, stringAST.GetString(), startInStack);
						}
						else if (storage is IEject ejectedStorage)
						{
							elementStorage = ejectedStorage.GetValue();
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + elementStorage);
						}
						else
						{
							storage.PrintASM(levelTabulatiion);
							elementStorage = ASMregisters.GetFreeRegisterData();
							ASM.WriteASMCode(levelTabulatiion + "pop\t" + elementStorage);
							ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "], " + elementStorage);
							ASMregisters.SetStateRegisterData(elementStorage, true);
						}
					}
					else
					{
						takeRegister = ASMregisters.GetFreeRegisterData();
						ASM.WriteASMCode(levelTabulatiion + "mov\t" + takeRegister + ", " + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + "]");
						ASM.WriteASMCode(levelTabulatiion + "push\t" + takeRegister);
						ASMregisters.SetStateRegisterData(takeRegister, true);
					}
				}
			}
		}

		public void PrintArrayIteration(string levelTabulatiion, bool pushResult = false, bool popInArray = false)
		{
			string registerForOffset = ASMregisters.GetFreeRegisterData();
			string registerBuffer = ASMregisters.GetFreeRegisterData();
			string specialRegister = ASMregisters.GetFreeRegisterSpecial();

			ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerForOffset + ", 0");
			ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerBuffer + ", " + brackets.GetSizeArrayString());
			ASM.WriteASMCode(levelTabulatiion + "imul\t" + registerBuffer + ", " + ASMregisters.GetSizeStep(brackets.Type));
			ASM.WriteASMCode(levelTabulatiion + "sub\t" + registerForOffset + ", " + registerBuffer);
			ASM.WriteASMCode(levelTabulatiion + "mov\t" + specialRegister + ", " + registerForOffset);

			if (popInArray)
			{
				ASM.WriteASMCode(levelTabulatiion + "pop\t" + registerForOffset);
				ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + " + " + specialRegister + "], " + registerForOffset);
			}
			
			if(pushResult)
			{
				ASM.WriteASMCode(levelTabulatiion + "mov\t" + registerForOffset + ", " + ASMregisters.GetNameType(type) + " [ebp-" + startInStack + " + " + specialRegister + "]");
				ASM.WriteASMCode(levelTabulatiion + "push\t" + registerForOffset);
			}

			ASMregisters.SetStateRegisterData(registerBuffer, true);
			ASMregisters.SetStateRegisterData(registerForOffset, true);
			ASMregisters.SetStateRegisterSpecial(specialRegister, true);
		}

		private void PrintSringArray(string levelTabulatiion, string arrayString, int locateStack)
		{
			ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + locateStack + "], '" + arrayString[0] + "'");
			for (int i = 1; i < arrayString.Length; i++)
			{
				locateStack = startInStack + (ASMregisters.GetSizeStep(type) * i);
				ASMregisters.stepByte += ASMregisters.GetSizeStep(type);
				ASM.WriteASMCode(levelTabulatiion + "mov\t" + ASMregisters.GetNameType(type) + " [ebp-" + locateStack + "], '" + arrayString[i] + "'");
			}
		}
	}
}
