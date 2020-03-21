namespace lab1.ASTNodes
{
    // VariableExprAST - Класс узла выражения для переменных (например, "a").
    class IdentificatorAST : ASTNode
    {
        private TypeAST type;
        private ASTNode storage;
        private AccessModifierAST modifier;
        private string nameID;

        public IdentificatorAST(TypeAST type, string nameID)
        {
            this.nameID = nameID;
            this.type = type;
        }

        public IdentificatorAST(TypeAST type, string nameID, ASTNode storage)
        {
            this.nameID = nameID;
            this.type = type;
            this.storage = storage;
        }


        public IdentificatorAST(string nameID) //call
        {
            this.nameID = nameID;
            //TODO: обращение к хэш таблице
        }

        public IdentificatorAST(AccessModifierAST modifier, string nameID)
        {
            this.modifier = modifier;
            this.nameID = nameID;
        }
    }
}
