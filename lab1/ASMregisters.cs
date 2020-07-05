using System;
using System.Collections.Generic;
using System.Text;

namespace lab1
{
	public static class ASMregisters
	{
		public static string[] registersData = { "eax", "ebx", "ecx", "edx" };
		public static int[] registersDataState = { 0, 0, 0, 0 };

		public static int stepByte { get; set; }

		public static string GetFreeRegisterData()
		{
			for(int i = 0; i < registersDataState.Length; i++)
			{
				if (registersDataState[i] == 0)
				{
					registersDataState[i] = 1;
					return registersData[i];
				}
			}
			return registersData[0];
		}

		public static string GetFirstFillRegister()
		{
			for (int i = 0; i < registersDataState.Length; i++)
			{
				if (registersDataState[i] == 1)
				{
					registersDataState[i] = 0;
					return registersData[i];
				}
			}
			return registersData[0];
		}

		public static void SetStateRegisterData(string register, bool isFree)
		{
			for (int i = 0; i < registersDataState.Length; i++)
			{
				if (registersData[i] == register)
				{
					registersDataState[i] = isFree ? 0 : 1;
					return;
				}
			}
		}

		public static int GetSizeStep(string type)
		{
			switch(type)
			{
				case "int":
					return 4;
				case "double":
					return 8;
				case "float":
					return 8;
				case "string":
					return 8;
				case "char":
					return 2;
				case "bool":
					return 1;
				default:
					return 4;
			}
		}

		public static string GetNameType(string type)
		{
			switch (type)
			{
				case "int":
					return "DWORD";
				case "double":
					return "QWORD";
				case "float":
					return "DWORD";
				case "string":
					return "QWORD";
				case "char":
				case "bool":
					return "BYTE";
				default:
					return "DWORD";
			}
		}

		public static string GetOperation(string op)
		{
			switch (op)
			{
				case "+":
					return "add";
				case "-":
					return "sub";
				case "*":
					return "mul";
				case "/":
					return "dib";
				default:
					return "";
			}
		}

		public static string GetCrement(string crement)
		{
			switch (crement)
			{
				case "++":
					return "inc";
				default:
					return "dec";
			}
		}
	}
}
