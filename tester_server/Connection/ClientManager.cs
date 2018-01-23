using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tester_server.Connection
{
    public class ClientManager
    {
        // databaza pripojenych uzivatelov podla ip:port
        private ConcurrentDictionary<string, Socket> connected_users;
        //objekt vylucuje mozne prepisanie statusu vlakna
        private object lock_thread_status = new object();
        // status vlakna spracuvavajuceho prijatie requestov
        //ak nie je pripojeny ziadny uzivatel vypne sa
        private bool thread_status;


        public ClientManager()
        {
            connected_users = new ConcurrentDictionary<string, Socket>();
        }

        void AddClient(Socket client_socket)
        {

        }

        public void Start()
        {
            lock (lock_thread_status)
            {
                if (!thread_status)
                {
                    thread_status = true;
                    new Thread(Run).Start();
                }
            }
        }
        /// <summary>
        /// Citanie prichadzajucej komunikacie
        /// Ak je socket zatvoreny komunikacia bude ukoncena a odkaz na socket vymazany
        /// </summary>
       /* void Run()
        {
            while (true)
            {
                //ak nie je pripojeny ziadny client vlakno sa ukonci a zmeni status
                if (connected_users.Count == 0)
                {
                    //zabezpecenie pri preplanovani
                    lock (lock_thread_status)
                    {
                        thread_status = false;
                    }
                    break;
                }
                // obsluha citania prichadzajucich sprav
                foreach (var item in connected_users)
                {
                    //ak nie je pripojeny odstran z tabulky
                    if (!item.Value.Connected)
                        Remove_client(item.Key);
                    //je mozne citat data na vstupe
                    if (item.Value.Available > 0)
                    {
                        string recieved_string = Recieve(item.Value);
                        Message converted_message = Converter.Convert_from_string(recieved_string);
                        request_buffer.Enqueue(new Request(item.Value, converted_message));
                    }
                }
            }
        }*/
    }
}
