using System;
using System.Text;
using System.Text.RegularExpressions;

namespace lab1
{
    class StringTreatment
    {
        public static string dictSpaceBetween;

        public static string[] DeleteCommentsAndTab(string stroke)
        {
            string deleteComments = FindComments(stroke);
            string deleteTabulation = deleteComments.Replace("\t", "");
            string deleteSlashR = deleteTabulation.Replace("\r", "");
            string[] deleteNewline = deleteSlashR.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            return deleteNewline;
        }

        private static string FindComments(string str)
        {
            StringBuilder strWithoutComments = new StringBuilder();
            StringBuilder multyLineComment = new StringBuilder();
            bool isFindNewLine = false;
            bool isFindMultyLine = false;
            for (int i = 0; i < str.Length - 1; i++)
            {
                if (str[i] == '/' && str[i + 1] == '*')
                {
                    isFindMultyLine = true;
                }
                if (str[i] == '/' && str[i + 1] == '/')
                {
                    isFindNewLine = true;
                }
                if (str[i] == '\n')
                {
                    isFindNewLine = false;
                }

                if (!isFindNewLine && !isFindMultyLine) strWithoutComments.Append(str[i]);
                else if (isFindMultyLine) multyLineComment.Append(str[i]);

                if (str[i] == '*' && str[i + 1] == '/')
                {
                    isFindMultyLine = false;
                    i++;
                }
            }
            if (isFindMultyLine) strWithoutComments.Append(multyLineComment);
            return strWithoutComments.ToString();
        }

        public static string FormatStroke(string stroke)
        {
            string insertSpaceBetweenKey = InsertSpaceBetween(stroke);
            return Regex.Replace(insertSpaceBetweenKey, @"\s+", " ");
        }

        private static string InsertSpaceBetween(string deleteNewline)
        {
            StringBuilder insertingSpace = new StringBuilder(deleteNewline);
            char[] strokeWithSpace = deleteNewline.ToCharArray();

            for (int i = 0, indexInsert = 0; i < strokeWithSpace.Length; i++, indexInsert++)
            {
                for (int j = 0; j < dictSpaceBetween.Length; j++)
                {
                    if (strokeWithSpace[i] == dictSpaceBetween[j])
                    {
                        insertingSpace.Insert(indexInsert, " ");
                        indexInsert++;
                        insertingSpace.Insert(indexInsert + 1, " ");
                        indexInsert++;
                        break;
                    }
                }
            }
            return insertingSpace.ToString();
        }
    }
}
