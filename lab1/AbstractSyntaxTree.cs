using System;
using System.Collections.Generic;

namespace lab1
{
    class TokenNode : Lexer
    {
        public TokenNode(Token token, string subString, int y, int x)
        {
            this.token = token;
            this.subString = subString;
            this.y = y + 1;
            this.x = x + 1;
        }

        public Token token;
        public string subString;
        public int y;
        public int x;
    }

    // PrototypeAST - Этот класс представляет "прототип" для функции,
    // который хранит её имя и имена аргументов (и, таким образом, 
    // неявно хранится число аргументов).
    class PrototypeAST : ASTNode
    {
        string Name;
        List<string> Args;
        public PrototypeAST(string Name, List<string> Args)
        {
            this.Name = Name;
            this.Args = Args;
        }
    }

    // FunctionAST - Представляет определение самой функции
    class FunctionAST : ASTNode
    {
        PrototypeAST Proto;
        ASTNode Body;
        public FunctionAST(PrototypeAST Proto, ASTNode Body)
        {
            this.Proto = Proto;
            this.Body = Body;
        }
    }

    // NumberExprAST - Класс узла выражения для числовых литералов (Например, "1.0").
    class NumberExprAST : ASTNode
    {
        double Val;
        public NumberExprAST(double Val)
        {
            this.Val = Val;
        }
    }

    // VariableExprAST - Класс узла выражения для переменных (например, "a").
    class VariableExprAST : ASTNode
    {
        string Name;
        public VariableExprAST(string Name)
        {
            this.Name = Name;
        }
    }

    // BinaryExprAST - Класс узла выражения для бинарных операторов.
    class BinaryExprAST : ASTNode
    {
        string Op;
        public ASTNode LeftNode;
        public ASTNode RightNode;
        public BinaryExprAST(string Op, ASTNode LeftNode, ASTNode RightNode)
        {
            this.Op = Op;
            this.LeftNode = LeftNode;
            this.RightNode = RightNode;
        }
    }

    // CallExprAST - Класс узла выражения для вызова функции.
    class CallExprAST : ASTNode
    {
        string Callee;
        List<ASTNode> Args;
        public CallExprAST(string Callee, List<ASTNode> Args)
        {
            this.Callee = Callee;
            this.Args = Args;
        }
    }
    class ASTNode
    {
        ~ASTNode() { }
    }

    class AbstractSyntaxTree
    {
        private static TokenNode CurTok;
        private static BinaryExprAST headATS;


        static void getNextToken()
        {
            //CurTok = Lexer.GetToken();
            if (CurTok == null) return;
        }

        static ASTNode ParsePrimary()
        {
            getNextToken();
            if (CurTok == null) return null;
            switch (CurTok.token)
            {
                case Lexer.Token.CHAR:
                    return null;
                case Lexer.Token.OP:
                    return ParseExpression();
                case Lexer.Token.NUM:
                    return null;
                case Lexer.Token.NUM_REAL:
                    return ParseNumberExpr(Convert.ToDouble(CurTok.subString));
                case Lexer.Token.ID:
                    return ParseIdentifierExpr(CurTok.subString);
                case Lexer.Token.TWINS:
                    return ParseParenExpr();
                case Lexer.Token.SEMILICON:
                    return null;
                case Lexer.Token.STRING:
                    return null;
                case Lexer.Token.NULL:
                    return null;
                case Lexer.Token.TYPE:
                    return null;
                case Lexer.Token.FAILED:
                    return null;
                default:
                    Error("unknown token when expecting an expression");
                    return null;
            }
        }

        public static void CreateAST()
        {
            ParseExpression();
        }

        private static void Error(string str)
        {
            Console.WriteLine("Error: {str}\n");
        }

        static NumberExprAST ParseNumberExpr(double NumVal)
        {
            NumberExprAST Result = new NumberExprAST(NumVal);
            return Result;
        }

        internal static void ViewAST()
        {
            DescentSide(headATS);
        }

        private static void DescentSide(BinaryExprAST nodeAST)
        {
            if(nodeAST.RightNode != null)
            {
                Console.WriteLine(nodeAST.RightNode);
                if (nodeAST.RightNode is BinaryExprAST)
                    DescentSide(nodeAST.RightNode as BinaryExprAST);
            }
            if (nodeAST.LeftNode != null)
            {
                Console.WriteLine(nodeAST.LeftNode);
                if (nodeAST.LeftNode is BinaryExprAST)
                    DescentSide(nodeAST.LeftNode as BinaryExprAST);
            }
        }

        /// parenexpr '(' expression ')'
        static ASTNode ParseParenExpr()
        {
            ASTNode V = ParseExpression();
            if (V == null) return null;

            if (CurTok.subString != ")")
                Error("expected ')'");
            return V;
        }

        /// identifierexpr
        ///    identifier
        ///    identifier '(' expression* ')'
        static ASTNode ParseIdentifierExpr(string IdName)
        {
            getNextToken();  // получаем идентификатор.
            if (CurTok == null) return null;

            if (CurTok.subString != "(") // Ссылка на переменную.
                return new VariableExprAST(IdName);

            // Вызов функции.
            getNextToken();  // получаем (
            if (CurTok == null) return null;
            List<ASTNode> Args = new List<ASTNode>();
            if (CurTok.subString != ")")
            {
                while (true)
                {
                    ASTNode Arg = ParseExpression();
                    if (Arg == null) return null;
                    Args.Add(Arg);

                    if (CurTok.subString == ")") break;

                    if (CurTok.subString != ",")
                        Error("Expected ')' or ',' in argument list");
                    getNextToken();
                    if (CurTok == null) return null;
                }
            }

            // получаем ')'.
            getNextToken();
            if (CurTok == null) return null;

            return new CallExprAST(IdName, Args);
        }

