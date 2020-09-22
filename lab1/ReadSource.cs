using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace lab1
{
    class ReadSource
    {
        public static string PathToSource => pathToSource;
        public static string PathToFasm => pathToFasm;

        private static string pathToSource;
        private static string pathToFasm;

        public static string ReadFile(string path)
        {
            pathToSource = Environment.CurrentDirectory + path.Split('.')[0];

            FileStream fstream = File.OpenRead(Environment.CurrentDirectory + "\\" + path);
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array);
        }

		public static void SetPathToFasm(string pathToFasmCompiler)
		{
            pathToFasm = pathToFasmCompiler;
		}
	}
}
