using System;

namespace lab1.ASTNodes
{
    // VariableExprAST - Класс узла выражения для переменных (например, "a").
    class IdentificatorAST : ASTNode
    {
        private string type = "";
        private ASTNode storage;
        private string nameID;

        public IdentificatorAST(string type, string nameID)
        {
            this.nameID = nameID;
            this.type = type;
        }

        public IdentificatorAST(string type, string nameID, ASTNode storage)
        {
            this.nameID = nameID;
            this.type = type;
            this.storage = storage;
        }

        public IdentificatorAST(string nameID, ASTNode storage)
        {
            this.nameID = nameID;
            this.storage = storage;
        }

        public IdentificatorAST(string nameID) //call
        {
            this.nameID = nameID;
            //TODO: обращение к хэш таблице
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
    }
}
