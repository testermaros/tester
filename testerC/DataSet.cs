using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testerC.Authentification
{
    public class DataSet
    {
        private static DataSet set = new DataSet();
        private Account dummy = new Account { password_hash = 0, Is_LogedOn = false, user_name = "" };

        public Account loged_user { get; private set; }
        
        private DataSet()
        {
            loged_user = dummy;
        }

        public static DataSet Instance()
        {
            return set;
        }

        public void SetLogedUser(string username, int pass_hash)
        {
            loged_user = new Account() { Is_LogedOn = true, password_hash = pass_hash, user_name = username };
        }

        public void UnSetLogedUser()
        {
            loged_user = dummy;
        }

    }
}
