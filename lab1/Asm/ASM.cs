using lab1.ASTNodes;
using lab1.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace lab1.Asm
{
	class ASM
	{
		private static FileStream fstream;

		public static void WriteASMCode(string code)
		{
			ConsoleHelper.WriteDefault(code);
			byte[] array = System.Text.Encoding.Default.GetBytes("\n" + code);
			fstream.Write(array, 0, array.Length);
		}

		public static void CreateASM(ASTNode head = null)
		{
			if (head == null) SetHeadAST(ref head);

			if (head is NamespaceAST)
			{
				fstream = new FileStream(ReadSource.PathToSource + ".asm", FileMode.Create);
				(head as NamespaceAST).PrintASM("", true);
				fstream.Close();
			}
		}

		public static void RunCompileProgramm()
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = "cmd";
			psi.Arguments = @"/k "+ ReadSource.PathToFasm + " " + ReadSource.PathToSource + ".asm";
			Process.Start(psi);
		}

		private static void SetHeadAST(ref ASTNode head)
		{
			head = AbstractSyntaxTree.HeadAST;
			if (head == null)
			{
				ConsoleHelper.WriteError("AST not to build");
				return;
			}
		}
	}
}
