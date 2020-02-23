
namespace lab1.ASTNodes
{
    // TypeExprAST - Этот класс представляет тип для идентификатора.
    class TypeExprAST : ASTNode
    {
        public string nameType;
        public string dimension;
        public IdentificatorExprAST idExprAST;
        public TypeExprAST(IdentificatorExprAST idExprAST, string nameType, string dimension)
        {
            this.idExprAST = idExprAST;
            this.nameType = nameType;
            this.dimension = dimension;
        }
    }
}
