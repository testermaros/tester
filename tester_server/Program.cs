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
            /*  EstablishmentManager m = new EstablishmentManager();
              m.Start();
              Console.ReadLine();
              Console.ReadLine();
              Console.ReadLine();*/
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);

            IPAddress broadcast = IPAddress.Parse("192.168.1.255");

            byte[] sendbuf = Encoding.ASCII.GetBytes("asdasdsa");
            IPEndPoint ep = new IPEndPoint(broadcast, 11000);

            s.SendTo(sendbuf, ep);

            Console.WriteLine("Message sent to the broadcast address");
            Console.ReadLine();
        }
    }
}
