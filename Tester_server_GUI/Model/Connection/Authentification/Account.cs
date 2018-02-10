using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tester_server.Connection.Authentification
{
    public class Account
    {
        public string user_name { get; set; }
        public int password_hash { get; set; }
        public bool Is_LogedOn { get; set; }
    }
}
