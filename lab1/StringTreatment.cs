using System;
using System.Text;

namespace lab1 // for(i++;)
{
    public sealed class StringTreatment
    {
        public static string[] DeleteComments(string stroke)
        {
            string deleteComments = FindComments(stroke);
            string deleteTabulation = deleteComments.Replace("\t", "");
            string deleteSlashR = deleteTabulation.Replace("\r", "");
            string[] deleteNewline = deleteSlashR.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            return deleteNewline;
        }

        public static string FindComments(string str)
        {
            StringBuilder strWithoutComments = new StringBuilder();
            StringBuilder multyLineComment = new StringBuilder();
            bool isFindLine = false;
            bool isFindMultyLine = false;
            if(str.Length > 1)
            {
                for (int i = 0; i < str.Length - 1; i++)
                {
                    if (str[i] == '/' && str[i + 1] == '*')
                    {
                        isFindMultyLine = true;
                    }

                    if (str[i] == '/' && str[i + 1] == '/')
                    {
                        isFindLine = true;
                    }

                    if (str[i] == '\n' || i == str.Length - 1)
                    {
                        isFindLine = false;
                    }

                    if (!isFindLine && !isFindMultyLine) strWithoutComments.Append(str[i]);
                    else if (isFindMultyLine) multyLineComment.Append(str[i]);

                    if (str[i] == '*' && str[i + 1] == '/')
                    {
                        isFindMultyLine = false;
                    }
                }
            }
            else strWithoutComments.Append(str[0]);

            if (isFindMultyLine) strWithoutComments.Append(multyLineComment);

            int strLen = str.Length;

            // добавляем последний элемет если он не относится ни к одному из видов комментариев
            try
            {
                if (!(str[strLen - 2] == '/' && str[strLen - 1] == '*'
                  || str[strLen - 2] == '/' && str[strLen - 1] == '/'
                  || str[strLen - 2] == '*' && str[strLen - 1] == '/'
                  || str[strLen - 1] == '\n') && !isFindLine)
                {
                    strWithoutComments.Append(str[strLen - 1]);
                }
            }
            catch
            {

            }
            return strWithoutComments.ToString();
        }
    }
}
