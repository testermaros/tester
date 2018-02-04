using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testerC.Authentification;
using testerC.Connection;

namespace testerC
{
    public class RequestHelper
    {
        /// <summary>
        /// Vlozenie a zabalenie spravy podla zadaneho typu
        /// do urcenej struktury
        /// </summary>
        /// <param name="type"> typ requestu</param>
        /// <param name="data"> data requestu</param>
        /// <return>Message</return>
       public Message Wrap(MESSAGE_TYPE m,SERVICE_TYPE type, string data)
       {
            string message_content = "";

            if (m == MESSAGE_TYPE.REQUEST)
                message_content = new Request(type, data, DataSet.Instance().loged_user).ToString();

            return new Message(m, message_content);
        }
    }
}
