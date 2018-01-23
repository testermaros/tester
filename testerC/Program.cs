using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace testerC
{
    class Program
    {
    
        public static int Main()
        {
            int GroupPort = 6974;
            UdpClient udp = new UdpClient();
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, GroupPort);
            string str4 = "Is anyone out there?";
            byte[] sendBytes4 = Encoding.ASCII.GetBytes(str4);
            Thread.Sleep(1000);
            udp.Send(sendBytes4, sendBytes4.Length, groupEP);
            IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveBytes = udp.Receive(ref groupEP);
            string returnData = Encoding.ASCII.GetString(receiveBytes);
            Console.WriteLine(returnData);
            Console.ReadLine();
            Console.ReadLine();

            return 0;
        }
    }
}
