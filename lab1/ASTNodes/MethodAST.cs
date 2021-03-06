﻿using System;
using System.Collections.Generic;
using lab1.Asm;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class MethodAST : ASTNode, IArea, IStorage, ISemantics
    {
        private readonly string typeMethod;
        private readonly List<ASTNode> argsMethod;
        private readonly BodyMethodAST bodyMethod;
        private readonly string nameMethod;

        public MethodAST(string typeMethod, string nameMethod, List<ASTNode> argsMethod, BodyMethodAST bodyMethod)
        {
            this.typeMethod = typeMethod;
            this.nameMethod = nameMethod;
            this.argsMethod = argsMethod;
            this.bodyMethod = bodyMethod;
        }

        public MethodAST(string typeMethod, string nameMethod, BodyMethodAST bodyMethod)
        {
            this.typeMethod = typeMethod;
            this.nameMethod = nameMethod;
            this.bodyMethod = bodyMethod;
        }

        public MethodAST(string nameMethod, List<ASTNode> argsMethod)
        {
            this.nameMethod = nameMethod;
            this.argsMethod = argsMethod;
        }

        public MethodAST(string nameMethod)
        {
            this.nameMethod = nameMethod;
        }

        public ASTNode GetParentNode(ASTNode node, ASTNode prevNode = null)
        {
            bodyMethod.parent = this;
            return bodyMethod.GetParentNode(node);
        }

        public ASTNode GetNextNode(ASTNode nodePrev)
		{
            return bodyMethod.GetNextNode(nodePrev);
        }

        public string GetTypeMethod()
        {
            return typeMethod;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[TYPE] " + typeMethod);
            Console.WriteLine(level + "[METHOD] " + nameMethod);
            if(argsMethod != null)
            {
                Console.WriteLine(level + "[ARGUMENTS] ");
                for (int i = 0; i < argsMethod.Count; i++)
                {
                    argsMethod[i].Print(level + "\t");
                }
            }
            bodyMethod.Print(level + "\t");
        }

        public SymTableUse GetSymTable(string nameParent, Dictionary<string, ASTNode> parentTable)
        {
            Dictionary<string, ASTNode> symTable = new Dictionary<string, ASTNode>(parentTable);
            if(argsMethod != null)
            {
                for (int i = 0; i < argsMethod.Count; i++)
                {
                    if (argsMethod[i] is IStorage)
                        (argsMethod[i] as IStorage).AddAllSymbolIn(symTable);
                }
            }
            return (bodyMethod as IArea).GetSymTable(nameMethod, symTable);
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (symTable.ContainsKey(nameMethod))
            {
                if (typeMethod != "") ConsoleHelper.WriteError(nameMethod + " - Variable is redeclared");
            }
            else symTable.Add(nameMethod, this);
        }

        public string GetTypeMember()
        {
            ViewMemberArea();
            return typeMethod;
        }

        public void ViewMemberArea()
        {
            if (bodyMethod is ISemantics)
                (bodyMethod as ISemantics).GetTypeMember();
            else if (bodyMethod is IArea)
                (bodyMethod as IArea).ViewMemberArea();
        }

		public override void PrintASM(string levelTabulatiion, bool isNewLine = false)
		{
			bodyMethod.PrintASM("\t\t", false);
		}
	}
}
