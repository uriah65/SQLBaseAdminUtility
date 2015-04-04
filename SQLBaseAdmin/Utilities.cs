using System;
using System.Collections.Generic;

namespace SQLBaseAdmin
{
    internal static class Utilities
    {
        public static string ExtractString(byte[] bytes, int stratPoint)
        {
            string result = "";
            for (int i = stratPoint; i < bytes.Length; i++)
            {
                if (bytes[i] == 0)
                {
                    break;
                }

                char ch = Convert.ToChar(bytes[i]);
                result += ch;
            }
            return result.Trim();
        }

        public static List<string> ExtractStrings(byte[] bytes, int startIx, int maxLength)
        {
            List<string> result = new List<string>();

            string str = ExtractString(bytes, startIx);
            while (str != "" && startIx < maxLength)
            {
                result.Add(str);
                startIx += str.Length + 1;
                str = ExtractString(bytes, startIx);
            }

            return result;
        }

        public static string ExceptionMessage(Exception ex)
        {
            string message = "";
            while (ex != null)
            {
                // message += Environment.NewLine;
                message += "Exception has occurred." + Environment.NewLine;
                message += "Message:  " + ex.Message + Environment.NewLine;
                message += "Stack:    " + ex.StackTrace + Environment.NewLine;
                ex = ex.InnerException;
            }
            return message;
        }
    }
}