using System;
using System.Collections.Generic;
using System.Text;

namespace lab1.ASTNodes
{
    public class ASTNode
    {
        public struct Point
        {
            int y;
            int x;
            public Point(int y, int x)
            {
                this.y = y;
                this.x = x;
            }
        }

        public virtual void Print(string level) { }
        ~ASTNode() { }
    }
}
