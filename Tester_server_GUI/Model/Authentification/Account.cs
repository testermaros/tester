using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tester_server.Connection.Authentification
{
    public class Account
    {
        public static readonly Account DUMMY = new Account { password_hash = 0, Is_LogedOn = false, user_name = "" };
        public string user_name { get; set; }
        public int password_hash { get; set; }
        public bool Is_LogedOn { get; set; }

        public override string ToString()
        {
            return "Username: " + user_name + " State: " +( (Is_LogedOn) ? "Online" : "Offline" );
        }
    }
}
