using lab1.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace lab1
{
	/// <summary>
	/// Глобальное хранилище ASM регистров для кодогенератора
	/// </summary>
	public static class ASMregisters
	{
		public static string MarkerJumpPrevBody { get; private set; }
		public static string MarkerJumpAfterBody { get; private set; }
		public static bool isContitionBelongsToCicle;

		private static int countMarkers = 1;
		private static string[] registersData = { "eax", "ebx", "ecx", "edx" };
		private static string[] registersSpecial = { "esi"};
		private static List<int> registersSpecialState = new List<int>{ 0 };
		private static List<int> registersDataState = new List<int> { 0, 0, 0, 0 };

		public static int stepByte { get; set; }

		/// <summary>
		/// Выдает первый свободный специальный регистр
		/// </summary>
		public static string GetFreeRegisterSpecial()
		{
			return GetFreeRegister(registersSpecial, registersSpecialState);
		}

		/// <summary>
		/// Выдает первый занятый специальный регистр
		/// </summary>
		public static string GetFirstFillRegisterSpecial()
		{
			return GetFirstFillRegister(registersSpecial, registersSpecialState);
		}

		/// <summary>
		/// Установить состояние для специального регистра занят/незанят
		/// </summary>
		public static void SetStateRegisterSpecial(string register, bool isFree)
		{
			SetStateRegister(registersSpecial, registersSpecialState, register, isFree);
		}

		/// <summary>
		/// Выдает первый свободный регистр
		/// </summary>
		public static string GetFreeRegisterData()
		{
			return GetFreeRegister(registersData, registersDataState);
		}

		/// <summary>
		/// Выдает первый занятый регистр
		/// </summary>
		public static string GetFirstFillRegisterData()
		{
			return GetFirstFillRegister(registersData, registersDataState);
		}

		/// <summary>
		/// Установить состояние для регистра занят/незанят
		/// </summary>
		public static void SetStateRegisterData(string register, bool isFree)
		{
			SetStateRegister(registersData, registersDataState, register, isFree);
		}

		private static string GetFirstFillRegister(string[] arrayRegisters, List<int> arrayStateRegisters)
		{
			for (int i = 0; i < arrayStateRegisters.Count; i++)
			{
				if (arrayStateRegisters[i] == 1)
				{
					arrayStateRegisters[i] = 0;
					return arrayRegisters[i];
				}
			}
			return arrayRegisters[0];
		}

		private static void SetStateRegister(string[] arrayRegisters, List<int> arrayStateRegisters, string register, bool isFree)
		{
			for (int i = 0; i < arrayStateRegisters.Count; i++)
			{
				if (arrayRegisters[i] == register)
				{
					arrayStateRegisters[i] = isFree ? 0 : 1;
					return;
				}
			}
		}

		private static string GetFreeRegister(string[] arrayRegisters, List<int> arrayStateRegisters)
		{
			for (int i = 0; i < arrayStateRegisters.Count; i++)
			{
				if (arrayStateRegisters[i] == 0)
				{
					arrayStateRegisters[i] = 1;
					return arrayRegisters[i];
				}
			}
			return arrayRegisters[0];
		}

		/// <summary>
		/// Размер сдвига для переменных, для размещения на стеке
		/// </summary>
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
					//return 8;
				case "char":
					return 2;
				case "bool":
					return 1;
				default:
					return 4;
			}
		}

		/// <summary>
		/// Очищает занятые маркеры под условные переходы
		/// </summary>
		public static void ClearMarkerks()
		{
			MarkerJumpPrevBody = "";
			MarkerJumpAfterBody = "";
		}

		/// <summary>
		/// Очищает занятые маркеры под условные переходы
		/// </summary>
		public static void ClearMarkerPrevBody()
		{
			MarkerJumpPrevBody = "";
		}

		/// <summary>
		/// Возвращает код для типа переменной
		/// </summary>
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
					//return "QWORD";
				case "char":
				case "bool":
					return "BYTE";
				default:
					return "DWORD";
			}
		}

		/// <summary>
		/// Возвращает код арифметической операции
		/// </summary>
		public static string GetOperation(string op)
		{
			switch (op)
			{
				case "+":
					return "add";
				case "-":
					return "sub";
				case "*":
					return "imul";
				case "/":
					return "div";
				default:
					return "";
			}
		}

		/// <summary>
		/// Возвращает код инкремента/декремента
		/// </summary>
		public static string GetCrement(string crement)
		{
			switch (crement)
			{
				case "++":
					return "add";
				default:
					return "sub";
			}
		}

		/// <summary>
		/// Новую метку для условного перехода к начлу тела
		/// </summary>
		public static string GetNewMarkerJumpPrevBody()
		{
			if (MarkerJumpPrevBody == null || MarkerJumpPrevBody == "") MarkerJumpPrevBody = GetNewNameMarker();
			return MarkerJumpPrevBody;
		}

		/// <summary>
		/// Новую метку для условного перехода к концу тела
		/// </summary>
		public static string GetNewMarkerJumpAfterBody()
		{
			if (MarkerJumpAfterBody == null || MarkerJumpAfterBody == "") MarkerJumpAfterBody = GetNewNameMarker();
			return MarkerJumpAfterBody;
		}

		/// <summary>
		/// Создет новое имя для маркера условного перехода
		/// </summary>
		private static string GetNewNameMarker()
		{
			string newMarker = ".L" + countMarkers;
			countMarkers++;
			return newMarker;
		}

		/// <summary>
		/// Возвращает код условного перехода
		/// </summary>
		public static string GetTypeConditionJump(string boolOp, bool isRevert)
		{
			string typeJump = "";
			switch(boolOp)
			{
				case "==":
					typeJump = isRevert ? "jne" : "je";
					break;
				case "!=":
					typeJump = isRevert ? "je" : "jne";
					break;
				case "<=":
					typeJump = isRevert ? "ja" : "jle";
					break;
				case ">=":
					typeJump = isRevert ? "jb" : "jge";
					break;
				case "<":
					typeJump = isRevert ? "jge" : "jb";
					break;
				case ">":
					typeJump = isRevert ? "jle" : "ja";
					break;
				default:
					ConsoleHelper.WriteError("Not find asm-code for this bool operation: " + boolOp);
					break;
			}
			return typeJump;
		}
	}
}
