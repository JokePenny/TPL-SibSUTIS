using System;
using System.Collections.Generic;
using System.Text;

namespace lab1
{
    public abstract class Dictionary
    {
        protected static string dictTokenKEYWORD;
        protected static string dictTokenTYPE;
        protected const string dictTokenNUM_REAL = @"^[0-9]+\.[0-9]*";
        protected const string dictTokenNUM = @"^[0-9]+";
        protected const string dictTokenOP = @"^[+\-\*/.=<>]";
        protected const string dictTokenTWINS = @"^[\[\](\)\{\}]";
        protected const string dictForbiddenSymbolsTokenID = @"T:\\\\";
        protected const string dictTokenID = @"^[A-Za-z][A-Za-z0-9]*";
        protected const string dictTokenSTRING = @"^[""].*[""]";
        protected const string dictTokenCHAR = @"^['][A-Za-z0-9][']";
        protected const char semicolon = ';';
    }
}
