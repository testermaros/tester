using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace tester_server.Connection.Authentification
{
    public class LoginManager
    {
        private static string file_name = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UsersTest.tst");
        private List<Account> accounts;
        // mapa IP - uzivatelske meno
        // odhlasenie pri vypadku spojenia
        private Dictionary<string, string> ip_map;
        public LoginManager()
        {
            accounts = new List<Account>();
            LoginManager m = RetrieveFromFile();
            //subor zatial neexistuje
            if (m == null)
            {
                accounts = new List<Account>();
                ip_map = new Dictionary<string, string>();
            }
            else
            {
                this.accounts = m.accounts;
                this.ip_map = m.ip_map;
            }
        }

        ~LoginManager()
        {
            StoreToFile();
        }

        private Account AccountByUsername(string username)
        {
            var result = from user in accounts where user.user_name == username select user;
            return result.First();
        }

        public bool AddUser(string username, string password)
        {
            var result = from user in accounts where user.user_name == username select user;
            //ak sa nenachadza uzivatel s danym menom pridaj ho
            if (result.Count() == 0)
            {
                Console.WriteLine("Ucet {0} bol pridany", username);
                accounts.Add(new Account() { Is_LogedOn = false, password_hash = password.GetHashCode(), user_name = username });
                return true;
            }
            else
                return false;
        }

        public void MapUser(string ip, string username)
        {
            ip_map.Add(ip, username);
            Console.WriteLine("Uzivatel {0} bol namapovany", username);
        }

        public void RemoveMapedUser(string ip)
        {
            string username;
            if (ip_map.TryGetValue(ip, out username)) {
                ip_map.Remove(ip);
                LogOut(username);
                Console.WriteLine("Uzivatel {0} bol odmapvany", username);
            }
        }

        public bool ChangePassword(string username, string old_password, string new_password)
        {
            int old_hash = old_password.GetHashCode();
            int new_hash = new_password.GetHashCode();
            Account temp = AccountByUsername(username);
            if (temp == null)
                return false;
            //ak sa stare heslo zhoduje zmen ho
            if (old_hash == temp.password_hash) {
                temp.password_hash = new_hash;
                return true;
            }
            return false;
        }

        public bool IsLoggedIn(string username)
        {
            Account temp = AccountByUsername(username);
            if (temp == null)
                return false;
            return temp.Is_LogedOn;
        }

        public bool LogIn(string username, int hash_pass)
        {
            if (!IsLoggedIn(username))
            {
                Account temp = AccountByUsername(username);
                if (temp == null)
                    return false;
                if (temp.password_hash == hash_pass)
                {
                    temp.Is_LogedOn = true;
                    Console.WriteLine("Uzivatel {0} bol prihlaseny", username);
                    return true;
                }
            }
            return false;
        }

        public void LogOut(string username)
        {
            Account user = AccountByUsername(username);
            if (user == null) return;
            user.Is_LogedOn = false;
            Console.WriteLine("Uzivatel {0} bol odhlaseny", username);
        }

        //serializacia udajov
        private void StoreToFile()
        {
            XmlSerializer ser = new XmlSerializer(typeof(LoginManager));
            using (Stream str = File.Create(file_name))
                ser.Serialize(str, this);
        }

        private static LoginManager RetrieveFromFile()
        {
            XmlSerializer ser = new XmlSerializer(typeof(LoginManager));
            try
            {
                using (Stream str = File.Open(file_name, FileMode.Open))
                    return (LoginManager)ser.Deserialize(str);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