        /// BinopPrecedence - Содержит приоритеты для бинарных операторов
        static List<string> binopPrecedence = new List<string> { "<", "+", "-", "*" };

        /// GetTokPrecedence - Возвращает приоритет текущего бинарного оператора.
        static int GetTokPrecedence()
        {
            for (int i = 0; i < binopPrecedence.Count; i++)
            {
                if (CurTok.subString == binopPrecedence[i])
                    return i;
            }
            return 0;
        }

        static ASTNode ParseExpression()
        {
            ASTNode LeftNode = ParsePrimary();
            if (LeftNode == null) return null;

            return ParseBinOpRHS(0, LeftNode);
        }

        /// binoprhs
        ///    ('+' primary)*
        static ASTNode ParseBinOpRHS(int ExprPrec, ASTNode LeftNode)
        {
            // Если это бинарный оператор, получаем его приоритет
            while (true)
            {
                int TokPrec = GetTokPrecedence();

                // Если этот бинарный оператор связывает выражения по крайней мере так же, 
                // как текущий, то используем его
                if (TokPrec < ExprPrec)
                    return LeftNode;
                // Отлично, мы знаем, что это бинарный оператор.
                string BinOp = CurTok.subString;

                // Разобрать первичное выражение после бинарного оператора
                ASTNode RightNode = ParsePrimary();
                if (RightNode == null) return null;
                // Если BinOp связан с RHS меньшим приоритетом, чем оператор после RHS, 
                // то берём часть вместе с RHS как LHS.
                int NextPrec = GetTokPrecedence();
                if (TokPrec < NextPrec)
                {
                    RightNode = ParseBinOpRHS(TokPrec + 1, RightNode);
                    if (RightNode == null) return null;
                }

                // Собираем LHS/RHS.
                LeftNode = new BinaryExprAST(BinOp, LeftNode, RightNode);
            }
        }

        /// prototype
        ///    id '(' id* ')'
        static PrototypeAST ParsePrototype()
        {
            if (CurTok.token != Lexer.Token.ID)
                Error("Expected function name in prototype");

            string FnName = CurTok.subString;
            getNextToken();
            if (CurTok == null) return null;

            if (CurTok.subString != "(")
                Error("Expected '(' in prototype");


            // Считываем список наименований аргументов.
            List<string> ArgNames = new List<string>();
            getNextToken();
            if (CurTok == null) return null;
            while (CurTok.token == Lexer.Token.ID)
            {
                ArgNames.Add(CurTok.subString);
                getNextToken();
            }
            if (CurTok.subString != ")")
                Error("Expected ')' in prototype");

            return new PrototypeAST(FnName, ArgNames);
        }

        /// definition  'def' prototype expression
        static FunctionAST ParseDefinition()
        {
            getNextToken();  // получаем def.
            if (CurTok == null) return null;
            PrototypeAST Proto = ParsePrototype();
            if (Proto == null) return null;

            ASTNode exp = ParseExpression();
            if (exp == null)
                return new FunctionAST(Proto, exp);
            return null;
        }

        /// external  'extern' prototype
        static PrototypeAST ParseExtern()
        {
            getNextToken();  // получаем extern.
            if (CurTok == null) return null;
            return ParsePrototype();
        }

        /// toplevelexpr  expression
        static FunctionAST ParseTopLevelExpr()
        {
            ASTNode exp = ParseExpression();
            if (exp != null)
            {
                // Создаём анонимный прототип.
                List<string> args = new List<string>();
                PrototypeAST Proto = new PrototypeAST(CurTok.subString, args);
                return new FunctionAST(Proto, exp);
            }
            return null;
        }


        /*
        static int lineMemmory = -1;
        static int indexMemmory = 0;
        static string line = "";
         public static TokenNode GetToken()
        {
            for (; lineMemmory < stringWithoutComm.Length;)
            {
                if (indexMemmory == line.Length)
                {
                    lineMemmory++;
                    indexMemmory = 0;
                    if (lineMemmory == stringWithoutComm.Length) return null;
                }
                Console.WriteLine(stringWithoutComm[lineMemmory]);

                line = StringTreatment.FormatStroke(stringWithoutComm[lineMemmory]);
                List<char> buildToken = new List<char>();
                Token tokenType = Token.NULL;
                string findToken = "";
                int countQuotes = 0;
                bool isFindString = false;

                for (; indexMemmory < line.Length; indexMemmory++)
                {
                    if (line[indexMemmory] == '"' || isFindString)// обработка строки в кавычках
                    {
                        isFindString = true;
                        buildToken.Add(line[indexMemmory]);
                        if (line[indexMemmory] == '"') countQuotes++;
                        if (countQuotes == 2)
                        {
                            findToken = new string(buildToken.ToArray());
                            return CreateNodeToken(Token.STRING, findToken);
                        }
                    }
                    else if (line[indexMemmory] != ' ')//обработка токена
                    {
                        buildToken.Add(line[indexMemmory]);
                        findToken = new string(buildToken.ToArray());
                        tokenType = FindToken(findToken);
                    }
                    else if (buildToken.Count != 0)
                    {
                        return CreateNodeToken(!isFindString ? tokenType : Token.FAILED, findToken);
                    }
                }
                if (isFindString)// проверка на закрытие найденной строки
                {
                    findToken = new string(buildToken.ToArray());
                    return CreateNodeToken(Token.FAILED, findToken);
                }
                else if (buildToken.Count != 0)
                {
                    return CreateNodeToken(!isFindString ? tokenType : Token.FAILED, findToken);
                }
                indexMemmory = 0;
            }
            return CreateNodeToken(Token.FAILED, "", lineMemmory, indexMemmory);
        }

        static string[] stringWithoutComm;
        */
    }
}
