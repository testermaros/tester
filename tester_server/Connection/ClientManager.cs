using System;
using System.Collections.Concurrent;
using System.Net;
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
        //ak su precitane byty aj za hranice konca spravy ulozia sa do pomocnej pamati
        private ConcurrentDictionary<string, string> buffers;

        //objekt vylucuje mozne prepisanie statusu vlakna
        private object lock_thread_status = new object();
        // status vlakna spracuvavajuceho prijatie requestov
        //ak nie je pripojeny ziadny uzivatel vypne sa
        private bool thread_status;

        //timer kontrola kazdych 10 sekund. Ak nepride keep alive socket je odstraneny(neaktivny)
        System.Timers.Timer timeout_timer = null;

        //spracovanie poziadaviek
        RequestHandler handler;

        //ukoncovacia sekvencia
        private const string EOM = "\r\t\n\r\t\n";
        private const int MAX_MESSAGE_DELAY = 500;
        private const int MAX_SIZE = 1024;

        public ClientManager()
        {
            connected_users = new ConcurrentDictionary<string, Socket>();
            last_communication = new ConcurrentDictionary<string, long>();
            buffers = new ConcurrentDictionary<string, string>();
            handler = new RequestHandler(this);
        }

        public void AddClient(string ip, Socket client_socket)
        {
            connected_users.TryAdd(ip, client_socket);
            last_communication.TryAdd(ip, CurrentMilliseconds());
            buffers.TryAdd(ip, "");
        }

        public void RemoveClient(string ip)
        {
            Socket removed;
            if(connected_users.TryRemove(ip, out removed))
                removed.Close();
            long temp;
            last_communication.TryRemove(ip,out temp);
            string temp_b;
            buffers.TryRemove(ip, out temp_b);
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
                }
                // obsluha citania prichadzajucich sprav
                foreach (var item in connected_users)
                {
                    //je mozne citat data na vstupe
                    if (item.Value.Available > 0)
                    {
                        HandleIncommingCommunication(item.Key, item.Value);
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
            string response = ProccessMessage(recieved_string, key);
            //zahod poziadavku a ukonci obsluhu
            if (response == null)
                return;
            //odoslanie odpovede
            SendResponse(response, value);
            // update poslednej komunikacie / zmena z komunikacie
            UpdateLastCommunicationTime(key);
        }

        private int Send(Socket client, string message)
        {
            try
            {
                //pridanie ukoncovacich znakov
                message = message + EOM;
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
        /// <summary>
        /// Rozdelenie sprav na zaklade ukoncovacich znakov.
        /// Ak je precitana sprava nad ramec spravy, zvysok je ulozeny do buffera od zaciatku
        /// </summary>
        /// <param name="client">Client ktoremu bude sprava odoslana</param>
        /// <returns></returns>
        private string Recieve(Socket client)
        {
            string ip = ((IPEndPoint)(client.RemoteEndPoint)).Address.ToString();
            //uloz do buildera predchadzajuce znaky
            string saved_part = "";
            buffers.TryGetValue(ip, out saved_part);
            StringBuilder builder = new StringBuilder(saved_part);
            string temp_message;
            int new_message_index = 0;
            long last_recieved_time = 0;
            byte[] buffer = new byte[MAX_SIZE];
            while (true) {
                while (client.Available > 0)
                {
                    int recieved = client.Receive(buffer, 0, MAX_SIZE, SocketFlags.None);
                    builder.Append(Encoding.ASCII.GetString(buffer, 0, recieved));
                    temp_message = builder.ToString();
                    //bola prijata cela sprava
                    if (IsEntireMessageRecieved(temp_message, out new_message_index))
                    {
                        //vrat zvysok po konci spravy na zaciatok buffera
                        string next_part = temp_message.Substring(new_message_index + EOM.Length);
                        //uloz zvysok
                        buffers.TryUpdate(ip, saved_part, next_part);
                        //return poslednu prijatu spravu
                        return temp_message.Substring(0, new_message_index);
                    }
                    //nastavenie poslednej doby vykonania requestu
                    last_recieved_time = CurrentMilliseconds();
                }
                //ak nie je sprava dokoncena ale nie su ziadne prichadzajuce data cakaj stanovenu dobu 
                //po uplynuti doby povazuj spravu za nedostupnu
                if (CurrentMilliseconds() - last_recieved_time > MAX_MESSAGE_DELAY)
                    return null;
            }
        }

        public string parse(string temp_message)
        {
            int new_message_index;
            if (IsEntireMessageRecieved(temp_message, out new_message_index))
            {
                //vrat zvysok po konci spravy na zaciatok buffera
                string next_part = temp_message.Substring(new_message_index + EOM.Length);
                Console.WriteLine(next_part);
                //return poslednu prijatu spravu
                return temp_message.Substring(0, new_message_index);
            }
            return null;
        }

        /// <summary>
        /// Hladanie ukoncovacej sekvencie. 
        /// </summary>
        /// <param name="temp_buffer"></param>
        /// <returns></returns>
        private bool IsEntireMessageRecieved(string string_buffer, out int next_message_index)
        {
            return (next_message_index = string_buffer.IndexOf(EOM)) >= 0;
        }

        /// <summary>
        /// Spracovanie prijatej spravy
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Vysledok spracovania</returns>
        private string ProccessMessage(string message, string key)
        {
            Response resp = handler.ProcessRequest(message, key);
            if (resp == null)
                return null;
            return resp.ConvertToString();
        }

        private void SendResponse(string response, Socket client)
        {
            Send(client, response);
        }

        private long CurrentMilliseconds()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public void UpdateLastCommunicationTime(string key)
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
