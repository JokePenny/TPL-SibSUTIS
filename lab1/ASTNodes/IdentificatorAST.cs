using System;
using System.Collections.Generic;
using lab1.Helpers;
using lab1.SemAnalyz;
using lab1.SymbolTable;

namespace lab1.ASTNodes
{
    class IdentificatorAST : ASTNode, IStorage, ISemantics
    {
        private string type = "";
        private ASTNode storage;
        private string nameID;


        public IdentificatorAST(string type, string nameID, Point point)
        {
            this.nameID = nameID;
            this.type = type;
            this.point = point;
        }

        public IdentificatorAST(string type, string nameID, ASTNode storage, Point point)
        {
            this.nameID = nameID;
            this.type = type;
            this.storage = storage;
            this.point = point;
        }

        public IdentificatorAST(string nameID, ASTNode storage, Point point)
        {
            this.nameID = nameID;
            this.storage = storage;
            this.point = point;
        }

        public IdentificatorAST(string nameID, Point point)
        {
            this.nameID = nameID;
            this.point = point;
        }

        public string GetTypeId()
        {
            return type;
        }

        public ASTNode GetStorage()
        {
            return storage;
        }

        public string GetName()
        {
            return nameID;
        }

        public override void Print(string level)
        {
            if (type != "")
            {
                Console.WriteLine(level + "[TYPE] " + type);
                level = level + "\t";
            }
            Console.WriteLine(level + "[ID] " + nameID);
            if (storage != null)
            {
                Console.WriteLine(level + "[STORAGE] =");
                storage.Print(level + "\t");
            }
        }

        public void AddAllSymbolIn(Dictionary<string, ASTNode> symTable)
        {
            if (symTable.ContainsKey(nameID))
            {
                if (type != "") ConsoleHelper.WriteError(nameID + " - Variable is redeclared");
                type = (symTable[nameID] as IdentificatorAST).GetTypeId();
            }
            else symTable.Add(nameID, this);

            if (storage is IStorage)
                (storage as IStorage).AddAllSymbolIn(symTable);
        }

        public string GetTypeMember()
        {
            if(storage is ISemantics && !(storage is BracketsAST))
            {
                string typeStorage = (storage as ISemantics).GetTypeMember();
                if (typeStorage != type) ConsoleHelper.WriteError("<" + point.y + "," + point.x + ">: Wrong type storage in '" + type + "' " + nameID + " = '" + typeStorage + "'");
            }
            return type;
        }
    }
}
