using lab1.ASTNodes;
using lab1.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace lab1
{
    class AbstractSyntaxTree
    {
        private static Hashtable hashtable = new Hashtable();
        private static TokenNode CurTok;
        private static ASTNode headAST;
        private static bool isCycleArea = false;

        public static void CreateAST()
        {
            GetNextToken();
            if (CurTok == null) return;
            if (CurTok.token != Tokens.Token.K_NAMESPACE)
            {
                ConsoleHelper.WriteErrorAST("Expected 'namespace'", CurTok.y, CurTok.x);
                SkipToToken(Tokens.Token.K_NAMESPACE);
            }
            else GetNextToken();

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
                if (CurTok == null) return null;

                if (CurTok.token == Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected '[X]'", CurTok.y, CurTok.x);

                ASTNode exp = MemberBinOperation();
                if (CurTok == null) return null;

                if(CurTok.token == Tokens.Token.BRACKET_L)
                {
                    exp = new BracketsAST(exp, ParseBrackets(true));
                    if (CurTok == null) return null;
                }

                brackets = exp;
                if (CurTok.token == Tokens.Token.OP)
                {
                    brackets = ParseBinaryOperation(exp);
                    if (CurTok == null) return null;
                }
                else if (CurTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", CurTok.y, CurTok.x);
            }
            else
            {
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", CurTok.y, CurTok.x);
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
                if (CurTok == null) break;
                if (CurTok.token == Tokens.Token.BRACE_R) break;
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

        private static bool isRawToken = false;
        private static ASTNode MemberMethod(bool isCycle)
        {
            if (!isRawToken)
            {
                GetNextToken();
                if (CurTok == null) return null;
            }
            else isRawToken = false;
            switch (CurTok.token)
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
                    return ParseIf();
                case Tokens.Token.K_WHILE:
                    return ParseWhile();
                case Tokens.Token.K_CONTINUE:
                case Tokens.Token.K_BREAK:
                    if (!isCycle) ConsoleHelper.WriteErrorAST("No external loop to interrupt or continue", CurTok.y, CurTok.x);
                    ASTNode node;
                    if (CurTok.token == Tokens.Token.K_BREAK) node = new BreakAST();
                    else node = new ContinueAST();

                    GetNextToken();
                    if (CurTok == null) return node;
                    if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
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
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Tokens.Token.BRACE_R:
                    return null;
                case Tokens.Token.K_CLASS:
                    return ParseMainArea(Area.CLASS);
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", CurTok.y, CurTok.x);
                    return null;
            }
        }

        private static ASTNode MemberClass()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Tokens.Token.TYPE:
                    return ParseType(false, true, Tokens.Token.SEMILICON);
                case Tokens.Token.BRACE_R:
                    return null;
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", CurTok.y, CurTok.x);
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
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.SEMILICON && CurTok.token != Tokens.Token.PARENTHESIS_R)
            {
                ASTNode leftNode = MemberBinOperation();
                if (leftNode == null) return null;
                if (CurTok == null) return null;
                if (CurTok.token == Tokens.Token.OP)
                {
                    exprassion = ParseBinaryOperation(leftNode);
                    return new ParenthesisExprAST(exprassion);
                }
                else if (CurTok.token == Tokens.Token.PARENTHESIS_L)
                {
                    ASTNode parenExpr = ParseParenthesis();
                    if (parenExpr == null) return null;
                    exprassion = ParseBinaryOperation(parenExpr);
                    return new ParenthesisExprAST(exprassion);
                }
                else if (CurTok.token != Tokens.Token.SEMILICON) return null;
            }
            else
            {
                ConsoleHelper.WriteErrorAST("Expected ')' or empty ()", CurTok.y, CurTok.x);
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
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.ID)
            {
                idName = "[NONAME]";
                ConsoleHelper.WriteErrorAST("Expected 'identificator'", CurTok.y, CurTok.x);
                SkipToToken(Tokens.Token.BRACE_L);
            }
            else
            {
                idName = CurTok.subString;
                GetNextToken();
            }

            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.BRACE_L) ConsoleHelper.WriteErrorAST("Expected '{'", CurTok.y, CurTok.x);

            List<ASTNode> memberArea = ParseBrace(Area.CLASS, false);

            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.BRACE_R) ConsoleHelper.WriteErrorAST("Expected '}'", CurTok.y, CurTok.x);

            if(area == Area.NAMESPACE) return new NamespaceAST(memberArea, idName);
            else return new ClassAST(memberArea, idName);
        }

        //---------------------
        // Парсер метода
        //---------------------
        private static ASTNode ParseArgInMethod(bool isCall)
        {
            if (CurTok.token == Tokens.Token.ID && isCall)
            {
                IdentificatorAST identificatorAST = new IdentificatorAST(CurTok.subString);
                return identificatorAST;
            }
            else if (CurTok.token == Tokens.Token.TYPE && !isCall)
            {
                return ParseType(false, false, Tokens.Token.PARENTHESIS_R);
            }
            else if (CurTok.token == Tokens.Token.PARENTHESIS_R)
            {
                return null;
            }
            else ConsoleHelper.WriteErrorAST("Expected 'type' for identificator in method declaration", CurTok.y, CurTok.x);
            return null;
        }

        private static List<ASTNode> ParseArgsMethod(bool isCall)
        {
            List<ASTNode> argsMethod = new List<ASTNode>();

            GetNextToken();
            if (CurTok == null) return null;

            while (true)
            {
                if (argsMethod.Count >= 1)
                {
                    if (CurTok.token == Tokens.Token.COMM)
                    {
                        ASTNode argMethod = ParseArgInMethod(isCall);
                        if (CurTok == null) break;
                        if (argMethod != null) argsMethod.Add(argMethod);
                        if (CurTok.token == Tokens.Token.COMM) continue;
                        if (CurTok.token != Tokens.Token.PARENTHESIS_R) break;
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
            if (CurTok.token != Tokens.Token.PARENTHESIS_R)
            {
                ConsoleHelper.WriteErrorAST("Expected ')'", CurTok.y, CurTok.x);
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
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.BRACE_L) ConsoleHelper.WriteErrorAST("Expected '{'", CurTok.y, CurTok.x);
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
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.PARENTHESIS_L)
            {
                ConsoleHelper.WriteErrorAST("Expected '('", CurTok.y, CurTok.x);
                SkipToToken(Tokens.Token.BRACE_L);
            }
            else
            {
                condition = ParseConditionIf();
                GetNextToken();
            }

            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.BRACE_L)
            {
                bodyWhile = new BodyMethodAST(ParseBrace(Area.METHOD, true));
            }
            else
            {
                bodyWhile = MemberBodyForSemilicon();
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.SEMILICON)
                {
                    ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
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
            string crement = CurTok.subString;
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.ID)
            {
                ConsoleHelper.WriteErrorAST("Expected 'identificator'", CurTok.y, CurTok.x);
                SkipToToken(endToken);
                return null;
            }
            else return new CrementAST(crement, ParseId(endToken));
        }

        private static ASTNode ParseCrement(ASTNode id, Tokens.Token endToken)
        {
            string crement = CurTok.subString;
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.OP)
                return ParseBinaryOperation(new CrementAST(crement, id));
            else if(CurTok.token != endToken)
            {
                ConsoleHelper.WriteErrorAST("Expected '" + endToken.ToString() + "'", CurTok.y, CurTok.x);
                SkipToToken(endToken);
            }
            return new CrementAST(crement, id);
        }

        //---------------------
        // Парсер id
        //---------------------

        private static ASTNode ParseId(Tokens.Token endToken)
        {
            string idName = CurTok.subString;
            ASTNode id = new IdentificatorAST(idName);
            GetNextToken();
            if (CurTok == null) return null;

            // массив
            if (CurTok.token == Tokens.Token.BRACKET_L)
            {
                ASTNode memberBrackets = ParseBrackets(true);
                if (CurTok == null) return null;
                return new BracketsAST(memberBrackets);
            }

            if (CurTok.token == Tokens.Token.CREMENT) return ParseCrement(id, endToken);

            if (CurTok.token == Tokens.Token.PARENTHESIS_L) // МЕТОД
            {
                return ParseMethod("", idName, true); // обявление в классе
            }
            else if (CurTok.token == Tokens.Token.ASSIGNMENT)
            {
                ASTNode expr = ParseInitID();
                return new IdentificatorAST(idName, expr);
            }
            else
            {
                ConsoleHelper.WriteErrorAST("Wrong token. Expected '[', '(', '++' or '='", CurTok.y, CurTok.x);
                SkipToToken(endToken);
                return null;
            }
        }

        private static ASTNode ParseInitID()
        {
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.K_NEW)
            {
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.TYPE) ConsoleHelper.WriteErrorAST("Expected 'type' during variable initialization", CurTok.y, CurTok.x);
                ASTNode exp = ParseType(true, false, Tokens.Token.SEMILICON);
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);

                return new NewAST(exp);
            }
            else
            {
                if (CurTok.token != Tokens.Token.SEMILICON || CurTok.token != Tokens.Token.COMM)
                {
                    ASTNode leftNode = MemberBinOperation();
                    if (leftNode == null) return null;
                    if (CurTok == null) return null;
                    if (CurTok.token == Tokens.Token.OP)
                    {
                        return ParseBinaryOperation(leftNode);
                    }
                    else if (CurTok.token == Tokens.Token.PARENTHESIS_L)
                    {
                        ASTNode parenExpr = ParseParenthesis();
                        if (parenExpr == null) return leftNode;
                        return ParseBinaryOperation(parenExpr);
                    }
                    else if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
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
            string typeId = CurTok.subString;

            GetNextToken();
            if (CurTok == null) return null;

            // массив
            if (CurTok.token == Tokens.Token.BRACKET_L)
            {
                if (isInit)
                {
                    ASTNode memberBrackets = ParseBrackets(isInit);
                    if (CurTok == null) return null;
                    if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                    return new BracketsAST(typeId, memberBrackets);
                }
                else
                {
                    typeId = typeId + "[]";
                    GetNextToken();
                    if (CurTok == null) return null;
                    if (CurTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", CurTok.y, CurTok.x);
                    else
                    {
                        GetNextToken();
                        if (CurTok == null) return null;
                    }
                }
            }
            // переменная
            if(CurTok.token == Tokens.Token.ID && !isInit)
            {
                string idName = CurTok.subString;
                GetNextToken();
                if (CurTok == null) return null;

                if (CurTok.token == Tokens.Token.PARENTHESIS_L) // МЕТОД
                {
                    if (isInClass) return ParseMethod(typeId, idName, false); // обявление в классе
                    else
                    {
                        ConsoleHelper.WriteErrorAST("Method declaration in a method", CurTok.y, CurTok.x);
                        return null; // ошибка при объявлении в методе
                    }
                }
                else if (CurTok.token == Tokens.Token.ASSIGNMENT)
                {
                    if(endToken != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Impossible token in this area '='", CurTok.y, CurTok.x);
                    else
                    {
                        ASTNode expr = ParseInitID();
                        if (CurTok.token != endToken)
                        {
                            ConsoleHelper.WriteErrorAST("Expected '" + endToken.ToString() + "'", CurTok.y, CurTok.x);
                            SkipToToken(endToken);
                        }
                        return new IdentificatorAST(typeId, idName, expr);
                    }
                }

                if (CurTok.token != endToken)
                {
                    ConsoleHelper.WriteErrorAST("Expected '" + endToken.ToString() + "'", CurTok.y, CurTok.x);
                    SkipToToken(endToken);
                }
                return new IdentificatorAST(typeId, idName);
            }
            else if(CurTok.token == Tokens.Token.PARENTHESIS_L && isInit)
            {
                string idName = CurTok.subString;
                GetNextToken();
                if (CurTok == null) return null;
                if(CurTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", CurTok.y, CurTok.x);
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                return new IdentificatorAST(typeId, idName);
            }
            else
            {
                ConsoleHelper.WriteErrorAST("Expected 'identificator'", CurTok.y, CurTok.x);
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
            switch (CurTok.token)
            {
                case Tokens.Token.ID:
                    node = new IdentificatorAST(CurTok.subString);
                    GetNextToken();
                    if (CurTok == null) return node;
                    if (CurTok.token == Tokens.Token.BRACKET_L) return new BracketsAST(node, ParseBrackets(true));
                    return node;
                case Tokens.Token.STRING:
                    node = new StringAST(CurTok.subString);
                    break;
                case Tokens.Token.CHAR:
                    node = new CharAST(CurTok.subString);
                    break;
                case Tokens.Token.INT_VALUE:
                case Tokens.Token.DOUBLE_VALUE:
                case Tokens.Token.X16_VALUE:
                case Tokens.Token.X8_VALUE:
                case Tokens.Token.X2_VALUE:
                    node = new NumAST(CurTok.token, CurTok.subString);
                    break;
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", CurTok.y, CurTok.x);
                    break;
            }
            GetNextToken();
            return node;
        }

        private static ASTNode MemberBoolBinOperation()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Tokens.Token.BOOL:
                    return new BoolAST(CurTok.subString);
                case Tokens.Token.STRING:
                case Tokens.Token.CHAR:
                case Tokens.Token.INT_VALUE:
                case Tokens.Token.DOUBLE_VALUE:
                case Tokens.Token.X16_VALUE:
                case Tokens.Token.X8_VALUE:
                case Tokens.Token.X2_VALUE:
                case Tokens.Token.ID:
                    string idName = CurTok.subString;
                    ASTNode leftNode = MemberBinOperation();
                    ASTNode leftNodeExp;

                    if (CurTok.token == Tokens.Token.OP)
                    {
                        leftNodeExp = ParseBinaryOperation(leftNode);
                        GetNextToken();
                        if (CurTok == null) return null;
                    }
                    else leftNodeExp = leftNode;

                    if (CurTok.token == Tokens.Token.BOOL_OP)
                    {
                        string op = CurTok.subString;
                        GetNextToken();
                        if (CurTok == null) return null;
                        ASTNode rightNodeExp = MemberBinOperation();
                        if (rightNodeExp == null) ConsoleHelper.WriteErrorAST("Expected 'x'", CurTok.y, CurTok.x);

                        if (CurTok == null) return null;
                        if (CurTok.token == Tokens.Token.BOOL_OP_AND || CurTok.token == Tokens.Token.BOOL_OP_OR)
                        {
                            return new BoolAST(new BinaryExprAST(CurTok.subString, new BinaryExprAST(op, leftNodeExp, rightNodeExp), MemberBoolBinOperation()));
                        }
                        else
                        {
                            return new BoolAST(new BinaryExprAST(op, leftNodeExp, rightNodeExp));
                        }
                    }
                    ConsoleHelper.WriteErrorAST("Expected 'boolean'", CurTok.y, CurTok.x);
                    return null;
                case Tokens.Token.PARENTHESIS_L:
                    return ParseParenthesis();
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", CurTok.y, CurTok.x);
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
            if(CurTok.token != Tokens.Token.OP) ConsoleHelper.WriteErrorAST("Expected 'op'", CurTok.y, CurTok.x);

            string oldOp = CurTok.subString;//+
            int oldPriority = opPriority[oldOp];
            ASTNode rightMember;

            GetNextToken();
            if (CurTok == null) return null;//(
            if(CurTok.token == Tokens.Token.PARENTHESIS_L)
            {
                GetNextToken();
                if (CurTok == null) return null;//5
                rightMember = MemberBinOperation();

                if (CurTok == null) return null;//)
                if (CurTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", CurTok.y, CurTok.x);
            }
            else rightMember = MemberBinOperation();

            if(rightMember == null) ConsoleHelper.WriteErrorAST("Expected 'identificator'", CurTok.y, CurTok.x);

            ASTNode binaryExpr;

            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.OP)
            {
                string newOp = CurTok.subString;
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
            List<ConditionNodeAST> branching = new List<ConditionNodeAST>();
            GetNextToken();
            if (CurTok == null) return null;
            
            bool isCondition = true;
            while (true)
            {
                ASTNode condition = null;
                if (CurTok.token == Tokens.Token.PARENTHESIS_L && isCondition)
                {
                    condition = ParseConditionIf();
                }
                else ConsoleHelper.WriteErrorAST("Expected '('", CurTok.y, CurTok.x);
                isCondition = false;
                ASTNode bodyIf = null;

                GetNextToken();
                if (CurTok == null) return null;
                if(CurTok.token == Tokens.Token.BRACE_L)
                {
                    bodyIf = new BodyMethodAST(ParseBrace(Area.METHOD, isCycleArea));
                    branching.Add(new ConditionNodeAST(condition, bodyIf));
                }
                else
                {
                    bodyIf = MemberBodyForSemilicon();
                    branching.Add(new ConditionNodeAST(condition, bodyIf));
                    if (CurTok == null) return null;
                    if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                    GetNextToken();
                    if (CurTok == null) return new IfAST(branching);
                }

                if (CurTok.token == Tokens.Token.K_ELSE)
                {
                    GetNextToken();
                    if (CurTok == null) return null;
                    if (CurTok.token == Tokens.Token.K_IF)
                    {
                        isCondition = true;
                        continue;
                    }
                    else
                    {
                        if (CurTok.token == Tokens.Token.BRACE_L)
                        {
                            bodyIf = new BodyMethodAST(ParseBrace(Area.METHOD, isCycleArea));
                            branching.Add(new ConditionNodeAST(condition, bodyIf));
                            GetNextToken();
                            if (CurTok == null) return new IfAST(branching);
                        }
                        else
                        {
                            bodyIf = MemberBodyForSemilicon();
                            branching.Add(new ConditionNodeAST(condition, bodyIf));
                            if (CurTok == null) return null;
                            if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                            GetNextToken();
                            if (CurTok == null) return new IfAST(branching);
                            break;
                        }
                    }
                }
                else isRawToken = true;
                break;
            }
            return new IfAST(branching);
        }

        private static ASTNode ParseConditionIf()
        {
            ASTNode expr = MemberBoolBinOperation();
            if (expr == null) ConsoleHelper.WriteErrorAST("Expected '( X )'", CurTok.y, CurTok.x);
            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.PARENTHESIS_R)
            {
                return expr;
            }
            ConsoleHelper.WriteErrorAST("Expected ')'", CurTok.y, CurTok.x);
            return null;
        }

        //---------------------
        // Парсер for
        //---------------------

        private static ASTNode ParseFor()
        {
            GetNextToken();
            if (CurTok == null) return null;

            List<ASTNode> declaredVar = new List<ASTNode>();
            ASTNode condition = null;
            ASTNode postCondition = null;
            ASTNode body;

            if (CurTok.token == Tokens.Token.PARENTHESIS_L)
            {
                declaredVar = ParseDeclaredVarInFor();
                condition = ParseConditionFor();
                postCondition = ParsePostConditionFor();
            }
            else ConsoleHelper.WriteErrorAST("Expected '('", CurTok.y, CurTok.x);

            if (CurTok == null) return null;
            if(CurTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", CurTok.y, CurTok.x);

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.BRACE_L)
                body = new BodyMethodAST(ParseBrace(Area.METHOD, true));
            else
            {
                body = MemberBodyForSemilicon();
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
            }
            return new ForAST(declaredVar, new ConditionNodeAST(condition, body), postCondition);
        }

        private static ASTNode MemberBodyForSemilicon()
        {
            switch (CurTok.token)
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
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", CurTok.y, CurTok.x);
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
                if (CurTok == null) return new PostconditionForAST(postcondition);
                if (member == null && CurTok.token != Tokens.Token.PARENTHESIS_R) break;

                postcondition.Add(member);
                if (CurTok.token != Tokens.Token.COMM)
                {
                    ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                    SkipToToken(Tokens.Token.PARENTHESIS_R);
                    break;
                }
            }
            return new PostconditionForAST(postcondition);
        }

        private static ASTNode MemberPosconditionFor()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Tokens.Token.ID:
                    return ParseId(Tokens.Token.PARENTHESIS_R);
                case Tokens.Token.CREMENT:
                    GetNextToken();
                    if (CurTok == null) return null;
                    if(CurTok.token != Tokens.Token.ID) ConsoleHelper.WriteErrorAST("Expected 'identificator'", CurTok.y, CurTok.x);
                    return new CrementAST(CurTok.subString, new IdentificatorAST(CurTok.subString));
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", CurTok.y, CurTok.x);
                    break;
            }
            return null;
        }

        private static ASTNode ParseConditionFor()
        {
            ASTNode condition = MemberBoolBinOperation();
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.SEMILICON)
            {
                ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
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
                if (CurTok == null) return null;
                if (CurTok.token == Tokens.Token.TYPE && isComm && isFirst)
                {
                    generalType = CurTok.subString;
                    ASTNode id = ParseType(false, false, Tokens.Token.SEMILICON);
                    declaredVar.Add(id);
                }
                else if (CurTok.token == Tokens.Token.ID && isComm)
                {
                    isFirst = false;
                    ASTNode id = null;
                    string idName = CurTok.subString;
                    GetNextToken();
                    if (CurTok == null) return null;
                    if (CurTok.token == Tokens.Token.ASSIGNMENT)
                    {
                        string typeId = GetTypeID();

                        if (typeId == null && generalType == null)
                        {
                            ConsoleHelper.WriteErrorAST("ID not init", CurTok.y, CurTok.x);
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
                            ConsoleHelper.WriteErrorAST("ID not init", CurTok.y, CurTok.x);
                            return null;
                        }
                    }
                    else 
                    {
                        ConsoleHelper.WriteErrorAST("Expected '=", CurTok.y, CurTok.x);
                    }
                    isComm = CurTok.token == Tokens.Token.COMM;
                }
                else if (CurTok.token == Tokens.Token.SEMILICON)
                {
                    break;
                }
                else
                {
                    ConsoleHelper.WriteErrorAST("Ecpected 'type' or 'id'", CurTok.y, CurTok.x);
                    return null;
                }
                if (CurTok.token == Tokens.Token.SEMILICON) break;
            }
            return declaredVar;
        }

        private static string GetTypeID()
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
            if(memberReturn is IdentificatorAST)
            {
                if((memberReturn as IdentificatorAST).GetTypeId() == typeMethodReturn)
                    return new ReturnAST(typeMethodReturn, memberReturn);
            }
            if (memberReturn is MethodAST)
            {
                if ((memberReturn as MethodAST).GetTypeMethod() == typeMethodReturn)
                    return new ReturnAST(typeMethodReturn, memberReturn);
            }
            if (memberReturn is BinaryExprAST)
            {
                if ((memberReturn as BinaryExprAST).GetTypeExp() == typeMethodReturn)
                    return new ReturnAST(typeMethodReturn, memberReturn);
            }
            return null;
        }

        private static ASTNode MemberReturn()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Tokens.Token.ID:
                    return ParseId(Tokens.Token.SEMILICON);
                case Tokens.Token.INT_VALUE:
                case Tokens.Token.DOUBLE_VALUE:
                case Tokens.Token.X16_VALUE:
                case Tokens.Token.X8_VALUE:
                case Tokens.Token.X2_VALUE:
                    NumAST value = new NumAST(CurTok.token, CurTok.subString);
                    GetNextToken();
                    if (CurTok == null) return null;
                    if (CurTok.token == Tokens.Token.OP) return ParseBinaryOperation(value);
                    else if (CurTok.token == Tokens.Token.SEMILICON) return value;
                    ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                    SkipToToken(Tokens.Token.SEMILICON);
                    return null;
                case Tokens.Token.CREMENT:
                    return ParseCrement(Tokens.Token.SEMILICON);
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token this area", CurTok.y, CurTok.x);
                    return null;
            }
        }

        //---------------------
        // Возвращает следующий токен
        //---------------------

        private static void GetNextToken()
        {
            CurTok = Lexer.GetToken();
        }

        private static void SkipToToken(Tokens.Token token)
        {
            while (CurTok.token != token)
            {
                GetNextToken();
                if (CurTok == null) break;
            }
        }
    }
}
