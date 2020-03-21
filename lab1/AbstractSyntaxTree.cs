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
        public static void CreateAST()
        {
            GetNextToken();
            headAST = ParseNamespace();
        }

        private static void Error(string str)
        {
            Console.WriteLine("Error: {str}\n");
        }
        enum IDParseBrace : int
        {
            NAMESPACE = 0,
            CLASS,
            METHOD,
            CONDITIONAL,
            STRUCT,
            ENUM
        }

        //---------------------
        // Парсер скобок
        //---------------------

        private static ASTNode ParseBrackets(bool isInit)
        {
            if (isInit)
            {
                ASTNode exp = null;
                ASTNode exprNode = ParseBinaryOperation(exp);
                if (CurTok == null) return null;
                if (CurTok.token != Lexer.Token.BRACKET_R) Error("Expected ']'");
                return new BracketsAST(exprNode);
            }
            else
            {
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token != Lexer.Token.BRACKET_R) Error("Expected ']'");
                return new BracketsAST();
            }
        }

        private static List<ASTNode> MemberBraceArea(IDParseBrace typeParse)
        {
            switch (typeParse)
            {
                case IDParseBrace.NAMESPACE:
                    return ParseBraceNamespace();
                case IDParseBrace.CLASS:
                    return ParseBraceClass();
                default:
                    return null;
            }
        }

        //---------------------
        // Парсер области имен
        //---------------------

        private static ASTNode ParseNamespace()
        {
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Lexer.Token.K_NAMESPACE) Error("Expected 'namespace'");

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Lexer.Token.ID) Error("Expected 'identificator'");

            string idName = CurTok.subString;

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Lexer.Token.BRACE_L) Error("Expected '{'");

            List<ASTNode> memberNamespace = MemberBraceArea(IDParseBrace.NAMESPACE);

            if (CurTok == null) return null;
            if (CurTok.token != Lexer.Token.BRACE_R) Error("Expected '}'");

            return new NamespaceAST(memberNamespace, idName);
        }

        private static List<ASTNode> ParseBraceNamespace()
        {
            List<ASTNode> memmbersNamespace = new List<ASTNode>();
            while (true)
            {
                ASTNode memmberNamespace = MemmberNamespace();

                if (CurTok.token == Lexer.Token.BRACE_R || memmberNamespace == null) break; // null

                memmbersNamespace.Add(memmberNamespace);
            }
            return memmbersNamespace;
        }

        private static ASTNode MemmberNamespace()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Lexer.Token.BRACE_R: // найден конец области имен (если все правильно)
                    return null;
                case Lexer.Token.ACCESS_MODIFIER: // модификатор доступа
                    return null; 
                case Lexer.Token.TYPE: // тип
                    return null;
                case Lexer.Token.K_CLASS: // класс
                    return ParseClass();
                default:
                    Error("Expected");
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
            if (CurTok.token != Lexer.Token.ID) Error("Expected 'identificator'");

            string idName = CurTok.subString;

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Lexer.Token.BRACE_L) Error("Expected '{'");

            List<ASTNode> memberClass = MemberBraceArea(IDParseBrace.CLASS);

            if (CurTok == null) return null;
            if (CurTok.token != Lexer.Token.BRACE_R) Error("Expected '}'");

            return new ClassAST(memberClass, idName);
        }

        private static List<ASTNode> ParseBraceClass()
        {
            List<ASTNode> memmbersClass = new List<ASTNode>();
            while (true)
            {
                ASTNode memmberClass = MemmberClass();

                if (CurTok == null) break;
                if (CurTok.token == Lexer.Token.BRACE_R) break; // null

                memmbersClass.Add(memmberClass);
            }
            return memmbersClass;
        }

        private static ASTNode MemmberClass()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Lexer.Token.ACCESS_MODIFIER:
                    return null;
                case Lexer.Token.TYPE:
                case Lexer.Token.K_CLASS:
                    return ParseType(false, true);
                default:
                    ConsoleHelper.WriteError("Error: unknown token when expecting an expression\n");
                    return null;
            }
        }

        //---------------------
        // Парсер типа
        //---------------------
        
        private static ASTNode ParseType(bool isInit, bool isInClass)
        {
            TypeAST typeId = new TypeAST(CurTok.subString);

            GetNextToken();
            if (CurTok == null) return null;

            // массив
            if (CurTok.token == Lexer.Token.BRACKET_L)
            {
                ASTNode memberBrackets = ParseBrackets(isInit);
                if (isInit)
                {

                }
                else
                {
                    GetNextToken();
                    string idName = CurTok.subString;
                    return new IdentificatorAST(typeId, idName);
                }
            }
            // переменная
            else if(CurTok.token == Lexer.Token.ID && !isInit)
            {
                string idName = CurTok.subString;
                GetNextToken();
                if (CurTok == null) return null;

                if (CurTok.token == Lexer.Token.PARENTHESIS_L) // МЕТОД
                {
                    if (isInClass) return ParseMethod(typeId, CurTok.subString, false); // обявление в классе
                    else return null; // ошибка при объявлении в методе
                }
                else if (CurTok.token == Lexer.Token.ASSIGNMENT)
                {
                    ASTNode expr = ParseInitID(typeId);
                    return new IdentificatorAST(typeId, idName, expr);
                }
                else
                {
                    return new IdentificatorAST(typeId, idName);
                }
            }
            else if(CurTok.token == Lexer.Token.PARENTHESIS_L && isInit)
            {
                string idName = CurTok.subString;
                GetNextToken();
                if (CurTok == null) return null;
                if(CurTok.token != Lexer.Token.PARENTHESIS_R) Error("Expected ')'");
                return new IdentificatorAST(typeId, idName);
            }
            else
            {
                Error("Expected identificator");
                return null;
            }
            return null;
        }

        //---------------------
        // Парсер идентификатора
        //---------------------

        private static ASTNode ParseInitID(ASTNode id)
        {
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Lexer.Token.K_NEW)
            {
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token != Lexer.Token.TYPE) Error("Expected 'type'");
                return ParseType(true, true);
            }
            else
            {
                if(CurTok.token != Lexer.Token.SEMILICON)
                {
                    ASTNode leftNode = MemberBinOperation();
                    if (leftNode == null) return null;
                    GetNextToken();
                    if (CurTok == null) return null;
                    if (CurTok.token == Lexer.Token.OP)
                    {
                        return ParseBinaryOperation(leftNode);
                    }
                    else if(CurTok.token == Lexer.Token.PARENTHESIS_L)
                    {
                        ASTNode parenExpr = ParseParenthesisExpr();
                        if (parenExpr == null) return null;
                        return ParseBinaryOperation(parenExpr);
                    }
                    else if (CurTok.token != Lexer.Token.SEMILICON) return null;
                }
            }
            return null;
        }

        //---------------------
        // Парсер бинарных выражений
        //---------------------

        private static ASTNode ParseParenthesisExpr()
        {
            ASTNode exprassion = null;
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token != Lexer.Token.SEMILICON && CurTok.token != Lexer.Token.PARENTHESIS_R)
            {
                ASTNode leftNode = MemberBinOperation();
                if (leftNode == null) return null;
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token == Lexer.Token.OP)
                {
                    exprassion = ParseBinaryOperation(leftNode);
                    return new ParenthesisExprAST(exprassion);
                }
                else if (CurTok.token == Lexer.Token.PARENTHESIS_L)
                {
                    ASTNode parenExpr = ParseParenthesisExpr();
                    if (parenExpr == null) return null;
                    exprassion = ParseBinaryOperation(parenExpr);
                    return new ParenthesisExprAST(exprassion);
                }
                else if (CurTok.token != Lexer.Token.SEMILICON) return null;
            }
            else
            {
                Error("Expected ')' or empty ()");
            }
            return null;
        }

        private static ASTNode MemberBinOperation()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Lexer.Token.ID:
                    return new IdentificatorAST(CurTok.subString);
                case Lexer.Token.NUM:
                    return new NumberExprAST(Convert.ToDouble(CurTok.subString));
                case Lexer.Token.NUM_REAL:
                    break;
                default:
                    break;
            }
            return null;
        }

        private static Dictionary<string, int> opPriority = new Dictionary<string, int>()
        {
            {"<=", 0}, {">=", 0},
            {"<", 0}, {">", 0},
            {"!=", 0}, {"==", 0},
            {"||", 0}, {"&&", 0},
            {"^", 0},
            {"+", 1}, {"-", 1},
            {"*", 2}, {"/", 2}
        };

        private static ASTNode ParseBinaryOperation(ASTNode leftNode)
        {
            string oldOp = CurTok.subString;
            int oldPriority = opPriority[oldOp];

            GetNextToken();
            if (CurTok == null) return null;
            ASTNode rightMember = MemberBinOperation();

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Lexer.Token.OP)
            {
                string newOp = CurTok.subString;
                int newPriority = opPriority[newOp];

                if (oldPriority > newPriority)
                {
                    ASTNode binaryExpr = new BinaryExprAST(oldOp, leftNode, rightMember);
                    return ParseBinaryOperation(binaryExpr);
                }
                else
                {
                    ASTNode rightNode = ParseBinaryOperation(rightMember);
                    ASTNode binaryExpr = new BinaryExprAST(newOp, leftNode, rightNode);
                    return ParseBinaryOperation(binaryExpr);
                }
            }
            else if (CurTok.token == Lexer.Token.SEMILICON)
            {
                ASTNode binaryExpr = new BinaryExprAST(oldOp, leftNode, rightMember);
                return binaryExpr;
            }
            else Error("Expected ';'");
            return null;
        }

        //---------------------
        // Парсер метода
        //---------------------

        private static ASTNode ParseMethod(TypeAST typeId, string idName, bool isCall)
        {
            List<ASTNode> argsMethod = new List<ASTNode>();

            GetNextToken();
            if (CurTok == null) return null;

            while (true)
            {
                if (argsMethod.Count >= 1)
                {
                    if (CurTok.token == Lexer.Token.COMM)
                    {
                        ASTNode argMethod = ParseArgInMethod(isCall);
                        if(argMethod != null) argsMethod.Add(argMethod);
                    }
                    else Error("Expected 'COMM'");
                }
                else
                {
                    ASTNode argMethod = ParseArgInMethod(isCall);
                    if (argMethod != null) argsMethod.Add(argMethod);
                }

                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token == Lexer.Token.PARENTHESIS_R) break;
            }

            if (argsMethod.Count > 0)
            {
                if (isCall)
                {
                    return new MethodAST(idName, argsMethod);
                }
                else
                {
                    BodyMethodAST bodyMethod = ParseBodyMethod();
                    return new MethodAST(typeId, idName, argsMethod, bodyMethod);
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
                    BodyMethodAST bodyMethod = ParseBodyMethod();
                    return new MethodAST(typeId, idName, bodyMethod);
                }
            }
        }

        private static BodyMethodAST ParseBodyMethod()
        {
            List<ASTNode> membersBodyMethod = new List<ASTNode>();

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Lexer.Token.BRACE_L) Error("Expected '{'");

            while (true)
            {
                ASTNode member = MemmberBodyMethod();
                if (member == null) return null;

                membersBodyMethod.Add(member);
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token == Lexer.Token.BRACE_R) break;
            }
            return new BodyMethodAST(membersBodyMethod);
        }
        
        private static ASTNode MemmberBodyMethod()
        {
            while (true)
            {
                GetNextToken();
                if (CurTok == null) return null;
                switch (CurTok.token)
                {
                    case Lexer.Token.SEMILICON:
                        break;
                    case Lexer.Token.TYPE:
                        ParseType(false, false);
                        return null;
                    case Lexer.Token.ID:
                        break;
                    case Lexer.Token.INCREMENT:
                        break;
                    case Lexer.Token.DECREMENT:
                        break;
                    case Lexer.Token.K_FOR:
                        break;
                    case Lexer.Token.K_IF:
                        break;
                    case Lexer.Token.K_ELSE:
                        break;
                    case Lexer.Token.K_RETURN:
                        break;
                    default:
                        break;
                }
            }
        }

        private static ASTNode ParseArgInMethod(bool isCall)
        {
            if (CurTok.token == Lexer.Token.ID && isCall)
            {
                IdentificatorAST identificatorAST = new IdentificatorAST(CurTok.subString);
                return identificatorAST;
            }
            else if (CurTok.token == Lexer.Token.TYPE && !isCall)
            {
                TypeAST typeAST = new TypeAST(CurTok.subString);

                GetNextToken();
                if (CurTok == null) Error("Expected");
                if (CurTok.token == Lexer.Token.ID)
                {
                    IdentificatorAST identificatorAST = new IdentificatorAST(typeAST, CurTok.subString);
                    return identificatorAST;
                }
                else Error("Expected 'identificator'");

            }
            else Error("Expected 'type'");
            return null;
        }

        //---------------------
        // Парсер условной конструкции if
        //---------------------

        private ASTNode ParseIf()
        {
            List<ASTNode> branching = new List<ASTNode>();
            GetNextToken();
            if (CurTok == null) return null;
            bool isCondition = true;
            while (true)
            {
                ASTNode condition = null;
                if (CurTok.token == Lexer.Token.PARENTHESIS_L && isCondition)
                {
                    condition = ParseConditionIf();
                }
                else Error("Expected '('");
                isCondition = false;

                ASTNode bodyIf = ParseBodyMethod();
                branching.Add(new ConditionNodeAST(condition, bodyIf));

                if (CurTok.token != Lexer.Token.K_ELSE) break;
                else
                {
                    GetNextToken();
                    if (CurTok == null) return null;
                    if(CurTok.token == Lexer.Token.K_IF) isCondition = true;
                }
            }
            return new IfNodeAST(branching);
        }

        private ASTNode ParseConditionIf()
        {
            ASTNode expr = null;
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Lexer.Token.PARENTHESIS_L) expr = ParseParenthesisExpr();

            ASTNode bodyCondition = ParseBodyConditionIf(expr);
            if (bodyCondition == null && expr == null) Error("Expected '( X )'");
            if (CurTok == null) return null;
            if (CurTok.token == Lexer.Token.PARENTHESIS_R)
            {
                return bodyCondition;
            }
            Error("Expected ')'");
            return null;
        }

        private ASTNode ParseBodyConditionIf(ASTNode expr)
        {
            ASTNode leftMember = expr ?? MemberBoolBinOperation(); // 5

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Lexer.Token.OP) // <
            {
                string oldOp = CurTok.subString;
                ASTNode rightMember = ParseBodyConditionIf(leftMember);
                ASTNode binaryExpr = new BinaryExprAST(oldOp, leftMember, rightMember);
                return ParseBodyConditionIf(binaryExpr);
            }
            else if (CurTok.token == Lexer.Token.PARENTHESIS_R)
            {
                return leftMember;
            }
            else Error("Expected ')'");
            return null;
        }

        private ASTNode MemberBoolBinOperation()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Lexer.Token.ID:
                    return new IdentificatorAST(CurTok.subString);
                case Lexer.Token.NUM:
                    break;
                case Lexer.Token.NUM_REAL:
                    break;
                case Lexer.Token.BOOL:
                    break;
                case Lexer.Token.PARENTHESIS_L:
                    break;
                default:
                    break;
            }
            return null;
        }

        //---------------------
        // Парсер цикла и условной конструкции For
        //---------------------

        private ASTNode ParseFor()
        {
            GetNextToken();
            if (CurTok == null) return null;

            List<ASTNode> declaredVar = new List<ASTNode>();
            ASTNode condition = null;
            ASTNode postCondition = null;
            ASTNode body = null;

            if (CurTok.token == Lexer.Token.PARENTHESIS_L)
            {
                declaredVar = ParseDeclaredVarInFor();
                condition = ParseConditionFor();
                postCondition = ParsePostConditionFor();
            }
            else Error("Expected '('");

            ASTNode bodyFor = ParseBodyMethod();
            return new BodyConditionForAST(declaredVar, new ConditionNodeAST(condition, body), postCondition);
        }

        private ASTNode ParseBodyFor()
        {
            List<ASTNode> memberFor = new List<ASTNode>();

            while (true)
            {
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token == Lexer.Token.BRACE_R) break;

                ASTNode member = ParseBodyMethod();
                if (member != null)
                {
                    memberFor.Add(member);
                }
                else return null;
            }
            return new BodyMethodAST(memberFor);
        }

        private ASTNode MemberBodyForSemilicon()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Lexer.Token.TYPE:
                    return ParseType(false, false);
                case Lexer.Token.ID:
                    //TODO: check in hashtable
                    return null;
                case Lexer.Token.INCREMENT:
                    break;
                case Lexer.Token.DECREMENT:
                    break;
                case Lexer.Token.K_FOR:
                    break;
                case Lexer.Token.K_IF:
                    break;
            }
            return null;
        }

        private ASTNode ParsePostConditionFor()
        {
            List<ASTNode> postcondition = new List<ASTNode>();
            while (true)
            {
                ASTNode member = MemberPosconditionFor();
                if (member == null) return null;

                postcondition.Add(member);
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token == Lexer.Token.PARENTHESIS_R || CurTok.token != Lexer.Token.COMM) break;
            }
            return new PostconditionForAST(postcondition);
        }

        private ASTNode MemberPosconditionFor()
        {
            GetNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Lexer.Token.ID:
                    return null;
                case Lexer.Token.INCREMENT:
                    return null;
                case Lexer.Token.DECREMENT:
                    return null;
            }
            Error("Expect ''");
            return null;
        }

        private ASTNode ParseConditionFor()
        {
            ASTNode condition = null;
            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Lexer.Token.PARENTHESIS_L) condition = ParseParenthesisExpr();

            ASTNode bodyCondition = ParseBodyConditionFor(condition);

            if (bodyCondition == null && condition == null) Error("Expected '( X )'");
            if (CurTok == null) return null;
            if (CurTok.token == Lexer.Token.PARENTHESIS_R)
            {
                return bodyCondition;
            }
            Error("Expected ')'");

            return bodyCondition;
        }

        

        private List<ASTNode> ParseDeclaredVarInFor()
        {
            List<ASTNode> declaredVar = new List<ASTNode>();
            TypeAST generalType = null;
            bool isComm = true;
            bool isFirst = true;
            while (true)
            {
                GetNextToken();
                if (CurTok == null) return null;
                if (CurTok.token == Lexer.Token.TYPE && isComm && isFirst)
                {
                    generalType = new TypeAST(CurTok.subString);
                    ASTNode id = ParseType(false, false);
                    declaredVar.Add(id);

                    GetNextToken();
                    if (CurTok == null) return null;
                    isComm = CurTok.token == Lexer.Token.COMM;
                }
                else if (CurTok.token == Lexer.Token.ID && isComm)
                {
                    isFirst = false;
                    ASTNode id = null;
                    string idName = CurTok.subString;
                    GetNextToken();
                    if (CurTok == null) return null;
                    if (CurTok.token == Lexer.Token.ASSIGNMENT)
                    {
                        TypeAST typeId = GetTypeID();

                        if (typeId == null && generalType == null)
                        {
                            Error("ID not init");
                            return null;
                        }
                        else if(typeId == null && generalType != null && !isFirst)
                        {
                            ASTNode expr = ParseInitID(typeId);
                            id = new IdentificatorAST(typeId, idName, expr);
                            declaredVar.Add(id);
                        }
                        else
                        {
                            Error("ID not init");
                            return null;
                        }
                    }
                    else 
                    {
                        Error("Expected '=");
                    }
                    GetNextToken();
                    if (CurTok == null) return null;
                    isComm = CurTok.token == Lexer.Token.COMM;
                }
                else if (CurTok.token == Lexer.Token.SEMILICON)
                {
                    break;
                }
                else
                {
                    Error("Ecpected 'type' or 'id'");
                    return null;
                }
            }
            return declaredVar;
        }

        private TypeAST GetTypeID()
        {
            throw new NotImplementedException();
        }

        private ASTNode ParseBodyConditionFor(ASTNode expr)
        {
            ASTNode leftMember = expr ?? MemberBoolBinOperation();

            GetNextToken();
            if (CurTok == null) return null;
            if (CurTok.token == Lexer.Token.OP)
            {
                string oldOp = CurTok.subString;
                ASTNode rightMember = ParseBodyConditionFor(leftMember);
                ASTNode binaryExpr = new BinaryExprAST(oldOp, leftMember, rightMember);
                return ParseBodyConditionIf(binaryExpr);
            }
            else if (CurTok.token == Lexer.Token.SEMILICON)
            {
                return leftMember;
            }
            else Error("Expected ';'");
            return null;
        }

        //---------------------
        // Парсер выражений
        //---------------------

        //---------------------
        // Парсер структуры
        //---------------------

        //---------------------
        // Парсер перечислителя
        //---------------------


        //---------------------
        // Возвращает следующий токен
        //---------------------

        private static void GetNextToken()
        {
            CurTok = Lexer.GetToken();
        }
    }
}
