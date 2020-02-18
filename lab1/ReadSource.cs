using System;
using System.IO;
using System.Text;

namespace lab1
{
    public sealed class ReadSource : Dictionary
    {
        private const string pathDictTokenKEYWORD = @"\dictionary\keyword.txt";
        private const string pathDictTokenTYPE = @"\dictionary\type.txt";

        public static string ReadFile(string path)
        {
            FileStream fstream = File.OpenRead(Environment.CurrentDirectory + path);
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array);
        }

        public static void FillDictionary()
        {
            dictTokenKEYWORD = GetString(pathDictTokenKEYWORD);
            dictTokenTYPE = GetString(pathDictTokenTYPE);
        }

        private static string GetString(string path)
        {
            return ReadFile(path).Replace("\r\n", "");
        }
    }
}
