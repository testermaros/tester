using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace testerC.Connection
{
    class ConnectionMannager
    {
        private const string EOM = "\r\t\n\r\t\n";
        private const int MAX_SIZE = 1024;
        private const int MAX_MESSAGE_DELAY = 7000;

        public Socket server_point { get; private set; }

        public ConnectionMannager()
        {

        }

        public bool Connect(string ip, int port)
        {
            IPEndPoint server_end_point;
            IPAddress server_address = null;

            //adresu sa nepodarilo sparsovat
            if(!IPAddress.TryParse(ip, out server_address))
            {
                Console.Error.WriteLine("Error: IP address parsing");
                return false;
            }

            server_end_point = new IPEndPoint(IPAddress.Parse(ip), port);
            server_point = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server_point.ReceiveTimeout = 1000;
            try
            {
                server_point.Connect(server_end_point);
            }
            catch (SocketException)
            {
                Console.Error.WriteLine("Nepodarilo sa pripojit na server");
                return false;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                return false;
            }

            return true;
        }

        public int Send(string message)
        {
            try
            {
                //pridanie ukoncovacich znakov
                message = message + EOM;
                server_point.Send(Encoding.ASCII.GetBytes(message));
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
        public string Recieve()
        {
            string saved_part = "";
            StringBuilder builder = new StringBuilder(saved_part);
            string temp_message;
            int new_message_index = 0, recieved;
            long last_recieved_time = CurrentMilliseconds();
            byte[] buffer = new byte[MAX_SIZE];
            while (true)
            {
                while (server_point.Available > 0)
                {
                    try
                    {
                        recieved = server_point.Receive(buffer, 0, MAX_SIZE, SocketFlags.None);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    // data boli prijate 
                    builder.Append(Encoding.ASCII.GetString(buffer, 0, recieved));
                    temp_message = builder.ToString();
                    //bola prijata cela sprava
                    if (IsEntireMessageRecieved(temp_message, out new_message_index))
                    {
                        string next_part = temp_message.Substring(new_message_index + EOM.Length);                     
                        return temp_message.Substring(0, new_message_index);
                    }
                    //nastavenie poslednej doby vykonania requestu
                    last_recieved_time = CurrentMilliseconds();
                }
                //ak nie je sprava dokoncena ale nie su ziadne prichadzajuce data cakaj stanovenu dobu 
                //po uplynuti doby povazuj spravu za nedostupnu
                if (CurrentMilliseconds() - last_recieved_time > MAX_MESSAGE_DELAY)
                {
                    Console.WriteLine("Recieve Time Out: doba prekrocena");
                    return null;
                }
            }
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

        private long CurrentMilliseconds()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
    }
}
