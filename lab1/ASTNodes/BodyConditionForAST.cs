using System.Collections.Generic;

namespace lab1.ASTNodes
{
    class BodyConditionForAST : ASTNode
    {
        private List<ASTNode> declaredVar; // int a = 5, int b = 7
        private ASTNode conditionWithBody; // a < b {body}
        private ASTNode postcondition; // a++

        public BodyConditionForAST(List<ASTNode> declaredVar, ASTNode conditionWithBody, ASTNode postcondition) // if(a > b) || if((a > s) > (b && s))
        {
            this.declaredVar = declaredVar;
            this.conditionWithBody = conditionWithBody;
            this.postcondition = postcondition;
        }
    }
}
