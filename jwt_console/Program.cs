using jwt_api;
using System;

namespace jwt_console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(Api.GetToken());
            Console.Read();
        }
    }
}