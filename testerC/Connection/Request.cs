using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using testerC.Authentification;

namespace testerC.Connection
{
    /// <summary>
    /// Enumerator moznych typov requestov
    /// TESTS_LIST - zoznam vsetkych vytvorenych testov
    /// GET - stiahnutie zadaneho testu 
    /// DISCONNECT - Odpojenie hosta
    /// LOGIN - prihlasenie sa pod uctom
    /// EVAL - vyhodnotenie testu
    /// </summary>
    public enum SERVICE_TYPE
    {
        TESTS_LIST, GET, DISCONNECT, LOGIN, EVAL
    }

    public class Request
    {
        public SERVICE_TYPE type { private set; get; }
        public string data { get; private set; }
        public Account user;
        public Request(SERVICE_TYPE type, string data, Account user)
        {
            this.type = type;
            this.data = data;
            this.user = user;
        }
        /*
         * <Request>
         * <user password_hash username></user>
         * <data></data>
         * </Request>
         */
        public override string ToString()
        {
            XElement user_element = new XElement("user");
            user_element.SetAttributeValue("password_hash", user.password_hash);
            user_element.SetAttributeValue("username", user.user_name);
            XElement data_element = new XElement("data", data);
            XElement tree = new XElement("Request", user_element, data_element);
            tree.SetAttributeValue("type", type);
            return tree.ToString();
        }

        /// <summary>
        /// Dummy parsovanie
        /// </summary>
        /// <param name="req_s"> string predstavujuci sparsovany objekt</param>
        /// <returns></returns>
        public static Request ConvertToRequest(string req_s)
        {
            try
            {
                XElement tree = XElement.Parse(req_s);
                SERVICE_TYPE t = (SERVICE_TYPE)Enum.Parse(typeof(SERVICE_TYPE), tree.Attribute("type").Value);

                XElement user_element = tree.Element("user");
                string username = user_element.Attribute("username").Value;
                int hash = Int32.Parse(user_element.Attribute("password_hash").Value);
                Account user = new Account() { Is_LogedOn = false, password_hash = hash, user_name = username };

                string data = tree.Element("data").Value;
                return new Request(t, data, user);
            }
            catch (Exception) {
                return null;
            }
        }
    }
}
