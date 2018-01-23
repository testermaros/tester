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
            UdpClient server = new UdpClient(UDP_PORT);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = server.Receive(ref groupEP);

                    Console.WriteLine("Received broadcast from {0} :\n {1}\n",
                    groupEP.ToString(),
                    Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                    server.Send(bytes, bytes.Length, groupEP);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                server.Close();
            }
        }
    }
}
