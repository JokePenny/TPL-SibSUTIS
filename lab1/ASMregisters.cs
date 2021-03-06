﻿using lab1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public static string ShowValue => showValue;
		public static string ShowString => showString;
		public static string SpaceString => spaceString;
		public static string NewString => newString;
		public static string BufferString => buuferString;
		public static int stepByte { get; set; }

		public static bool isContitionBelongsToCicle;

		private static int countMarkers = 1;
		private static string showValue = "showValue";
		private static string showString = "showString";
		private static string spaceString = "spaceString";
		private static string newString = "newString";
		private static string buuferString = "bufferString";
		private static string[] registersData = { "eax", "ebx", "ecx", "edx" };
		private static string[] registersSpecial = { "esi"};
		private static List<int> registersSpecialState = new List<int>{ 0 };
		private static List<int> registersDataState = new List<int> { 0, 0, 0, 0 };

		public enum Register
		{
			SPECIAL,
			DATA
		}

		/// <summary>
		/// Установить состояние для специального регистра занят/незанят
		/// </summary>
		public static void SetStateRegister(Register typeRegister, string register, bool isFree)
		{
			switch (typeRegister)
			{
				case Register.SPECIAL: SetStateRegister(registersSpecial, registersSpecialState, register, isFree);
					break;
				case Register.DATA: SetStateRegister(registersData, registersDataState, register, isFree);
					break;
			}
		}

		/// <summary>
		/// Выдает первый свободный регистр
		/// </summary>
		public static string GetFreeRegister(Register typeRegister, string forceGetRegister = "")
		{
			switch (typeRegister)
			{
				case Register.SPECIAL: return GetFreeRegister(registersSpecial, registersSpecialState);
				case Register.DATA: return GetFreeRegister(registersData, registersDataState);
			}
			return "";
		}

		/// <summary>
		/// Выдает первый занятый регистр
		/// </summary>
		public static string GetFirstFillRegister(Register typeRegister)
		{
			switch(typeRegister)
			{
				case Register.SPECIAL: return GetFirstFillRegister(registersSpecial, registersSpecialState);
				case Register.DATA: return GetFirstFillRegister(registersData, registersDataState);
			}
			return "";
		}

		/// <summary>
		/// Есть ли заполненные регистры
		/// </summary>
		/// 
		public static bool HasFillRegisters(Register typeRegister)
		{
			switch (typeRegister)
			{
				case Register.SPECIAL: return HasFillRegisters(registersSpecial, registersSpecialState);
				case Register.DATA: return HasFillRegisters(registersData, registersDataState);
			}
			return false;
		}

		public static bool HasFillRegisters(string[] arrayRegisters, List<int> arrayStateRegisters)
		{
			for (int i = 0; i < arrayStateRegisters.Count; i++)
			{
				if (arrayStateRegisters[i] == 1)
				{
					arrayStateRegisters[i] = 0;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Возвращает первый заполненный регистр
		/// </summary>
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

		public static void SwapJumpMarkers()
		{
			string tmp = MarkerJumpPrevBody;
			MarkerJumpPrevBody = MarkerJumpAfterBody;
			MarkerJumpAfterBody = tmp;
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

		private static string GetFreeRegister(string[] arrayRegisters, List<int> arrayStateRegisters, string forceGetRegister = "")
		{
			bool isForceGet = forceGetRegister != "";
			for (int i = 0; i < arrayStateRegisters.Count; i++)
			{
				if (arrayStateRegisters[i] == 0 && !isForceGet)
				{
					arrayStateRegisters[i] = 1;
					return arrayRegisters[i];
				}
				else if(isForceGet && arrayRegisters[i] == forceGetRegister)
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
					return 4;
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
					//return "BYTE";
					return "DWORD";
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
				default:
					ConsoleHelper.WriteError("Its operation not work");
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

		public static void SetMarkersJump(string markerPrevBpdy, string markerAfterBody)
		{
			MarkerJumpPrevBody = markerPrevBpdy;
			MarkerJumpAfterBody = markerAfterBody;
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
			string typeJump = "jmp";
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
