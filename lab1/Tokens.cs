
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
    }
}
