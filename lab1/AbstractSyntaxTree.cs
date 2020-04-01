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
            //GetNextToken();
            headAST = ParseNamespace();
            headAST.Print("");
        }

        //---------------------
        // Парсер скобки [
        //---------------------

        private static ASTNode ParseBrackets(bool isCanValues)
        {
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
                }

                ASTNode exprNode = exp;
                if (CurTok.token == Tokens.Token.OP)
                {
                    exprNode = ParseBinaryOperation(exp);
                    if (CurTok == null) return null;
                }
                else if (CurTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", CurTok.y, CurTok.x);
                else GetNextToken();
                return exprNode;
            }
            else
            {
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", CurTok.y, CurTok.x);
                return new BracketsAST();
            }
        }

        //---------------------
        // Парсер области имен
        //---------------------

        private static ASTNode ParseNamespace()
        {
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.K_NAMESPACE) ConsoleHelper.WriteErrorAST("Expected 'namespace'", CurTok.y, CurTok.x);

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.ID) ConsoleHelper.WriteErrorAST("Expected 'identificator' for namespace", CurTok.y, CurTok.x);

            string idName = CurTok.subString;

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.BRACE_L) ConsoleHelper.WriteErrorAST("Expected '{'", CurTok.y, CurTok.x);

            List<ASTNode> memberNamespace = ParseBraceNamespace();

            GetNextToken();
            if (CurTok == null) return new NamespaceAST(memberNamespace, idName);
            if (CurTok.token != Tokens.Token.BRACE_R) ConsoleHelper.WriteErrorAST("Expected '}'", CurTok.y, CurTok.x);

            return new NamespaceAST(memberNamespace, idName);
        }

        private static List<ASTNode> ParseBraceNamespace()
        {
            List<ASTNode> memmbersNamespace = new List<ASTNode>();
            while (true)
            {
                ASTNode memmberNamespace = MemmberNamespace();

                if (memmberNamespace == null) break; // null
                memmbersNamespace.Add(memmberNamespace);

                if (CurTok == null) return memmbersNamespace;
                if (CurTok.token == Tokens.Token.BRACE_R) break; // null
            }
            return memmbersNamespace;
        }

        private static ASTNode MemmberNamespace()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Tokens.Token.BRACE_R: // найден конец области имен (если все правильно)
                    return null;
                case Tokens.Token.TYPE: // тип
                    return null;
                case Tokens.Token.K_CLASS: // класс
                    return ParseClass();
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", CurTok.y, CurTok.x);
                    return null;
            }
        }

        //---------------------
        // Парсер класса
        //---------------------

        private static ASTNode ParseClass()
        {
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.ID) ConsoleHelper.WriteErrorAST("Expected 'identificator' for class", CurTok.y, CurTok.x);

            string idName = CurTok.subString;

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.BRACE_L) ConsoleHelper.WriteErrorAST("Expected '{'", CurTok.y, CurTok.x);

            List<ASTNode> memberClass = ParseBraceClass();

            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.BRACE_R) ConsoleHelper.WriteErrorAST("Expected '}'", CurTok.y, CurTok.x);

            return new ClassAST(memberClass, idName);
        }

        private static List<ASTNode> ParseBraceClass()
        {
            List<ASTNode> memmbersClass = new List<ASTNode>();
            GetNextToken();
            if (CurTok == null) return null;
            while (true)
            {
                ASTNode memmberClass = MemmberClass();
                if (memmberClass == null) return memmbersClass;
                memmbersClass.Add(memmberClass);
            }
        }

        private static ASTNode MemmberClass()
        {
            //GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Tokens.Token.TYPE:
                    return ParseType(false, true);
                case Tokens.Token.BRACE_R:
                    return null;
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", CurTok.y, CurTok.x);
                    return null;
            }
        }

        //---------------------
        // Парсер метода
        //---------------------

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
                        if (argMethod != null) argsMethod.Add(argMethod);
                    }
                    else if(CurTok.token == Tokens.Token.PARENTHESIS_R)
                    {
                        return argsMethod;
                    }
                    else ConsoleHelper.WriteErrorAST("Expected ','", CurTok.y, CurTok.x);
                }
                else
                {
                    ASTNode argMethod = ParseArgInMethod(isCall);
                    if (argMethod != null) argsMethod.Add(argMethod);
                    else return argsMethod;
                }
            }
        }

        private static ASTNode ParseMethod(string typeId, string idName, bool isCall)
        {
            List<ASTNode> argsMethod = ParseArgsMethod(isCall);

            List<ASTNode> bodyMethod = new List<ASTNode>();
            if (!isCall)
            {
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.BRACE_L) ConsoleHelper.WriteErrorAST("Expected '{'", CurTok.y, CurTok.x);
                bodyMethod = ParseBraceMethod(false);
            }

            if (argsMethod.Count > 0)
            {
                if (isCall)
                {
                    return new MethodAST(idName, argsMethod);
                }
                else
                {
                    return new MethodAST(typeId, idName, argsMethod, new BodyMethodAST(bodyMethod));
                }
            }
            else
            {
                if (isCall)
                {
                    return new MethodAST(idName);
                }
                else
                {
                    return new MethodAST(typeId, idName, new BodyMethodAST(bodyMethod));
                }
            }
        }

        private static List<ASTNode> ParseBraceMethod(bool isCycle)
        {
            isCycleArea = isCycle;
            List<ASTNode> membersBodyMethod = new List<ASTNode>();
            while (true)
            {
                ASTNode member = MemmberBodyMethod(isCycle);
                if (member != null) membersBodyMethod.Add(member);
                if (CurTok == null) return membersBodyMethod;
                if (CurTok.token == Tokens.Token.BRACE_R) break;
            }
            GetNextToken();
            if (CurTok == null) return membersBodyMethod;
            return membersBodyMethod;
        }

        private static ASTNode MemmberBodyMethod(bool isCycle)
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Tokens.Token.SEMILICON:
                    break;
                case Tokens.Token.TYPE:
                    return ParseType(false, false);
                case Tokens.Token.ID:
                    return ParseId();
                case Tokens.Token.CREMENT:
                    return ParseCrement();
                case Tokens.Token.K_FOR:
                    return ParseFor();
                case Tokens.Token.K_IF:
                    return ParseIf();
                case Tokens.Token.K_WHILE:
                    return ParseWhile();
                case Tokens.Token.K_CONTINUE:
                    if (!isCycle) ConsoleHelper.WriteErrorAST("No external loop to interrupt or continue", CurTok.y, CurTok.x);
                    GetNextToken();
                    if (CurTok == null) return null;
                    if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                    return new ContinueAST();
                case Tokens.Token.K_BREAK:
                    if (!isCycle) ConsoleHelper.WriteErrorAST("No external loop to interrupt or continue", CurTok.y, CurTok.x);
                    GetNextToken();
                    if (CurTok == null) return null;
                    if(CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                    return new BreakAST();
                case Tokens.Token.K_RETURN:
                    return ParseReturn();
                case Tokens.Token.BRACE_R:
                    return null;
                default:
                    return null;
            }
            return null;
        }

        private static ASTNode ParseWhile()
        {
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.PARENTHESIS_L) ConsoleHelper.WriteErrorAST("Expected '('", CurTok.y, CurTok.x);
            ASTNode condition = ParseConditionIf();
            ASTNode bodyWhile = null;

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.BRACE_L)
            {
                bodyWhile = new BodyMethodAST(ParseBraceMethod(isCycleArea));
            }
            else
            {
                bodyWhile = MemberBodyForSemilicon();
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
            }
            return new WhileAST(condition, bodyWhile);
        }

        private static ASTNode ParseCrement()
        {
            string crement = CurTok.subString;
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Tokens.Token.ID) ConsoleHelper.WriteErrorAST("Expected 'type' for identificator in method declaration", CurTok.y, CurTok.x);
            return new CrementAST(crement, ParseId());
        }

        private static ASTNode ParseCrement(ASTNode id)
        {
            string crement = CurTok.subString;
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.OP)
                return ParseBinaryOperation(new CrementAST(crement, id));
            else return new CrementAST(crement, id);
        }

        private static ASTNode ParseId()
        {
            string idName = CurTok.subString;
            ASTNode id = new IdentificatorAST(idName);
            GetNextToken();
            if (CurTok == null) return null;

            // массив
            if (CurTok.token == Tokens.Token.BRACKET_L)
            {
                ASTNode memberBrackets = ParseBrackets(true);
                return new BracketsAST(memberBrackets);
            }

            if (CurTok.token == Tokens.Token.CREMENT) return ParseCrement(id);

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
                ConsoleHelper.WriteErrorAST("Expected something action with identificator", CurTok.y, CurTok.x);
                return null;
            }
        }

        private static ASTNode ParseArgInMethod(bool isCall)
        {
            if (CurTok.token == Tokens.Token.ID && isCall)
            {
                IdentificatorAST identificatorAST = new IdentificatorAST(CurTok.subString);
                return identificatorAST;
            }
            else if (CurTok.token == Tokens.Token.TYPE && !isCall)
            {
                return ParseType(false, false);
            }
            else if(CurTok.token == Tokens.Token.PARENTHESIS_R)
            {
                return null;
            }
            else ConsoleHelper.WriteErrorAST("Expected 'type' for identificator in method declaration", CurTok.y, CurTok.x);
            return null;
        }

        //---------------------
        // Парсер типа
        //---------------------

        private static ASTNode ParseType(bool isInit, bool isInClass)
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
                    GetNextToken();
                    if (CurTok == null) return null;
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
                    ASTNode expr = ParseInitID();
                    return new IdentificatorAST(typeId, idName, expr);
                }
                else
                {
                    return new IdentificatorAST(typeId, idName);
                }
            }
            else if(CurTok.token == Tokens.Token.PARENTHESIS_L && isInit)
            {
                string idName = CurTok.subString;
                GetNextToken();
                if (CurTok == null) return null;
                if(CurTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", CurTok.y, CurTok.x);
                return new IdentificatorAST(typeId, idName);
            }
            else
            {
                ConsoleHelper.WriteErrorAST("Expected 'identificator'", CurTok.y, CurTok.x);
                return null;
            }
        }

        //---------------------
        // Парсер идентификатора
        //---------------------

        private static ASTNode ParseInitID()
        {
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.K_NEW)
            {
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.TYPE) ConsoleHelper.WriteErrorAST("Expected 'type' during variable initialization", CurTok.y, CurTok.x);
                ASTNode exp = ParseType(true, false);
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                
                return new NewAST(exp);
            }
            else
            {
                if(CurTok.token != Tokens.Token.SEMILICON || CurTok.token != Tokens.Token.COMM)
                {
                    ASTNode leftNode = MemberBinOperation();
                    if (leftNode == null) return null;
                    if (CurTok == null) return null;
                    if (CurTok.token == Tokens.Token.OP)
                    {
                        return ParseBinaryOperation(leftNode);
                    }
                    else if(CurTok.token == Tokens.Token.PARENTHESIS_L)
                    {
                        ASTNode parenExpr = ParseParenthesisExpr();
                        if (parenExpr == null) return leftNode;
                        return ParseBinaryOperation(parenExpr);
                    }
                    else if (CurTok.token == Tokens.Token.SEMILICON) return leftNode;
                }
            }
            return null;
        }

        //---------------------
        // Парсер бинарных выражений
        //---------------------

        private static ASTNode ParseParenthesisExpr()
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
                    ASTNode parenExpr = ParseParenthesisExpr();
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
                        if(rightNodeExp == null) ConsoleHelper.WriteErrorAST("Expected 'x'", CurTok.y, CurTok.x);

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
                    return ParseParenthesisExpr();
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", CurTok.y, CurTok.x);
                    break;
            }
            return null;
        }

        private static ASTNode ParseBoolBinaryOperation(ASTNode leftNode)
        {
            if (CurTok.token != Tokens.Token.BOOL_OP) ConsoleHelper.WriteErrorAST("Expected 'bool_op'", CurTok.y, CurTok.x);

            string oldOp = CurTok.subString;//+
            ASTNode rightMember;

            GetNextToken();
            if (CurTok == null) return null;//(
            if (CurTok.token == Tokens.Token.PARENTHESIS_L)
            {
                GetNextToken();
                if (CurTok == null) return null;//5
                rightMember = MemberBinOperation();
                if (rightMember == null)
                {
                    rightMember = MemberBoolBinOperation();
                }

                if (CurTok == null) return null;//)
                if (CurTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", CurTok.y, CurTok.x);
            }
            else rightMember = MemberBinOperation();

            if (rightMember == null) ConsoleHelper.WriteErrorAST("Expected 'bool'", CurTok.y, CurTok.x);

            ASTNode binaryExpr;

            if (CurTok == null) return null;
            if (CurTok.token == Tokens.Token.BOOL_OP_AND || CurTok.token == Tokens.Token.BOOL_OP_OR)
            {
                binaryExpr = new BinaryExprAST(oldOp, leftNode, rightMember);
                return ParseBinaryOperation(binaryExpr);
            }
            return rightMember;
        }

        //---------------------
        // Парсер условной конструкции if
        //---------------------

        private static ASTNode ParseIf()
        {
            List<ConditionNodeAST> branching = new List<ConditionNodeAST>();
            GetNextToken();
            if (CurTok == null) return null;
            if(CurTok.token != Tokens.Token.PARENTHESIS_L) ConsoleHelper.WriteErrorAST("Expected '('", CurTok.y, CurTok.x);
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
                    bodyIf = new BodyMethodAST(ParseBraceMethod(isCycleArea));
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
                            bodyIf = new BodyMethodAST(ParseBraceMethod(isCycleArea));
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
        // Парсер цикла и условной конструкции For
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
                body = new BodyMethodAST(ParseBraceMethod(true));
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
                    return ParseType(false, false);
                case Tokens.Token.ID:
                    return ParseId();
                case Tokens.Token.CREMENT:
                    return ParseCrement();
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
                if (member == null) break;

                postcondition.Add(member);
                if (CurTok == null) return null;
                if (CurTok.token != Tokens.Token.COMM) break;
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
                    return ParseId();
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
            if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
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
                    ASTNode id = ParseType(false, false);
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
            throw new NotImplementedException();
        }

        //---------------------
        // Парсер return
        //---------------------

        static string typeMethodReturn = "";

        private static ASTNode ParseReturn()
        {
            ASTNode memberReturn = ParseMemberReturn();
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

        private static ASTNode ParseMemberReturn()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Tokens.Token.ID:
                    ASTNode idNode = ParseId();
                    if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                    return idNode;
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
                    return null;
                case Tokens.Token.CREMENT:
                    ASTNode crement = ParseCrement();
                    if (CurTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", CurTok.y, CurTok.x);
                    return crement;
                default:
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
    }
}
