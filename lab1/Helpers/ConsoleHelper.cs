using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.Helpers
{
    public static class ConsoleHelper
    {
        private const ConsoleColor Header = ConsoleColor.Yellow;
        private const ConsoleColor Error = ConsoleColor.Red;
        private const ConsoleColor Success = ConsoleColor.Green;

        public static void WriteHeader(string message)
        {
            Console.ForegroundColor = Header;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = Error;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteErrorAST(string message, int y, int x)
        {
            Console.ForegroundColor = Error;
            Console.WriteLine("Error <" + y + ", " + x + ">: " + message);
            Console.ResetColor();
        }

        public static void WriteSuccess(string message)
        {
            Console.ForegroundColor = Success;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteDefault(string message)
        {
            Console.WriteLine(message);
        }

        public static void WriteSeparator()
        {
            Console.WriteLine("-----------------------------------------");
        }
    }
}
