﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tester_server.Connection;

namespace tester_server
{
    class Start
    {
        EventWaitHandle block_server_threads;
        //server
        private Socket listener;
        private string state;
        private bool running_flag;
        private const int TCP_PORT = 8619;

        // spracovanie dat po akceptovani pripojenia
        private ClientManager mannager;
        public Start()
        {
            running_flag = false;
            state = "TURNED_OFF";
            block_server_threads = new ManualResetEvent(true);
        }
        /// <summary>
        /// Vytvorenie servera na zadanom porte
        /// </summary>
        /// <param name="allowed_host"> Maximalny pocet uzivatelov</param>
        public void Create_Server(int allowed_host)
        {
            //ziskanie udajov
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[3];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, TCP_PORT);
            //vytvorenie servera
            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(allowed_host);
        }

        public void Start_server()
        {
            if (state.Equals("RUNNING"))
            {
                Console.Error.WriteLine("Error: action start, state: " + state);
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
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                //uspanie v pripade uspania servera
                block_server_threads.WaitOne();
            }
        }


        public void AcceptCallback(IAsyncResult ar) // ar objekt typu socket
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket connected_client = listener.EndAccept(ar);
            //ziskanie IP adresy
            string ip = ((IPEndPoint)(connected_client.RemoteEndPoint)).Address.ToString();
            Console.WriteLine("Connection accepted, IP: " + ip);
            //dummy ak nie je vlakno spracuvavajuce vstup do uzivatela zapnute zapni ho inak ignoruj
            mannager.Start();
        }
    }
}
