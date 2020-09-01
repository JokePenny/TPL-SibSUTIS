using lab1.ASTNodes;
using lab1.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace lab1.Asm
{
	class ASM
	{
		private static FileStream fstream;
		private static string path;

		public static void CreateASM(ASTNode head)
		{
			if (head is NamespaceAST)
			{
				fstream = new FileStream(Environment.CurrentDirectory + "\\note.cs", FileMode.Truncate);
				(head as NamespaceAST).PrintASM("", true);
			}
			fstream.Close();
		}

		public static void WriteASMCode(string code)
		{
			ConsoleHelper.WriteDefault(code);
			byte[] array = System.Text.Encoding.Default.GetBytes("\n" + code);
			fstream.Write(array, 0, array.Length);
		}
	}
}
