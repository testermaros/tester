using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace testerC
{
    public class SenderBroadcast
    {
        private const int port1 = 6974;
        public void SendBroadcast() {
            UdpClient client = new UdpClient();
            client.EnableBroadcast = true;
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, port1);
            byte[] bytes = Encoding.ASCII.GetBytes("hello");
            client.Send(bytes, bytes.Length, ip);

            client.Close();
        }
        public void Receive(UdpClient Client, IPEndPoint ip) {

            var ServerResponseData = Client.Receive(ref ip);
            var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
            Console.WriteLine("Sprava {0} od {1}", ServerResponse, ip.Address.ToString());
        }
        
    
    }
}