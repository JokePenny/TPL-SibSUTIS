﻿using lab1.ASTNodes;
using lab1.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace lab1
{
    class AbstractSyntaxTree
    {
        private static Hashtable hashtable = new Hashtable();
        private static TokenNode curTok;
        private static TokenNode bufferTok;
        private static ASTNode headAST;
        private static bool isCycleArea = false;

        public static void CreateAST()
        {
            GetNextToken();
            if (curTok == null) return;
            if (curTok.token != Tokens.Token.K_NAMESPACE)
            {
                ConsoleHelper.WriteErrorAST("Expected 'namespace'", curTok.y, curTok.x);
                SkipToToken(Tokens.Token.K_NAMESPACE);
            }

            headAST = ParseMainArea(Area.NAMESPACE);
            headAST.Print("");
        }

        //---------------------
        // Парсер скобки [
        //---------------------

        private static ASTNode ParseBrackets(bool isCanValues)
        {
            ASTNode brackets = null;
            if (isCanValues)
            {
                GetNextToken();
                if (curTok == null) return null;

                if (curTok.token == Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected '[X]'", curTok.y, curTok.x);

                ASTNode exp = MemberBinOperation();
                if (curTok == null) return null;

                if(curTok.token == Tokens.Token.BRACKET_L)
                {
                    exp = new BracketsAST(exp, ParseBrackets(true));
                    if (curTok == null) return null;
                }

                brackets = exp;
                if (curTok.token == Tokens.Token.OP)
                {
                    brackets = ParseBinaryOperation(exp);
                    if (curTok == null) return null;
                }
                else if (curTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", curTok.y, curTok.x);
            }
            else
            {
                GetNextToken();
                if (curTok == null) return null;
                if (curTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", curTok.y, curTok.x);
                brackets = new BracketsAST();
            }
            GetNextToken();
            return brackets;
        }

        //---------------------
        // Парсер скобки {
        //---------------------

        private enum Area : int
        {
            NAMESPACE = 0,
            CLASS,
            METHOD
        }

        private static List<ASTNode> ParseBrace(Area area, bool isCycle)
        {
            isCycleArea = isCycle;
            List<ASTNode> membersBodyMethod = new List<ASTNode>();
            while (true)
            {
                ASTNode member = GetMemberArea(area, isCycle);
                if (member != null) membersBodyMethod.Add(member);
                if (curTok == null) break;
                if (curTok.token == Tokens.Token.BRACE_R) break;
            }
            GetNextToken();
            return membersBodyMethod;
        }

        private static ASTNode GetMemberArea(Area area, bool isCycle)
        {
            switch (area)
            {
                case Area.NAMESPACE:
                    return MemberNamespace();
                case Area.CLASS:
                    return MemberClass();
                case Area.METHOD:
                    return MemberMethod(isCycle);
            }
            return null;
        }
        private static ASTNode MemberMethod(bool isCycle)
        {
            GetNextToken();
            if (curTok == null) return null;
            switch (curTok.token)
            {
                case Tokens.Token.SEMILICON:
                    break;
                case Tokens.Token.TYPE:
                    return ParseType(false, false, Tokens.Token.SEMILICON);
                case Tokens.Token.ID:
                    return ParseId(Tokens.Token.SEMILICON);
                case Tokens.Token.CREMENT:
                    return ParseCrement(Tokens.Token.SEMILICON);
                case Tokens.Token.K_FOR:
                    return ParseFor();
                case Tokens.Token.K_IF:
                    bufferTok = curTok;
                    return ParseIf();
                case Tokens.Token.K_ELSE:
                    if(bufferTok.token != Tokens.Token.K_IF) ConsoleHelper.WriteErrorAST("No external loop to interrupt or continue", curTok.y, curTok.x);
                    return ParseIf();
                case Tokens.Token.K_WHILE:
                    return ParseWhile();
                case Tokens.Token.K_CONTINUE:
                case Tokens.Token.K_BREAK:
                    if (!isCycle) ConsoleHelper.WriteErrorAST("No external loop to interrupt or continue", curTok.y, curTok.x);
                    ASTNode node;
                    if (curTok.token == Tokens.Token.K_BREAK) node = new BreakAST();
                    else node = new ContinueAST();

                    GetNextToken();
                    if (curTok == null) return node;
                    if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                    return node;
                case Tokens.Token.K_RETURN:
                    return ParseReturn();
                case Tokens.Token.BRACE_R:
                    return null;
                default:
                    return null;
            }
            return null;
        }

        private static ASTNode MemberNamespace()
        {
            GetNextToken();
            if (curTok == null) return null;
            switch (curTok.token)
            {
                case Tokens.Token.BRACE_R:
                    return null;
                case Tokens.Token.K_CLASS:
                    return ParseMainArea(Area.CLASS);
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", curTok.y, curTok.x);
                    return null;
            }
        }

        private static ASTNode MemberClass()
        {
            GetNextToken();
            if (curTok == null) return null;
            switch (curTok.token)
            {
                case Tokens.Token.TYPE:
                    return ParseType(false, true, Tokens.Token.SEMILICON);
                case Tokens.Token.BRACE_R:
                    return null;
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", curTok.y, curTok.x);
                    return null;
            }
        }

        //---------------------
        // Парсер скобки (
        //---------------------

        private static ASTNode ParseParenthesis()
        {
            ASTNode exprassion;
            GetNextToken();
            if (curTok == null) return null;
            if (curTok.token != Tokens.Token.SEMILICON && curTok.token != Tokens.Token.PARENTHESIS_R)
            {
                ASTNode leftNode = MemberBinOperation();
                if (leftNode == null) return null;
                if (curTok == null) return null;
                if (curTok.token == Tokens.Token.OP)
                {
                    exprassion = ParseBinaryOperation(leftNode);
                    return new ParenthesisExprAST(exprassion);
                }
                else if (curTok.token == Tokens.Token.PARENTHESIS_L)
                {
                    ASTNode parenExpr = ParseParenthesis();
                    if (parenExpr == null) return null;
                    exprassion = ParseBinaryOperation(parenExpr);
                    return new ParenthesisExprAST(exprassion);
                }
                else if (curTok.token != Tokens.Token.SEMILICON) return null;
            }
            else
            {
                ConsoleHelper.WriteErrorAST("Expected ')' or empty ()", curTok.y, curTok.x);
            }
            return null;
        }

        //---------------------
        // Парсер области имен и класса
        //---------------------

        private static ASTNode ParseMainArea(Area area)
        {
            string idName;
            GetNextToken();
            if (curTok == null) return null;
            if (curTok.token != Tokens.Token.ID)
            {
                idName = "[NONAME]";
                ConsoleHelper.WriteErrorAST("Expected 'identificator'", curTok.y, curTok.x);
                SkipToToken(Tokens.Token.BRACE_L);
            }
            else
            {
                idName = curTok.subString;
                GetNextToken();
            }

            if (curTok == null) return null;
            if (curTok.token != Tokens.Token.BRACE_L) ConsoleHelper.WriteErrorAST("Expected '{'", curTok.y, curTok.x);

            List<ASTNode> memberArea = ParseBrace(area, false);

            if (curTok == null) return null;
            if (curTok.token != Tokens.Token.BRACE_R) ConsoleHelper.WriteErrorAST("Expected '}'", curTok.y, curTok.x);

            if(area == Area.NAMESPACE) return new NamespaceAST(memberArea, idName);
            else return new ClassAST(memberArea, idName);
        }

        //---------------------
        // Парсер метода
        //---------------------
        private static ASTNode ParseArgInMethod(bool isCall)
        {
            if (curTok.token == Tokens.Token.ID && isCall)
            {
                IdentificatorAST identificatorAST = new IdentificatorAST(curTok.subString);
                return identificatorAST;
            }
            else if (curTok.token == Tokens.Token.TYPE && !isCall)
            {
                return ParseType(false, false, Tokens.Token.PARENTHESIS_R);
            }
            else if (curTok.token == Tokens.Token.PARENTHESIS_R)
            {
                return null;
            }
            else ConsoleHelper.WriteErrorAST("Expected 'type' for identificator in method declaration", curTok.y, curTok.x);
            return null;
        }

        private static List<ASTNode> ParseArgsMethod(bool isCall)
        {
            List<ASTNode> argsMethod = new List<ASTNode>();

            GetNextToken();
            if (curTok == null) return null;

            while (true)
            {
                if (argsMethod.Count >= 1)
                {
                    if (curTok.token == Tokens.Token.COMM)
                    {
                        ASTNode argMethod = ParseArgInMethod(isCall);
                        if (curTok == null) break;
                        if (argMethod != null) argsMethod.Add(argMethod);
                        if (curTok.token == Tokens.Token.COMM) continue;
                        if (curTok.token != Tokens.Token.PARENTHESIS_R) break;
                    }
                    break;
                }
                else
                {
                    ASTNode argMethod = ParseArgInMethod(isCall);
                    argsMethod.Add(argMethod);
                    break;
                }
            }
            if (curTok.token != Tokens.Token.PARENTHESIS_R)
            {
                ConsoleHelper.WriteErrorAST("Expected ')'", curTok.y, curTok.x);
                SkipToToken(Tokens.Token.BRACE_L);
            }
            GetNextToken();
            return argsMethod;
        }

        private static ASTNode ParseMethod(string typeId, string idName, bool isCall)
        {
            List<ASTNode> argsMethod = ParseArgsMethod(isCall);

            List<ASTNode> bodyMethod = new List<ASTNode>();
            if (!isCall)
            {
                if (curTok == null) return null;
                if (curTok.token != Tokens.Token.BRACE_L) ConsoleHelper.WriteErrorAST("Expected '{'", curTok.y, curTok.x);
                bodyMethod = ParseBrace(Area.METHOD, false);
            }

            if (argsMethod.Count > 0)
            {
                if (isCall) return new MethodAST(idName, argsMethod);
                else return new MethodAST(typeId, idName, argsMethod, new BodyMethodAST(bodyMethod));
            }
            else
            {
                if (isCall) return new MethodAST(idName);
                else return new MethodAST(typeId, idName, new BodyMethodAST(bodyMethod));
            }
        }

        //---------------------
        // Парсер while
        //---------------------

        private static ASTNode ParseWhile()
        {
            ASTNode condition = null;
            ASTNode bodyWhile;
            GetNextToken();
            if (curTok == null) return null;
            if (curTok.token != Tokens.Token.PARENTHESIS_L)
            {
                ConsoleHelper.WriteErrorAST("Expected '('", curTok.y, curTok.x);
                SkipToToken(Tokens.Token.BRACE_L);
            }
            else
            {
                condition = ParseConditionIf();
                GetNextToken();
            }

            if (curTok == null) return null;
            if (curTok.token == Tokens.Token.BRACE_L)
            {
                bodyWhile = new BodyMethodAST(ParseBrace(Area.METHOD, true));
            }
            else
            {
                bodyWhile = MemberBodyForSemilicon();
                if (curTok == null) return null;
                if (curTok.token != Tokens.Token.SEMILICON)
                {
                    ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                    SkipToToken(Tokens.Token.SEMILICON);
                }
            }
            return new WhileAST(condition, bodyWhile);
        }

        //---------------------
        // Парсер increment и decrement
        //---------------------

        private static ASTNode ParseCrement(Tokens.Token endToken)
        {
            string crement = curTok.subString;
            GetNextToken();
            if (curTok == null) return null;
            if (curTok.token != Tokens.Token.ID)
            {
                ConsoleHelper.WriteErrorAST("Expected 'identificator'", curTok.y, curTok.x);
                SkipToToken(endToken);
                return null;
            }
            else return new CrementAST(crement, ParseId(endToken));
        }

        private static ASTNode ParseCrement(ASTNode id, Tokens.Token endToken)
        {
            string crement = curTok.subString;
            GetNextToken();
            if (curTok == null) return null;
            if (curTok.token == Tokens.Token.OP)
                return ParseBinaryOperation(new CrementAST(crement, id));
            else if(curTok.token != endToken)
            {
                ConsoleHelper.WriteErrorAST("Expected '" + endToken.ToString() + "'", curTok.y, curTok.x);
                SkipToToken(endToken);
            }
            return new CrementAST(crement, id);
        }

        //---------------------
        // Парсер id
        //---------------------

        private static ASTNode ParseId(Tokens.Token endToken)
        {
            string idName = curTok.subString;
            string typeId = GetTypeID(idName);

            ASTNode id = new IdentificatorAST(idName);
            GetNextToken();
            if (curTok == null) return null;

            // массив
            if (curTok.token == Tokens.Token.BRACKET_L)
            {
                ASTNode memberBrackets = ParseBrackets(true);
                if (curTok == null) return null;
                return new BracketsAST(memberBrackets);
            }

            if (curTok.token == Tokens.Token.CREMENT) return ParseCrement(id, endToken);

            if (curTok.token == Tokens.Token.PARENTHESIS_L) // МЕТОД
            {
                return ParseMethod("", idName, true); // обявление в классе
            }
            else if (curTok.token == Tokens.Token.ASSIGNMENT)
            {
                ASTNode expr = ParseInitID();
                return new IdentificatorAST(typeId, idName, expr);
            }
            else
            {
                ConsoleHelper.WriteErrorAST("Wrong token. Expected '[', '(', '++' or '='", curTok.y, curTok.x);
                SkipToToken(endToken);
                return null;
            }
        }

        private static ASTNode ParseInitID()
        {
            GetNextToken();
            if (curTok == null) return null;
            if (curTok.token == Tokens.Token.K_NEW)
            {
                GetNextToken();
                if (curTok == null) return null;
                if (curTok.token != Tokens.Token.TYPE) ConsoleHelper.WriteErrorAST("Expected 'type' during variable initialization", curTok.y, curTok.x);
                ASTNode exp = ParseType(true, false, Tokens.Token.SEMILICON);
                if (curTok == null) return null;
                if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);

                return new NewAST(exp);
            }
            else
            {
                if (curTok.token != Tokens.Token.SEMILICON || curTok.token != Tokens.Token.COMM)
                {
                    ASTNode leftNode = MemberBinOperation();
                    if (leftNode == null) return null;
                    if (curTok == null) return null;
                    if (curTok.token == Tokens.Token.OP)
                    {
                        return ParseBinaryOperation(leftNode);
                    }
                    else if (curTok.token == Tokens.Token.PARENTHESIS_L)
                    {
                        ASTNode parenExpr = ParseParenthesis();
                        if (parenExpr == null) return leftNode;
                        return ParseBinaryOperation(parenExpr);
                    }
                    else if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                    return leftNode;
                }
            }
            return null;
        }

        //---------------------
        // Парсер типа
        //---------------------

        private static ASTNode ParseType(bool isInit, bool isInClass, Tokens.Token endToken)
        {
            string typeId = curTok.subString;

            GetNextToken();
            if (curTok == null) return null;

            // массив
            if (curTok.token == Tokens.Token.BRACKET_L)
            {
                if (isInit)
                {
                    ASTNode memberBrackets = ParseBrackets(isInit);
                    if (curTok == null) return null;
                    if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                    return new BracketsAST(typeId, memberBrackets);
                }
                else
                {
                    typeId = typeId + "[]";
                    GetNextToken();
                    if (curTok == null) return null;
                    if (curTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", curTok.y, curTok.x);
                    else
                    {
                        GetNextToken();
                        if (curTok == null) return null;
                    }
                } 
            }
            // переменная
            if(curTok.token == Tokens.Token.ID && !isInit) return ParseId(endToken);
            else if(curTok.token == Tokens.Token.PARENTHESIS_L && isInit)
            {
                string idName = curTok.subString;
                GetNextToken();
                if (curTok == null) return null;
                if(curTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", curTok.y, curTok.x);
                GetNextToken();
                if (curTok == null) return null;
                if (curTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                return new IdentificatorAST(typeId, idName);
            }
            else
            {
                ConsoleHelper.WriteErrorAST("Expected 'identificator'", curTok.y, curTok.x);
                SkipToToken(endToken);
                return null;
            }
        }

        //---------------------
        // Парсер бинарных выражений
        //---------------------
 
        private static ASTNode MemberBinOperation()
        {
            ASTNode node = null;
            switch (curTok.token)
            {
                case Tokens.Token.ID:
                    node = new IdentificatorAST(curTok.subString);
                    GetNextToken();
                    if (curTok == null) return node;
                    if (curTok.token == Tokens.Token.BRACKET_L) return new BracketsAST(node, ParseBrackets(true));
                    return node;
                case Tokens.Token.STRING:
                    node = new StringAST(curTok.subString);
                    break;
                case Tokens.Token.CHAR:
                    node = new CharAST(curTok.subString);
                    break;
                case Tokens.Token.INT_VALUE:
                case Tokens.Token.DOUBLE_VALUE:
                case Tokens.Token.X16_VALUE:
                case Tokens.Token.X8_VALUE:
                case Tokens.Token.X2_VALUE:
                    node = new NumAST(curTok.token, curTok.subString);
                    break;
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", curTok.y, curTok.x);
                    break;
            }
            GetNextToken();
            return node;
        }

        private static ASTNode MemberBoolBinOperation()
        {
            GetNextToken();
            if (curTok == null) return null;
            switch (curTok.token)
            {
                case Tokens.Token.BOOL:
                    return new BoolAST(curTok.subString);
                case Tokens.Token.STRING:
                case Tokens.Token.CHAR:
                case Tokens.Token.INT_VALUE:
                case Tokens.Token.DOUBLE_VALUE:
                case Tokens.Token.X16_VALUE:
                case Tokens.Token.X8_VALUE:
                case Tokens.Token.X2_VALUE:
                case Tokens.Token.ID:
                    ASTNode leftNode = MemberBinOperation();
                    ASTNode leftNodeExp;

                    if (curTok.token == Tokens.Token.OP)
                    {
                        leftNodeExp = ParseBinaryOperation(leftNode);
                        GetNextToken();
                        if (curTok == null) return null;
                    }
                    else leftNodeExp = leftNode;

                    if (curTok.token == Tokens.Token.BOOL_OP)
                    {
                        string op = curTok.subString;
                        GetNextToken();
                        if (curTok == null) return null;
                        ASTNode rightNodeExp = MemberBinOperation();
                        if (rightNodeExp == null) ConsoleHelper.WriteErrorAST("Expected 'x'", curTok.y, curTok.x);

                        if (curTok == null) return null;
                        if (curTok.token == Tokens.Token.BOOL_OP_AND || curTok.token == Tokens.Token.BOOL_OP_OR)
                            return new BoolAST(new BinaryExprAST(curTok.subString, new BinaryExprAST(op, leftNodeExp, rightNodeExp), MemberBoolBinOperation()));
                        else return new BoolAST(new BinaryExprAST(op, leftNodeExp, rightNodeExp));
                    }
                    ConsoleHelper.WriteErrorAST("Expected 'boolean'", curTok.y, curTok.x);
                    return null;
                case Tokens.Token.PARENTHESIS_L:
                    return ParseParenthesis();
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", curTok.y, curTok.x);
                    break;
            }
            return null;
        }

        private static Dictionary<string, int> opPriority = new Dictionary<string, int>()
        {
            {"%", 1},
            {"+", 1}, {"-", 1},
            {"*", 2}, {"/", 2},
            {"^", 3},
        };

        private static ASTNode ParseBinaryOperation(ASTNode leftNode)
        {
            if(curTok.token != Tokens.Token.OP) ConsoleHelper.WriteErrorAST("Expected 'op'", curTok.y, curTok.x);

            string oldOp = curTok.subString;//+
            int oldPriority = opPriority[oldOp];
            ASTNode rightMember;

            GetNextToken();
            if (curTok == null) return null;//(
            if(curTok.token == Tokens.Token.PARENTHESIS_L)
            {
                GetNextToken();
                if (curTok == null) return null;//5
                rightMember = MemberBinOperation();

                if (curTok == null) return null;//)
                if (curTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", curTok.y, curTok.x);
            }
            else rightMember = MemberBinOperation();

            if(rightMember == null) ConsoleHelper.WriteErrorAST("Expected 'identificator'", curTok.y, curTok.x);

            ASTNode binaryExpr;

            if (curTok == null) return null;
            if (curTok.token == Tokens.Token.OP)
            {
                string newOp = curTok.subString;
                int newPriority = opPriority[newOp];

                if (oldPriority > newPriority)
                {
                    binaryExpr = new BinaryExprAST(oldOp, leftNode, rightMember);
                    return ParseBinaryOperation(binaryExpr);
                }
                else
                {
                    ASTNode rightNode = ParseBinaryOperation(rightMember);
                    binaryExpr = new BinaryExprAST(newOp, leftNode, rightNode);
                    return ParseBinaryOperation(binaryExpr);
                }
            }
            return rightMember;
        }

        //---------------------
        // Парсер if
        //---------------------
        
        private static ASTNode ParseIf()
        {
            ASTNode branching;
            GetNextToken();
            if (curTok == null) return null;
            
            ASTNode condition = null;
            if (curTok.token == Tokens.Token.PARENTHESIS_L)
            {
                condition = ParseConditionIf();
                GetNextToken();
            }
            else
            {
                ConsoleHelper.WriteErrorAST("Expected '('", curTok.y, curTok.x);
                SkipToToken(Tokens.Token.BRACE_L);
            }
            ASTNode bodyIf;

            if (curTok == null) return null;
            if(curTok.token == Tokens.Token.BRACE_L)
            {
                bodyIf = new BodyMethodAST(ParseBrace(Area.METHOD, isCycleArea));
                branching = new ConditionNodeAST(condition, bodyIf);
            }
            else
            {
                bodyIf = MemberBodyForSemilicon();
                branching = new ConditionNodeAST(condition, bodyIf);
                if (curTok == null) return null;
                if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
            }
            return new IfAST(branching);
        }

        private static ASTNode ParseElse()
        {
            ASTNode branching;
            ASTNode body;
            GetNextToken();
            if (curTok == null) return null;

            if (curTok.token == Tokens.Token.K_IF) {
                bufferTok.token = Tokens.Token.K_ELSE_IF;
                return new ElseAST(ParseIf());
            }
            else if(curTok.token == Tokens.Token.BRACE_L) return new ElseAST(new BodyMethodAST(ParseBrace(Area.METHOD, isCycleArea)));
            else
            {
                if(bufferTok.token == Tokens.Token.K_ELSE) ConsoleHelper.WriteErrorAST("Expected 'else if' or 'if'", curTok.y, curTok.x);

                body = MemberBodyForSemilicon();
                branching = new ConditionNodeAST(null, body);
                if (curTok == null) return null;
                if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                GetNextToken();
                if (curTok == null) return new ElseAST(branching);
            }
            return new ElseAST(branching);
        }

        private static ASTNode ParseConditionIf()
        {
            ASTNode expr = MemberBoolBinOperation();
            if (expr == null) ConsoleHelper.WriteErrorAST("Expected '( X )'", curTok.y, curTok.x);
            if (curTok == null) return null;
            if (curTok.token != Tokens.Token.PARENTHESIS_R)
            {
                ConsoleHelper.WriteErrorAST("Expected ')'", curTok.y, curTok.x);
                SkipToToken(Tokens.Token.BRACE_L);
            }
            return expr;
        }

        //---------------------
        // Парсер for
        //---------------------

        private static ASTNode ParseFor()
        {
            GetNextToken();
            if (curTok == null) return null;

            List<ASTNode> declaredVar = new List<ASTNode>();
            ASTNode condition = null;
            ASTNode postCondition = null;
            ASTNode body;

            if (curTok.token == Tokens.Token.PARENTHESIS_L)
            {
                declaredVar = ParseDeclaredVarInFor();
                condition = ParseConditionFor();
                postCondition = ParsePostConditionFor();
            }
            else ConsoleHelper.WriteErrorAST("Expected '('", curTok.y, curTok.x);

            if (curTok == null) return null;
            if(curTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", curTok.y, curTok.x);

            GetNextToken();
            if (curTok == null) return null;
            if (curTok.token == Tokens.Token.BRACE_L)
                body = new BodyMethodAST(ParseBrace(Area.METHOD, true));
            else
            {
                body = MemberBodyForSemilicon();
                if (curTok == null) return null;
                if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
            }
            return new ForAST(declaredVar, new ConditionNodeAST(condition, body), postCondition);
        }

        private static ASTNode MemberBodyForSemilicon()
        {
            switch (curTok.token)
            {
                case Tokens.Token.TYPE:
                    return ParseType(false, false, Tokens.Token.SEMILICON);
                case Tokens.Token.ID:
                    return ParseId(Tokens.Token.SEMILICON);
                case Tokens.Token.CREMENT:
                    return ParseCrement(Tokens.Token.SEMILICON);
                case Tokens.Token.K_FOR:
                    return ParseFor();
                case Tokens.Token.K_IF:
                    return ParseIf();
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", curTok.y, curTok.x);
                    break;
            }
            return null;
        }

        private static ASTNode ParsePostConditionFor()
        {
            List<ASTNode> postcondition = new List<ASTNode>();
            while (true)
            {
                ASTNode member = MemberPosconditionFor();
                if (curTok == null) return new PostconditionForAST(postcondition);
                if (member == null && curTok.token != Tokens.Token.PARENTHESIS_R) break;

                postcondition.Add(member);
                if (curTok.token != Tokens.Token.COMM)
                {
                    ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                    SkipToToken(Tokens.Token.PARENTHESIS_R);
                    break;
                }
            }
            return new PostconditionForAST(postcondition);
        }

        private static ASTNode MemberPosconditionFor()
        {
            GetNextToken();
            if (curTok == null) return null;
            switch (curTok.token)
            {
                case Tokens.Token.ID:
                    return ParseId(Tokens.Token.PARENTHESIS_R);
                case Tokens.Token.CREMENT:
                    GetNextToken();
                    if (curTok == null) return null;
                    if(curTok.token != Tokens.Token.ID) ConsoleHelper.WriteErrorAST("Expected 'identificator'", curTok.y, curTok.x);
                    return new CrementAST(curTok.subString, new IdentificatorAST(curTok.subString));
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", curTok.y, curTok.x);
                    break;
            }
            return null;
        }

        private static ASTNode ParseConditionFor()
        {
            ASTNode condition = MemberBoolBinOperation();
            if (curTok == null) return null;
            if (curTok.token != Tokens.Token.SEMILICON)
            {
                ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                SkipToToken(Tokens.Token.BRACE_L);
                return null;
            }
            return condition;
        }

        private static List<ASTNode> ParseDeclaredVarInFor()
        {
            List<ASTNode> declaredVar = new List<ASTNode>();
            string generalType = null;
            bool isComm = true;
            bool isFirst = true;
            while (true)
            {
                GetNextToken();
                if (curTok == null) return null;
                if (curTok.token == Tokens.Token.TYPE && isComm && isFirst)
                {
                    generalType = curTok.subString;
                    ASTNode id = ParseType(false, false, Tokens.Token.SEMILICON);
                    if(id != null) declaredVar.Add(id);
                }
                else if (curTok.token == Tokens.Token.ID && isComm)
                {
                    isFirst = false;
                    ASTNode id = null;
                    string idName = curTok.subString;
                    GetNextToken();
                    if (curTok == null) return null;
                    if (curTok.token == Tokens.Token.ASSIGNMENT)
                    {
                        string typeId = GetTypeID(idName);

                        if (typeId == null && generalType == null)
                        {
                            ConsoleHelper.WriteErrorAST("ID not init", curTok.y, curTok.x);
                            return null;
                        }
                        else if(typeId == null && generalType != null && !isFirst)
                        {
                            ASTNode expr = ParseInitID();
                            id = new IdentificatorAST(typeId, idName, expr);
                            declaredVar.Add(id);
                        }
                        else
                        {
                            ConsoleHelper.WriteErrorAST("ID not init", curTok.y, curTok.x);
                            return null;
                        }
                    }
                    else 
                    {
                        ConsoleHelper.WriteErrorAST("Expected '=", curTok.y, curTok.x);
                    }
                    isComm = curTok.token == Tokens.Token.COMM;
                }
                else if (curTok.token == Tokens.Token.SEMILICON)
                {
                    break;
                }
                else
                {
                    ConsoleHelper.WriteErrorAST("Ecpected 'type' or 'id'", curTok.y, curTok.x);
                    return null;
                }
                if (curTok.token == Tokens.Token.SEMILICON) break;
            }
            return declaredVar;
        }

        private static string GetTypeID(string idName)
        {
            return "";//извлекает из хэш таблицы тип переменной
        }

        //---------------------
        // Парсер return
        //---------------------

        static string typeMethodReturn = "";

        private static ASTNode ParseReturn()
        {
            ASTNode memberReturn = MemberReturn();
            if (curTok == null) return null;
            if (typeMethodReturn == "void" && memberReturn != null)
                ConsoleHelper.WriteErrorAST("Wrong type return", curTok.y, curTok.x);
            else
            {
                if (memberReturn is IdentificatorAST)
                {
                    if ((memberReturn as IdentificatorAST).GetTypeId() == typeMethodReturn)
                        return new ReturnAST(typeMethodReturn, memberReturn);
                }
                if (memberReturn is MethodAST)
                {
                    if ((memberReturn as MethodAST).GetTypeMethod() == typeMethodReturn)
                        return new ReturnAST(typeMethodReturn, memberReturn);
                }
                if (memberReturn is NumAST)
                {
                    if ((memberReturn as NumAST).GetTypeValue() == typeMethodReturn)
                        return new ReturnAST(typeMethodReturn, memberReturn);
                }
                if (memberReturn is BinaryExprAST)
                {
                    if ((memberReturn as BinaryExprAST).GetTypeExp() == typeMethodReturn)
                        return new ReturnAST(typeMethodReturn, memberReturn);
                }
            }
            ConsoleHelper.WriteErrorAST("Wrong type return", curTok.y, curTok.x);
            return null;
        }

        private static ASTNode MemberReturn()
        {
            GetNextToken();
            if (curTok == null) return null;
            switch (curTok.token)
            {
                case Tokens.Token.ID:
                    return ParseId(Tokens.Token.SEMILICON);
                case Tokens.Token.INT_VALUE:
                case Tokens.Token.DOUBLE_VALUE:
                case Tokens.Token.X16_VALUE:
                case Tokens.Token.X8_VALUE:
                case Tokens.Token.X2_VALUE:
                    NumAST value = new NumAST(curTok.token, curTok.subString);
                    GetNextToken();
                    if (curTok == null) return null;
                    if (curTok.token == Tokens.Token.OP) return ParseBinaryOperation(value);
                    else if (curTok.token == Tokens.Token.SEMILICON) return value;
                    ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                    SkipToToken(Tokens.Token.SEMILICON);
                    return null;
                case Tokens.Token.CREMENT:
                    return ParseCrement(Tokens.Token.SEMILICON);
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token this area", curTok.y, curTok.x);
                    return null;
            }
        }

        //---------------------
        // Возвращает следующий токен
        //---------------------

        private static void GetNextToken()
        {
            curTok = Lexer.GetToken();
        }

        private static void SkipToToken(Tokens.Token token)
        {
            while (curTok.token != token)
            {
                GetNextToken();
                if (curTok == null) break;
            }
        }
    }
}
