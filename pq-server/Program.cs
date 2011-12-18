using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace pq_server
{
    class Program
    {
        static void Main(string[] args)
        {

            HttpServer server = new HttpServer(25);
            server.Start(8085);
            Console.WriteLine("Press [Enter] to quit.");
            Console.ReadLine();
        }
    }
}
