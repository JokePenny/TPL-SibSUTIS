using System;

namespace lab1.ASTNodes
{
    public struct Point
    {
        public readonly int y;
        public readonly int x;
        public Point(int y, int x)
        {
            this.y = y;
            this.x = x;
        }
    }

    public class ASTNode
    { 
        public Point point;

        public virtual void Print(string level) { }
		public virtual void PrintASM(string leveltabulation, bool isNewLine = false) { }
		~ASTNode() { }
    }
}
