using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

//ahooh

namespace testerC
{
    public class SenderBroadcast
    {
        private const int port1 = 6974;
        UdpClient client = new UdpClient();
        IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, port1);
        public void SendBroadcast() {
            client.EnableBroadcast = true;
            byte[] bytes = Encoding.ASCII.GetBytes("hello");
            client.Send(bytes, bytes.Length, ip);

            client.Close();
        }
        public void Receive() {

            var ServerResponseData = client.Receive(ref ip);
            var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
            Console.WriteLine("Sprava {0} od {1}", ServerResponse, ip.Address.ToString());
        }
        
    
    }
}