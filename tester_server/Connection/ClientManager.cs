﻿using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace tester_server.Connection
{
    public class ClientManager
    {
        // databaza pripojenych uzivatelov podla ip:port
        private ConcurrentDictionary<string, Socket> connected_users;
        private ConcurrentDictionary<string, long> last_communication;
        //objekt vylucuje mozne prepisanie statusu vlakna
        private object lock_thread_status = new object();
        // status vlakna spracuvavajuceho prijatie requestov
        //ak nie je pripojeny ziadny uzivatel vypne sa
        private bool thread_status;
        //timer kontrola kazdych 10 sekund. Ak nepride keep alive socket je odstraneny(neaktivny)
        System.Timers.Timer timeout_timer = null;


        public ClientManager()
        {
            connected_users = new ConcurrentDictionary<string, Socket>();
            last_communication = new ConcurrentDictionary<string, long>();
        }

        public void AddClient(string ip, Socket client_socket)
        {
            connected_users.TryAdd(ip, client_socket);
            last_communication.TryAdd(ip, CurrentMilliseconds());
        }

        public void RemoveClient(string ip)
        {
            Socket removed;
            if(connected_users.TryRemove(ip, out removed))
                removed.Close();
        }
        /// <summary>
        /// Ukoncenie vlakien servera
        /// </summary>
        public void Stop()
        {
            thread_status = false;
            StopTimeoutTimer();
        }

        /// <summary>
        /// Spustenie servera - obsluhy poziadavok na server
        /// Neblokujuca verzia
        /// </summary>
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
        private void Run()
        {
            //spustenie timera poslednej komunikacie
            StartTimeoutTimer();

            Console.WriteLine("Obsluha poziadaviek spustena\n Pocet klientov: " + connected_users.Count);
            while (thread_status)
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
                    //je mozne citat data na vstupe
                    if (item.Value.Available > 0)
                    {
                        new Thread(() => HandleIncommingCommunication(item.Key,item.Value) ).Start();
                    }
                }
            }
            StopTimeoutTimer();
            Console.WriteLine("Vlakno ukoncene");
        }

        void HandleIncommingCommunication(string key, Socket value)
        {
            // nastavenie flagu na prijimanie spravy
            SetCommunicationTime(key, 0);
            string recieved_string = Recieve(value);
            //spracovanie poziadavky
            string response = ProccessMessage(recieved_string);
            //odoslanie odpovede
            SendResponse(response, value);
            // update poslednej komunikacie / zmena z komunikacie
            UpdateLastCommunicationTime(key);
        }

        private int Send(Socket client, string message)
        {
            try
            {
                client.Send(Encoding.ASCII.GetBytes(message));
            }
            catch (ArgumentNullException e)
            {
                Console.Error.WriteLine(e.Message);
                return 3;
            }
            catch (SocketException e)
            {
                Console.Error.WriteLine(e.Message);
                return 4;
            }
            catch (ObjectDisposedException e)
            {
                Console.Error.WriteLine(e.Message);
                return 5;
            }
            return 0;
        }

        private string Recieve(Socket client)
        {
            StringBuilder builder = new StringBuilder("");
            int size = 1024;
            byte[] buffer = new byte[size];
            while (client.Available > 0)
            {
                int recieved = client.Receive(buffer, size, SocketFlags.None);
                builder.Append(Encoding.ASCII.GetString(buffer, 0, recieved));
            }
            return builder.ToString();
        }

        private string ProccessMessage(string message)
        {
            
            return null;
        }

        private void SendResponse(string response, Socket client)
        {
            Send(client, response);
        }

        private long CurrentMilliseconds()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private void UpdateLastCommunicationTime(string key)
        {
            SetCommunicationTime(key, CurrentMilliseconds());
        }

        private void SetCommunicationTime(string key, long time_milis) {
            long value;
            last_communication.TryGetValue(key, out value);
            last_communication.TryUpdate(key, value, time_milis);
        }

        private void StartTimeoutTimer()
        {
            //ochrana pred opakovanym zapnutim
            if (timeout_timer != null)
                return;
            timeout_timer = new System.Timers.Timer(10000);
            timeout_timer.Enabled = true;
            //spustenie kontroly kazdych 10 sekund
            timeout_timer.Elapsed += (sender, e) => {
                Console.WriteLine("Spustam timer, pocet klientov:" + last_communication.Count);
                foreach (var item in last_communication)
                {
                    // ak bola posledna komunikacia uskutocnena pred viac ako 10 sekundami 
                    //ukonci spojenie s klientom -> klient je nedostupny
                    // 0 - komunikacia medzi klientom a serverom neukonci komunikaciu
                    if (item.Value != 0 && CurrentMilliseconds() - item.Value  >= 10000)
                    {
                        Console.WriteLine("Klient {0} neaktivny ukoncujem spojenie.", item.Key);
                        RemoveClient(item.Key);
                    }

                }
            };
            timeout_timer.Start();
        }

        private void StopTimeoutTimer()
        {
            if (timeout_timer == null)
                return;
            timeout_timer.Stop();
            timeout_timer = null;
        }
    }
}
