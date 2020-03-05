using System;
using System.IO;
using System.Text;

namespace lab1
{
    public sealed class ReadSource
    {
        private const string PathTokenKeyword = @"\dictionary\keyword.txt";
        private const string PathTokenType = @"\dictionary\type.txt";

        public static string ReadFile(string path)
        {
            FileStream fstream = File.OpenRead(Environment.CurrentDirectory + path);
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array);
        }

        public static void FillTokens()
        {
            Tokens.Keyword = GetString(PathTokenKeyword);
            Tokens.Type = GetString(PathTokenType);
        }

        private static string GetString(string path)
        {
            return ReadFile(path).Replace("\r\n", "");
        }
    }
}
