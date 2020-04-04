using System;
using System.Collections.Generic;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class BodyMethodAST : ASTNode, IArea
    {
        private List<ASTNode> memberMethod;

        public BodyMethodAST(List<ASTNode> memberMethod)
        {
            this.memberMethod = memberMethod;
        }

        public List<ASTNode> GetMemberMethod()
        {
            return memberMethod;
        }

        public override void Print(string level)
        {
            Console.WriteLine(level + "[BODY]");
            
            for(int i = 0; i < memberMethod.Count; i++)
            {
                memberMethod[i].Print(level + "\t");
            }
        }

        public SymTableUse GetSymTable(string areaName, Dictionary<string, ASTNode> parentTable)
        {
            List<SymTableUse> nestedArea = new List<SymTableUse>();
            for (int i = 0; i < memberMethod.Count; i++)
            {
                if (memberMethod[i] is IStorage)
                    SetNewSymbol(parentTable, memberMethod[i]);
                else if (memberMethod[i] is IArea)
                    nestedArea.Add((memberMethod[i] as IArea).GetSymTable("", parentTable));
            }
            return new SymTableUse(areaName, parentTable, nestedArea);
        }

        private void SetNewSymbol(Dictionary<string, ASTNode> symTable, ASTNode aSTNode)
        {
            string name = (aSTNode as IdentificatorAST).GetName();
            if (symTable.ContainsKey(name))
            {

            }
            else symTable.Add(name, aSTNode);

            ASTNode node = (aSTNode as IdentificatorAST).GetStorage();
            if(node is BinaryExprAST)
            {

            }
            else if(node is TypeAST)
            {

            }
            else if(node is TypeAST)
            {

            }
            else if(node is TypeAST)
            {

            }
            else if(node is TypeAST)
            {

            }
            else if(node is TypeAST)
            {

            }
        }
    }
}
