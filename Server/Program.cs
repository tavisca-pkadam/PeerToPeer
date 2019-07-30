using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            Console.WriteLine("Hello World!");
            server.ExecuteServer();
        }
    }
}
