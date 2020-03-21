
using System.Collections.Generic;

namespace lab1
{
    public static class Tokens
    {
        public static string Keyword;
        public static string Type;
        public const string NumReal = @"^[0-9]+\.[0-9]*$";
        public const string Num = @"^[0-9]+$";
        public const string Operator = @"^[+\-\*/.,=<>!&%]";
        public const string Twins = @"^[\[\](\)\{\}]";
        public const string ForbiddenSymbolsId = @"T:\\\\";
        public const string Id = @"^[A-Za-z][A-Za-z0-9]*";
        public const string String = @"^[""].*[""]";
        public const string Char = @"^['][A-Za-z0-9][']";
        public const char Semicolon = ';';
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>
        {
            {"if", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"else", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"for", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"while", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"do", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"foreach", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"break", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"continue", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"if", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"if", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"if", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
            {"if", "KEYWORD_IF"}, {"", "KEYWORD_FOR"}, {"", "KEYWORD_FOR"},
        };
    }
}
