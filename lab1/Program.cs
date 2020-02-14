using System;

namespace lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            Command.RunCommand(args);
        }

        private static void TestMethod()
        {

            // find min
            int[] array = new int[8];
            int tmp = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (tmp > array[i])
                    tmp = array[i];
            }


            // find substroke in stroke
            string stroke = "sdf";
            string strokeInFind = "sasd asd sssa as sdf";
            int doneLetter = stroke.Length;
            int nowAccesLetter = 0;
            int indexStartWordInStroke = -1;
            for (int i = 0; i < strokeInFind.Length; i++)
            {
                if (stroke[nowAccesLetter] == strokeInFind[i])
                {
                    nowAccesLetter++;
                    if (nowAccesLetter == doneLetter)
                    {
                        indexStartWordInStroke = i - nowAccesLetter;
                        break;
                    }
                }
                else nowAccesLetter = 0;
            }

            // find NOD
            int firstNum = 5;
            int secondNum = 10;
            while (firstNum != 0 && secondNum != 0)
            {
                if (firstNum > secondNum)
                    firstNum = firstNum % secondNum;
                else
                    secondNum = secondNum % firstNum;
            }
            Console.WriteLine(secondNum + firstNum);
        }
    }
}
