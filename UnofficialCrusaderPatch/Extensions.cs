using System;
using System.Text;
using System.Windows;

namespace UCP
{
    static class Extensions
    {
        public static double MeasureHeight(this FrameworkElement element)
        {
            element.Arrange(new Rect());
            return element.ActualHeight;
        }

        public static int FindIndex(this byte[] self, params byte[] search)
        {
            int last = search.Length - 1;
            for (int i = 0; i < self.Length; i++)
            {
                for (int j = 0; j < search.Length; j++)
                {
                    if (self[i + j] == search[j])
                    {
                        if (j == last)
                            return i;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return -1;
        }

        public static int FindIndex(this byte[] self, int value)
        {
            return self.FindIndex(BitConverter.GetBytes(value));
        }

        public static string ToHexString(this byte[] self)
        {
            StringBuilder sb = new StringBuilder(12 + 3 * self.Length);

            sb.Append("byte[");
            sb.Append(self.Length);
            sb.Append("] { ");

            for (int i = 0; i < self.Length; i++)
            {
                sb.Append(self[i].ToString("X2"));
                sb.Append(' ');
            }
            sb.Append('}');

            return sb.ToString();
        }
        
    }
}
