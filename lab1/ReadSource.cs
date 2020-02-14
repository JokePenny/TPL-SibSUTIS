using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace lab1
{
    class ReadSource
    {
        public static string ReadFile(string path)
        {
            FileStream fstream = File.OpenRead(Environment.CurrentDirectory + path);
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array);
        }
    }
}
