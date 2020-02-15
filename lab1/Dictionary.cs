using System;
using System.Collections.Generic;
using System.Text;

namespace lab1
{
    abstract class Dictionary
    {
        protected static string dictTokenKEYWORD;
        protected static string dictTokenTYPE;
        protected readonly static string dictTokenNUM_REAL = @"^[0-9]+\.[0-9]*";
        protected readonly static string dictTokenNUM = @"^[0-9]+";
        protected readonly static string dictTokenOP = @"^[+\-\*/.=<>]";
        protected readonly static string dictTokenTWINS = @"^[\[\](\)\{\}]";
        protected readonly static string dictForbiddenSymbolsTokenID = @"T:\\\\";
        protected readonly static string dictTokenID = @"^[A-Za-z][A-Za-z0-9]*";
        protected readonly static string dictTokenCHAR = @"^['][A-Za-z0-9][']";
        protected readonly static char semicolon = ';';
    }
}
