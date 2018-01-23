using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tester_server.Connection
{
    public class EstablishmentManager
    {
        private const int UDP_PORT = 6974;
        /// <summary>
        /// Spustenie UDP servera. Neblokujuca verzia
        /// Bezi na vlastnom vlakne
        /// </summary>
        public void Start()
        {
            Thread deamon = new Thread(Run);
            deamon.IsBackground = true;
            deamon.Start();
        }

        /// <summary>
        /// Funkcia vlakna. Nekonecna slucka kde udp server caka na hello packet na ktory odpovie.
        /// Vlakno sa ukonci pri ukonceni programu
        /// </summary>
        private void Run()
        {
            var Server = new UdpClient(8888);
            var ResponseData = Encoding.ASCII.GetBytes("Hello world");

            while (true){
                var ClientEp = new IPEndPoint(IPAddress.Any, 0);
                Console.WriteLine();
                var ClientRequestData = Server.Receive(ref ClientEp);
                var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

                Console.WriteLine("Recived {0} from {1}, sending response", ClientRequest, ClientEp.Address.ToString());
                Server.Send(ResponseData, ResponseData.Length, ClientEp);
            }
        }
    }
}
