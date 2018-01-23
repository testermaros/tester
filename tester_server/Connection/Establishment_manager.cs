using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tester_server.Connection
{
    public class Establishment_manager
    {
        private const int TCP_PORT = 8619;
        private const int UDP_PORT = 6974;
        public Establishment_manager(int port)
        {

        }
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
            while (true)
            {


            }
        }

    }
}
