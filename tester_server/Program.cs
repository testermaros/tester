﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
             new Thread(Do).Start();
            Console.WriteLine();
            Console.ReadLine();
            Console.ReadLine();
            Console.ReadLine();
        }

        static void Do()
        {
            TcpClient tcpclnt = new TcpClient();
            Console.WriteLine("Connecting.....");

            tcpclnt.Connect("192.168.1.155", 8619);
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
