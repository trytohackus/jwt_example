using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwt_api
{
    public static class Helpers
    {
        public static byte[] ReadFully(this Stream input, long length)
        {
            if (length > 10 * 1024 * 1024) throw new Exception("Now allowed Stream bigger than 10MB!");

            byte[] buffer = new byte[length];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }

        public static void ForEachStop<T>(this IEnumerable<T> source, Func<T, bool> action)
        {
            foreach (T element in source)
                if (action(element))
                    break;
        }
    }
}