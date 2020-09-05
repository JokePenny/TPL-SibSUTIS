using lab1.ASTNodes;
using lab1.Helpers;
using lab1.SymbolTable;
using lab1.SemAnalyz;
using System.Collections.Generic;
using System;
using lab1.Asm;

namespace lab1
{
    public sealed class AbstractSyntaxTree
    {
        private static TokenNode curTok;
        private static TokenNode bufferTok;
        private static ASTNode headAST;
        private static bool isCycleArea = false;
        private static bool isRawToken = false;

        public static void CreateAST()
        {
            GetNextToken();
            CheckupClosedToken(Tokens.Token.K_NAMESPACE);

            headAST = ParseMainArea(Area.NAMESPACE);
            if (headAST != null)
            {
                headAST.Print("");
                SymTable.CreateSymTable(headAST);
                SemAnalyzer.StartSemAnalyzer(headAST, SymTable.symTabls);
				ASM.CreateASM(headAST);
			}
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

                if (curTok.token == Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected '[X]'", curTok.y, curTok.x);
                
                ASTNode exp = MemberBinOperation();

                if(curTok.token == Tokens.Token.BRACKET_L)
                {
                    exp = new BracketsAST(exp, ParseBrackets(true));
                }

                brackets = exp;
                if (curTok.token == Tokens.Token.OP)
                {
                    brackets = ParseBinaryOperation(exp);
                }
                else if (curTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", curTok.y, curTok.x);
            }
            else
            {
                GetNextToken();
                if (curTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", curTok.y, curTok.x);
                brackets = new BracketsAST();
            }
            GetNextToken();
            return brackets;
        }

        //---------------------
        // Парсер скобки {
        //---------------------

        public enum Area : int
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
                if (curTok.token == Tokens.Token.BRACE_R && member == null) break;
            }
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
            ASTNode node;
            if (!isRawToken)
            {
                GetNextToken();
            }
            else isRawToken = false;
            switch (curTok.token)
            {
                case Tokens.Token.SEMILICON:
                    break;
                case Tokens.Token.TYPE:
                    node = ParseType(false, false);
                    CheckupClosedToken(Tokens.Token.SEMILICON);
                    return node;
                case Tokens.Token.ID:
                    node = ParseId("", false);
                    CheckupClosedToken(Tokens.Token.SEMILICON);
                    return node;
                case Tokens.Token.CREMENT:
                    node = ParseCrement();
                    CheckupClosedToken(Tokens.Token.SEMILICON);
                    return node;
                case Tokens.Token.K_FOR:
                    return ParseFor();
                case Tokens.Token.K_IF:
                    bufferTok = curTok;
                    return ParseIf();
                case Tokens.Token.K_ELSE:
                    if(bufferTok.token != Tokens.Token.K_IF) ConsoleHelper.WriteErrorAST("No condition if", curTok.y, curTok.x);
                    return ParseElse();
                case Tokens.Token.K_WHILE:
                    return ParseWhile();
                case Tokens.Token.K_CONTINUE:
                case Tokens.Token.K_BREAK:
                    if (!isCycle) ConsoleHelper.WriteErrorAST("No external loop to interrupt or continue", curTok.y, curTok.x);
                    if (curTok.token == Tokens.Token.K_BREAK) node = new BreakAST();
                    else node = new ContinueAST();

                    GetNextToken();
                    CheckupClosedToken(Tokens.Token.SEMILICON);
                    return node;
                case Tokens.Token.K_RETURN:
                    node = ParseReturn();
                    CheckupClosedToken(Tokens.Token.SEMILICON);
                    return node;
				case Tokens.Token.CONSOLE:
					node = ParseConsoleCommand();
					GetNextToken();
					CheckupClosedToken(Tokens.Token.SEMILICON);
					return node;
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
            switch (curTok.token)
            {
                case Tokens.Token.TYPE:
                    return ParseType(false, true);
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
            if (curTok.token != Tokens.Token.SEMILICON && curTok.token != Tokens.Token.PARENTHESIS_R)
            {
                ASTNode leftNode = MemberBinOperation();
                if (leftNode == null) return null;
                if (curTok.token == Tokens.Token.OP)
                {
                    exprassion = ParseBinaryOperation(leftNode);
                    if(curTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", curTok.y, curTok.x);
                    return new ParenthesisExprAST(exprassion);
                }
                else if (curTok.token == Tokens.Token.PARENTHESIS_L)
                {
                    ASTNode parenExpr = ParseParenthesis();
                    if (parenExpr == null) return null;
                    exprassion = ParseBinaryOperation(parenExpr);
                    if (curTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", curTok.y, curTok.x);
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

        public static ASTNode ParseMainArea(Area area)
        {
            string idName;
            GetNextToken();
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

            if (curTok.token != Tokens.Token.BRACE_L) ConsoleHelper.WriteErrorAST("Expected '{'", curTok.y, curTok.x);

            List<ASTNode> memberArea = ParseBrace(area, false);

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
                IdentificatorAST identificatorAST = new IdentificatorAST(curTok.subString, new Point(curTok.y, curTok.x));
                return identificatorAST;
            }
            else if (curTok.token == Tokens.Token.TYPE && !isCall) return ParseType(false, false);
            else if (curTok.token == Tokens.Token.PARENTHESIS_R) return null;
            else ConsoleHelper.WriteErrorAST("Expected 'type' for identificator in method declaration", curTok.y, curTok.x);
            return null;
        }

        private static List<ASTNode> ParseArgsMethod(bool isCall)
        {
            List<ASTNode> argsMethod = new List<ASTNode>();
            GetNextToken();
            while (true)
            {
                if (argsMethod.Count >= 1)
                {
                    if (curTok.token == Tokens.Token.COMM)
                    {
                        ASTNode argMethod = ParseArgInMethod(isCall);
                        if (argMethod != null) argsMethod.Add(argMethod);
                        if (curTok.token == Tokens.Token.COMM) continue;
                        if (curTok.token != Tokens.Token.PARENTHESIS_R) break;
                    }
                    break;
                }
                else
                {
                    ASTNode argMethod = ParseArgInMethod(isCall);
                    if(argMethod != null) argsMethod.Add(argMethod);
                    break;
                }
            }
            CheckupClosedToken(Tokens.Token.PARENTHESIS_R);
            GetNextToken();
            return argsMethod;
        }

        public static ASTNode ParseMethod(string typeId, string idName, bool isCall)
        {
            List<ASTNode> argsMethod = ParseArgsMethod(isCall);

            List<ASTNode> bodyMethod = new List<ASTNode>();
            if (!isCall)
            {
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

        public static ASTNode ParseWhile()
        {
            ASTNode condition = null;
            ASTNode bodyWhile;
            GetNextToken();
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

            if (curTok.token == Tokens.Token.BRACE_L)
            {
                bodyWhile = new BodyMethodAST(ParseBrace(Area.METHOD, true));
            }
            else
            {
                bodyWhile = MemberMethod(true);
                CheckupClosedToken(Tokens.Token.SEMILICON);
            }
            return new WhileAST(condition, bodyWhile);
        }

        //---------------------
        // Парсер increment и decrement
        //---------------------

        private static ASTNode ParseCrement()
        {
            string crement = curTok.subString;
            GetNextToken();
            Point point = new Point(curTok.y, curTok.x);
            if (curTok.token != Tokens.Token.ID)
            {
                ConsoleHelper.WriteErrorAST("Expected 'identificator'", curTok.y, curTok.x);
                return null;
            }
            else return new CrementAST(crement, ParseId("", false), point);
        }

        private static ASTNode ParseCrement(ASTNode id)
        {
            string crement = curTok.subString;
            GetNextToken();
            Point point = new Point(curTok.y, curTok.x);
            if (curTok.token == Tokens.Token.OP) return ParseBinaryOperation(new CrementAST(crement, id, point));
            return new CrementAST(crement, id, point);
        }

        //---------------------
        // Парсер id
        //---------------------

        private static ASTNode ParseId(string type, bool isArray)
        {
            bool isCall = type == "";
            string idName = curTok.subString;
            string typeId = type == "" ? GetTypeID(idName) : type;

            ASTNode id = new IdentificatorAST(typeId, idName, new Point(curTok.y, curTok.x), isArray);
            GetNextToken();

            // массив
            if (curTok.token == Tokens.Token.BRACKET_L)
            {
                ASTNode memberBrackets = ParseBrackets(true);
                return new BracketsAST(memberBrackets);
            }

            Point point = new Point(curTok.y, curTok.x);
            if (curTok.token == Tokens.Token.CREMENT) return ParseCrement(id);
            if (curTok.token == Tokens.Token.PARENTHESIS_L) return ParseMethod("", idName, isCall);
            else if (curTok.token == Tokens.Token.ASSIGNMENT) return new IdentificatorAST(typeId, idName, ParseInitID(), point, isArray);
            return id;
        }

        private static ASTNode ParseInitID()
        {
            GetNextToken();
            if (curTok.token == Tokens.Token.K_NEW)
            {
                GetNextToken();
                if (curTok.token != Tokens.Token.TYPE) ConsoleHelper.WriteErrorAST("Expected 'type' during variable initialization", curTok.y, curTok.x);
                ASTNode exp = ParseType(true, false);
                if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);

                return new NewAST(exp);
            }
            else
            {
                if (curTok.token != Tokens.Token.SEMILICON || curTok.token != Tokens.Token.COMM)
                {
                    ASTNode leftNode = MemberBinOperation();
                    if (leftNode == null) return null;
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
                    return leftNode;
                }
            }
            return null;
        }

        //---------------------
        // Парсер типа
        //---------------------

        private static ASTNode ParseType(bool isInit, bool isInClass)
        {
            string typeId = curTok.subString;
			bool isArray = false;

            GetNextToken();

            // массив
            if (curTok.token == Tokens.Token.BRACKET_L)
            {
                if (isInit)
                {
                    ASTNode memberBrackets = ParseBrackets(isInit);
                    if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
                    return new BracketsAST(typeId, memberBrackets, new Point(curTok.y, curTok.x));
                }
                else
                {
					isArray = true;
					//typeId = typeId;
					GetNextToken();
                    if (curTok.token != Tokens.Token.BRACKET_R) ConsoleHelper.WriteErrorAST("Expected ']'", curTok.y, curTok.x);
                    else
                    {
                        GetNextToken();
                    }
                } 
            }
            // переменная
            if(curTok.token == Tokens.Token.ID && !isInit) return ParseId(typeId, isArray);
            else if(curTok.token == Tokens.Token.PARENTHESIS_L && isInit)
            {
                string idName = curTok.subString;
                GetNextToken();
                if (curTok.token != Tokens.Token.PARENTHESIS_R)
                {
                    ConsoleHelper.WriteErrorAST("Expected ')'", curTok.y, curTok.x);
                    return null;
                }
                return new IdentificatorAST(typeId, idName, new Point(curTok.y, curTok.x), isArray);
            }
            return null;
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
                    node = new IdentificatorAST(curTok.subString, new Point(curTok.y, curTok.x));
                    GetNextToken();
                    if (curTok.token == Tokens.Token.BRACKET_L) return new BracketsAST(node, ParseBrackets(true));
                    return node;
                case Tokens.Token.STRING:
                    node = new StringAST(curTok.subString);
                    break;
                case Tokens.Token.CHAR:
                    node = new CharAST(curTok.subString, new Point(curTok.y, curTok.x));
                    break;
                case Tokens.Token.PARENTHESIS_L:
                    node = ParseParenthesis();
                    break;
                case Tokens.Token.BOOL:
                    node = new BoolAST(curTok.subString, null, new Point(curTok.y, curTok.x));
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

        private static ASTNode MemberBoolBinOperation(bool isAndOp = false)
        {
            GetNextToken();
            switch (curTok.token)
            {
                case Tokens.Token.BOOL:
                    return new BoolAST(curTok.subString, new Point(curTok.y, curTok.x));
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
                    }
                    else leftNodeExp = leftNode;

                    if (curTok.token == Tokens.Token.BOOL_OP)
                    {
                        string op = curTok.subString;
                        GetNextToken();
                        ASTNode rightNodeExp = MemberBinOperation();
                        if (rightNodeExp == null) ConsoleHelper.WriteErrorAST("Expected 'x'", curTok.y, curTok.x);
						if(isAndOp)
						{
							ASTNode nodeExprLeft = new BinaryExprAST(op, leftNodeExp, rightNodeExp, new Point(curTok.y, curTok.x));
							ASTNode boolNode = new BoolAST(nodeExprLeft, new Point(curTok.y, curTok.x));
							return boolNode;
						}
						string opBool;
                        if (curTok.token == Tokens.Token.BOOL_OP_AND)
                        {
							opBool = curTok.subString;
							ASTNode nodeExprLeft = new BinaryExprAST(op, leftNodeExp, rightNodeExp, new Point(curTok.y, curTok.x));
							ASTNode boolNode = new BoolAST(nodeExprLeft, new Point(curTok.y, curTok.x));
							ASTNode nodeExpRight = new BinaryExprAST(opBool, boolNode, MemberBoolBinOperation(true), new Point(curTok.y, curTok.x));
							ASTNode nodeBool;
							while(curTok.token != Tokens.Token.PARENTHESIS_R)
							{
								string boolor = curTok.subString;
								ASTNode memberBoolOr = MemberBoolBinOperation(curTok.token == Tokens.Token.BOOL_OP_AND);
								nodeExpRight = new BinaryExprAST(boolor, memberBoolOr, nodeExpRight, new Point(curTok.y, curTok.x));
							}
							nodeBool = nodeExpRight;
							return new BoolAST(nodeBool, new Point(curTok.y, curTok.x));
						}
                        else if(curTok.token == Tokens.Token.BOOL_OP_OR)
						{
							opBool = curTok.subString;
							ASTNode nodeExprLeft = new BinaryExprAST(op, leftNodeExp, rightNodeExp, new Point(curTok.y, curTok.x));
							ASTNode boolNode = new BoolAST(nodeExprLeft, new Point(curTok.y, curTok.x));
							ASTNode nodeExpRight = new BinaryExprAST(opBool, boolNode, MemberBoolBinOperation(), new Point(curTok.y, curTok.x));
							return new BoolAST(nodeExpRight, new Point(curTok.y, curTok.x));
						}
						else
                        {
                            ASTNode nodeExpr = new BinaryExprAST(op, leftNodeExp, rightNodeExp, new Point(curTok.y, curTok.x));
                            return new BoolAST(nodeExpr, new Point(curTok.y, curTok.x));
                        }
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

            string oldOp = curTok.subString;
            int oldPriority = opPriority[oldOp];
            ASTNode rightMember;

            GetNextToken();
            if(curTok.token == Tokens.Token.PARENTHESIS_L)
            {
                rightMember = ParseParenthesis();
                GetNextToken();
            }
            else rightMember = MemberBinOperation();

            if(rightMember == null) ConsoleHelper.WriteErrorAST("Expected 'identificator'", curTok.y, curTok.x);

            ASTNode binaryExpr;

            if (curTok.token == Tokens.Token.OP)
            {
                string newOp = curTok.subString;
                int newPriority = opPriority[newOp];

                if (oldPriority > newPriority)
                {
                    binaryExpr = new BinaryExprAST(oldOp, leftNode, rightMember, new Point(curTok.y, curTok.x));
                    return ParseBinaryOperation(binaryExpr);
                }
                else
                {
                    Point point = new Point(curTok.y, curTok.x);
                    ASTNode rightNode = ParseBinaryOperation(rightMember);
                    binaryExpr = new BinaryExprAST(oldOp, leftNode, rightNode, point);
                    return binaryExpr;
                }
            }
            return new BinaryExprAST(oldOp, leftNode, rightMember, new Point(curTok.y, curTok.x));
        }

        //---------------------
        // Парсер if
        //---------------------
        
        public static ASTNode ParseIf()
        {
            ASTNode branching;
            GetNextToken();

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
            if(curTok.token == Tokens.Token.BRACE_L)
            {
                bodyIf = new BodyMethodAST(ParseBrace(Area.METHOD, isCycleArea));
                branching = new ConditionNodeAST(condition, bodyIf);
            }
            else
            {
                isRawToken = true;
                bodyIf = MemberMethod(isCycleArea);
                branching = new ConditionNodeAST(condition, bodyIf);
            }
            return new IfAST(branching);
        }

        private static ASTNode ParseElse()
        {
            ASTNode branching;
            ASTNode body;
            GetNextToken();

            if (curTok.token == Tokens.Token.K_IF) {
                bufferTok.token = Tokens.Token.K_ELSE_IF;
                return new ElseAST(ParseIf());
            }
            else if(curTok.token == Tokens.Token.BRACE_L) return new ElseAST(new BodyMethodAST(ParseBrace(Area.METHOD, isCycleArea)));
            else
            {
                if(bufferTok.token == Tokens.Token.K_ELSE) ConsoleHelper.WriteErrorAST("Expected 'else if' or 'if'", curTok.y, curTok.x);
                isRawToken = true;
                body = MemberMethod(false);
                branching = new ConditionNodeAST(null, body);
            }
            return new ElseAST(branching);
        }

        private static ASTNode ParseConditionIf()
        {
            ASTNode expr = MemberBoolBinOperation();
            if (expr == null)
            {
                ConsoleHelper.WriteErrorAST("Expected '( X )'", curTok.y, curTok.x);
                SkipToToken(Tokens.Token.PARENTHESIS_R);
            }
            CheckupClosedToken(Tokens.Token.PARENTHESIS_R);
            return expr;
        }

        //---------------------
        // Парсер for
        //---------------------

        public static ASTNode ParseFor()
        {
            GetNextToken();

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

            if(curTok.token != Tokens.Token.PARENTHESIS_R) ConsoleHelper.WriteErrorAST("Expected ')'", curTok.y, curTok.x);

            GetNextToken();
            if (curTok.token == Tokens.Token.BRACE_L)
                body = new BodyMethodAST(ParseBrace(Area.METHOD, true));
            else
            {
                body = MemberMethod(false);
                if (curTok.token != Tokens.Token.SEMILICON) ConsoleHelper.WriteErrorAST("Expected ';'", curTok.y, curTok.x);
            }
            return new ForAST(declaredVar, new ConditionNodeAST(condition, body), postCondition);
        }

        private static ASTNode ParsePostConditionFor()
        {
            List<ASTNode> postcondition = new List<ASTNode>();
            while (true)
            {
                ASTNode member = MemberPosconditionFor();
                if (member != null) postcondition.Add(member);

                if (curTok.token == Tokens.Token.PARENTHESIS_R) break;
                if (curTok.token != Tokens.Token.COMM)
                {
                    ConsoleHelper.WriteErrorAST("Expected ','", curTok.y, curTok.x);
                    SkipToToken(Tokens.Token.PARENTHESIS_R);
                    break;
                }
            }
            return new PostconditionForAST(postcondition);
        }

        private static ASTNode MemberPosconditionFor()
        {
            GetNextToken();
            switch (curTok.token)
            {
                case Tokens.Token.ID:
                    return ParseId("", false);
                case Tokens.Token.CREMENT:
                    Point point = new Point(curTok.y, curTok.x);
                    GetNextToken();
                    if(curTok.token != Tokens.Token.ID) ConsoleHelper.WriteErrorAST("Expected 'identificator'", curTok.y, curTok.x);
                    return new CrementAST(curTok.subString, new IdentificatorAST(curTok.subString, new Point(curTok.y, curTok.x)), point);
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token in this area", curTok.y, curTok.x);
                    break;
            }
            return null;
        }

        private static ASTNode ParseConditionFor()
        {
            ASTNode condition = MemberBoolBinOperation();
            CheckupClosedToken(Tokens.Token.SEMILICON);
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
                if (curTok.token == Tokens.Token.TYPE && isComm && isFirst)
                {
                    generalType = curTok.subString;
                    ASTNode id = ParseType(false, false);
                    if(id != null) declaredVar.Add(id);
                }
                else if (curTok.token == Tokens.Token.ID && isComm)
                {
                    isFirst = false;
                    ASTNode id = null;
                    string idName = curTok.subString;
                    GetNextToken();
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
                            Point point = new Point(curTok.y, curTok.x);
                            ASTNode expr = ParseInitID();
                            id = new IdentificatorAST(typeId, idName, expr, point, false);
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
            ASTNode node;
            GetNextToken();
            switch (curTok.token)
            {
                case Tokens.Token.ID:
                    node = ParseId("", false);
                    CheckupClosedToken(Tokens.Token.SEMILICON);
                    return node;
                case Tokens.Token.INT_VALUE:
                case Tokens.Token.DOUBLE_VALUE:
                case Tokens.Token.X16_VALUE:
                case Tokens.Token.X8_VALUE:
                case Tokens.Token.X2_VALUE:
                    NumAST value = new NumAST(curTok.token, curTok.subString);
                    GetNextToken();
                    if (curTok.token == Tokens.Token.OP) return ParseBinaryOperation(value);
                    CheckupClosedToken(Tokens.Token.SEMILICON);
                    return value;
                case Tokens.Token.CREMENT:
                    node = ParseCrement();
                    CheckupClosedToken(Tokens.Token.SEMILICON);
                    return node;
                default:
                    ConsoleHelper.WriteErrorAST("Impossible token this area", curTok.y, curTok.x);
                    return null;
            }
        }

		//---------------------
		// Парсер команд консоли
		//---------------------

		private static ASTNode ParseConsoleCommand()
		{
			GetNextToken();
			if (curTok.token == Tokens.Token.DOT)
			{
				GetNextToken();
				switch (curTok.token)
				{
					case Tokens.Token.CONSOLE_READLINE:
					case Tokens.Token.CONSOLE_READ_KEY:
						return ParseReadConsoleCommand();
					case Tokens.Token.CONSOLE_WRITE:
					case Tokens.Token.CONSOLE_WRITELINE:
						return ParseWriteConsoleCommand();
					default:
						ConsoleHelper.WriteErrorAST("Impossible token this area", curTok.y, curTok.x);
						return null;
				}
			}
			else
			{
				ConsoleHelper.WriteErrorAST("Impossible token this area", curTok.y, curTok.x);
				return null;
			}
		}

		private static ASTNode ParseWriteConsoleCommand()
		{
			Tokens.Token tokenCommand = curTok.token;
			GetNextToken();
			if (curTok.token == Tokens.Token.PARENTHESIS_L)
			{
				return new ConsoleCommandAST(tokenCommand, MemberWriteConsole());
			}
			else
			{
				ConsoleHelper.WriteErrorAST("Impossible token this area", curTok.y, curTok.x);
				return null;
			}
		}

		private static ASTNode ParseReadConsoleCommand()
		{
			Tokens.Token tokenCommand = curTok.token;
			GetNextToken();
			if (curTok.token == Tokens.Token.PARENTHESIS_L)
			{
				GetNextToken();
				if (curTok.token != Tokens.Token.PARENTHESIS_R)
				{
					ConsoleHelper.WriteErrorAST("Impossible token this area", curTok.y, curTok.x);
					return null;
				}
				return new ConsoleCommandAST(tokenCommand);
			}
			else
			{
				ConsoleHelper.WriteErrorAST("Impossible token this area", curTok.y, curTok.x);
				return null;
			}
		}

		private static ASTNode MemberWriteConsole()
		{
			GetNextToken();
			switch (curTok.token)
			{
				case Tokens.Token.ID:
					return new IdentificatorAST(curTok.subString, new Point(curTok.y, curTok.x));
				case Tokens.Token.STRING:
					return new StringAST(curTok.subString);
				case Tokens.Token.CHAR:
					return new CharAST(curTok.subString, new Point(curTok.y, curTok.x));
				case Tokens.Token.BOOL:
					return new BoolAST(curTok.subString, null, new Point(curTok.y, curTok.x));
				case Tokens.Token.INT_VALUE:
				case Tokens.Token.DOUBLE_VALUE:
				case Tokens.Token.X16_VALUE:
				case Tokens.Token.X8_VALUE:
				case Tokens.Token.X2_VALUE:
					return new NumAST(curTok.token, curTok.subString);
				default:
					ConsoleHelper.WriteErrorAST("Impossible token in this area", curTok.y, curTok.x);
					return null;
			}
		}

		//---------------------
		// Возвращает следующий токен
		//---------------------

		private static void GetNextToken()
        {
            curTok = Lexer.GetToken();
			if (curTok == null)
			{
				ConsoleHelper.WriteErrorAST("No more lexems", curTok.y, curTok.x);
				System.Environment.Exit(0);
			}
		}

        private static void GetNextTokenError()
        {
            curTok = Lexer.GetTokenError();
        }

        private static void CheckupClosedToken(Tokens.Token closedToken)
        {
            if (curTok.token != closedToken)
            {
                ConsoleHelper.WriteErrorAST("Expected '" + closedToken.ToString() + "'", curTok.y, curTok.x);
                SkipToToken(Tokens.Token.SEMILICON);
            }
        }

        private static void SkipToToken(Tokens.Token token)
        {
            while (curTok.token != token )
            {
                GetNextTokenError();
            }
        }
    }
}
