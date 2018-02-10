using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using tester_server.Connection;

namespace tester_server
{
    public class Start
    {
        EventWaitHandle block_server_threads;
        //server
        private Socket listener;
        private string state;
        private bool running_flag;
        public const int TCP_PORT = 8619;

        // spracovanie dat po akceptovani pripojenia
        private ClientManager mannager;
        public Start()
        {
            running_flag = false;
            state = "TURNED_OFF";
            block_server_threads = new ManualResetEvent(true);
            mannager = new ClientManager();
        }

        private static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        /// <summary>
        /// Vytvorenie servera na zadanom porte
        /// </summary>
        /// <param name="allowed_host"> Maximalny pocet uzivatelov</param>
        public bool Create_Server(int allowed_host)
        {
            try
            {
                //ziskanie udajov
                IPAddress ipAddress = GetLocalIPAddress();
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, TCP_PORT);
                //vytvorenie servera
                listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(allowed_host);
                return true;
            }
            catch (SocketException)
            {
                Console.Error.WriteLine("Port sa uz pouziva");
                return false;
            }
        }

        public void Start_server()
        {
            if (state.Equals("RUNNING"))
            {
                Console.Error.WriteLine("Error: action start, state: " + state);
                return;
            }
            //server bezi
            if (state.Equals("SUSPENDED"))
            {
                //odblokovanie stavov
                block_server_threads.Set();
            }
            else
            {
                //zapnutie servera
                new Thread(Run).Start();
                running_flag = true;
            }
            //zmena stavu
            state = "RUNNING";
        }

        void Suspend_server()
        {
            if (!state.Equals("RUNNING"))
            {
                Console.Error.WriteLine("Error: action suspend, state: ", state);
                return;
            }
            //akcia suspendovania
            //zablokovanie
            block_server_threads.Reset();
            //zmena stavu
            state = "SUSPENDED";
        }

        void Shutdown_server()
        {
            if (state.Equals("TURNED_OFF"))
            {
                Console.Error.WriteLine("Error: action shutdown, state: " + state);
            }
            running_flag = false;
            state = "TURNED_OFF";
        }

        private void Run()
        {
            //spracovavanie pripojeni na server
            while (running_flag)
            {
                // cakanie na pripojenie
                Console.WriteLine("Waiting for a connection...");
                AcceptCallback(listener);
                //uspanie v pripade uspania servera
                block_server_threads.WaitOne();
            }
        }


        private void AcceptCallback(Socket server)
        {
            Socket connected_client = server.Accept();
            //ziskanie IP adresy
            string ip = ((IPEndPoint)(connected_client.RemoteEndPoint)).Address.ToString();
            Console.WriteLine("Connection accepted, IP: " + ip);
            //dummy ak nie je vlakno spracuvavajuce vstup do uzivatela zapnute zapni ho inak ignoruj
            mannager.AddClient(ip, connected_client);
            mannager.Start();
        }
    }
}
