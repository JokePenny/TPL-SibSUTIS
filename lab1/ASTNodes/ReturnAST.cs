using System;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class ReturnAST : ASTNode, IStorage, ISemantics
    {
        private readonly string typeReturn;
        private readonly ASTNode returnNode;
        
        public ReturnAST(string typeReturn, ASTNode returnNode)
        {
            this.typeReturn = typeReturn;
            this.returnNode = returnNode;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[RETURN] " + typeReturn);
            returnNode.Print(level + "\t");
        }

        public void AddAllSymbolIn(System.Collections.Generic.Dictionary<string, ASTNode> symTable)
        {
            if (returnNode is IStorage)
                (returnNode as IStorage).AddAllSymbolIn(symTable);
        }

        public string GetTypeMember()
        {
            if (returnNode is ISemantics)
            {
                string typeReturnNode = (returnNode as ISemantics).GetTypeMember();
                if (typeReturnNode != typeReturn) ConsoleHelper.WriteError("<" + point.y + "," + point.x + ">: Wrong type return: " + typeReturnNode + "'");
            }
            return typeReturn;
        }
	}
}
