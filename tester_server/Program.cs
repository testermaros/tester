using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using tester_server.Connection;

namespace tester_server
{
    class Program
    {
        static void Main(string[] args)
        {
              EstablishmentManager m = new EstablishmentManager();
              m.Start();
              Console.ReadLine();
              Console.ReadLine();
              Console.ReadLine();
        }
    }
}
