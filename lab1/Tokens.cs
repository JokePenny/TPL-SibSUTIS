﻿
using System;
using System.Collections.Generic;

namespace lab1
{
    public static class Tokens
    {
        public const string DoubleVal = @"^[0-9]+\.[0-9]*$";
        public const string IntVal = @"^[0-9]+$";
        public const string Id = @"^[A-Za-z][A-Za-z0-9]*$";
        public const string String = @"^[""].*[""]$";
        public const string Char = @"^['].[']$";
        public const string X16 = @"^0x[A-Fa-f0-9]+$";
        public const string X8 = @"^0[0-7]+$";
        public const string X2 = @"^0b[0-1]+$";

        public enum Token : int
        {
            K_FOR,
            K_IF,
            K_CLASS,
            K_NAMESPACE,
            K_NEW,
            K_ELSE,
            K_ELSE_IF,
            K_BREAK,
            K_CONTINUE,
            K_WHILE,
            K_RETURN,
            K_NULL,
            COMM,
            BRACE_L,
            BRACE_R,
            PARENTHESIS_L,
            PARENTHESIS_R,
            BRACKET_L,
            BRACKET_R,
            CREMENT,
            ASSIGNMENT,
            ASSIGNMENT_PLUS,
            ASSIGNMENT_MINUS,
            ASSIGNMENT_MULTY,
            ASSIGNMENT_DIVISION,
            TYPE,
            BOOL,
            OP,
            BOOL_OP,
            BOOL_OP_AND,
            BOOL_OP_OR,
            BIT_OP_AND,
            BIT_OP_OR,
            INT_VALUE,
            DOUBLE_VALUE,
            X16_VALUE,
            X8_VALUE,
            X2_VALUE,
            ID,
            SEMILICON,
            STRING,
            CHAR,
			DOUBLE_DOTS,
			DOT,
			CONSOLE,
			CONSOLE_WRITE,
			CONSOLE_WRITELINE,
			EMPTY,
            FAILED
        }

        public static Dictionary<string, Token> DictionaryWord = new Dictionary<string, Token>
        {
            {"if", Token.K_IF},
            {"else", Token.K_ELSE},
            {"for", Token.K_FOR},
            {"break", Token.K_BREAK},
            {"continue", Token.K_CONTINUE},
            {"while", Token.K_WHILE},
            {"new", Token.K_NEW},
            {"int", Token.TYPE},
            {"double", Token.TYPE},
            {"void", Token.TYPE},
            {"string", Token.TYPE},
            {"bool", Token.TYPE},
            {"true", Token.BOOL},
            {"false", Token.BOOL},
            {"return", Token.K_RETURN},
            {"null", Token.K_NULL},
            {"namespace", Token.K_NAMESPACE},
            {"class", Token.K_CLASS},
            {"!", Token.BOOL_OP},
            {"<", Token.BOOL_OP},
            {"<=", Token.BOOL_OP},
            {">", Token.BOOL_OP},
            {">=", Token.BOOL_OP},
            {"==", Token.BOOL_OP},
            {"!=", Token.BOOL_OP},
            {"&&", Token.BOOL_OP_AND},
            {"||", Token.BOOL_OP_OR},
            {"&", Token.BIT_OP_AND},
            {"|", Token.BIT_OP_OR},
            {"+", Token.OP},
            {"%", Token.OP},
            {"-", Token.OP},
            {"/", Token.OP},
            {"*", Token.OP},
            {"++", Token.CREMENT},
            {"--", Token.CREMENT},
            {"=", Token.ASSIGNMENT},
            {"+=", Token.ASSIGNMENT},
            {"-=", Token.ASSIGNMENT},
            {"/=", Token.ASSIGNMENT},
            {"*=", Token.ASSIGNMENT},
            {"(", Token.PARENTHESIS_L},
            {")", Token.PARENTHESIS_R},
            {"[", Token.BRACKET_L},
            {"]", Token.BRACKET_R},
            {"{", Token.BRACE_L},
            {"}", Token.BRACE_R},
            {".", Token.DOT},
            {",", Token.COMM},
            {";", Token.SEMILICON},
            {":", Token.DOUBLE_DOTS},
			{"Console", Token.CONSOLE},
			{"Write", Token.CONSOLE_WRITE},
			{"WriteLine", Token.CONSOLE_WRITELINE},
		};

        public static bool IsErrorToken(ref Token tokenType, Token tokenTypeMemory, int length)
        {
            bool isOk = false;
            switch (tokenType)
            {
                case Token.K_IF:
                case Token.K_FOR:
                case Token.K_ELSE:
                case Token.K_RETURN:
                case Token.K_NAMESPACE:
                case Token.K_NULL:
                case Token.K_WHILE:
                case Token.K_BREAK:
                case Token.K_CONTINUE:
                case Token.K_CLASS:
                case Token.K_NEW:
                case Token.BOOL:
                case Token.ID:
                case Token.TYPE:
				case Token.CONSOLE:
				case Token.CONSOLE_WRITE:
				case Token.CONSOLE_WRITELINE:
					isOk =  tokenTypeMemory == Token.K_IF ||
                            tokenTypeMemory == Token.K_WHILE ||
                            tokenTypeMemory == Token.K_FOR ||
                            tokenTypeMemory == Token.K_ELSE ||
                            tokenTypeMemory == Token.K_RETURN ||
                            tokenTypeMemory == Token.K_NAMESPACE ||
                            tokenTypeMemory == Token.K_NULL ||
                            tokenTypeMemory == Token.K_CLASS ||
                            tokenTypeMemory == Token.K_NEW ||
                            tokenTypeMemory == Token.K_BREAK ||
                            tokenTypeMemory == Token.ID ||
                            tokenTypeMemory == Token.TYPE ||
							tokenTypeMemory == Token.CONSOLE ||
							tokenTypeMemory == Token.CONSOLE_WRITE ||
							tokenTypeMemory == Token.CONSOLE_WRITELINE ||
							tokenTypeMemory == Token.EMPTY ||
                            tokenTypeMemory == Token.FAILED;
                    break;
                case Token.OP:
                case Token.BOOL_OP:
                case Token.BOOL_OP_AND:
                case Token.BOOL_OP_OR:
                case Token.BIT_OP_AND:
                case Token.BIT_OP_OR:
                case Token.CREMENT:
                case Token.ASSIGNMENT:
                    isOk = (tokenTypeMemory == Token.OP ||
                            tokenTypeMemory == Token.BOOL_OP ||
                            tokenTypeMemory == Token.BIT_OP_OR ||
                            tokenTypeMemory == Token.BIT_OP_AND ||
                            tokenTypeMemory == Token.CREMENT ||
                            tokenTypeMemory == Token.ASSIGNMENT ||
                            tokenTypeMemory == Token.EMPTY ||
                            tokenTypeMemory == Token.FAILED) && length < 3;
                    break;
                case Token.SEMILICON:
                case Token.PARENTHESIS_L:
                case Token.PARENTHESIS_R:
                case Token.BRACE_L:
                case Token.BRACE_R:
                case Token.BRACKET_L:
                case Token.BRACKET_R:
                case Token.COMM:
                case Token.DOT:
                case Token.DOUBLE_DOTS:
                    return true;
                case Token.INT_VALUE:
                case Token.DOUBLE_VALUE:
                case Token.X16_VALUE:
                case Token.X8_VALUE:
                case Token.X2_VALUE:
                case Token.STRING:
                    return true;
                case Token.FAILED:
                    return false;
            }
            tokenType = isOk ? tokenType : Token.FAILED;
            return isOk;
        }
    }
}
