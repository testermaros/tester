using System;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;
using tester_server.Connection;

namespace tester_server
{
    class Program
    {
        static void Main(string[] args)
        {
            /*EstablishmentManager udp_establishment = new EstablishmentManager();
            udp_establishment.Start();*/
            //spustenie tcp servera
            Start start_sequence = new Start();
            start_sequence.Create_Server(100);
            start_sequence.Start_server();
            /*new Thread(Do).Start();*/
            /*ClientManager m = new ClientManager();
            Console.WriteLine(m.parse("prva sprava\nje toto stale\r\t\n\r\t\ntoto je uz druha sprava"));*/
            /*string data = "<data> <string> erik</string> </data>";
            XElement e = new XElement("El", data);
            string s = e.ToString();
            XElement r = XElement.Parse(s);
            XElement v = XElement.Parse(r.Value);
            Console.WriteLine((SERVICE_TYPE)1);
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();*/
        }

        static void Do()
        {
            TcpClient tcpclnt = new TcpClient();
            Console.WriteLine("Connecting.....");

            tcpclnt.Connect("192.168.1.153", 8619);
            /*Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 8619);
            client.Connect(ep);
            Thread.Sleep(5000);
            client.Close();*/            
            Thread.Sleep(5000);
            Console.WriteLine("Klient sa ukoncil\n");
            tcpclnt.Close();
        }
    }
}
