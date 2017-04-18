using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            new Server().StartServerAsync("http://localhost:8080").Wait();
        }
    }
}