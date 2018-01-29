using System;
using System.Xml.Linq;

namespace tester_server.Connection
{
    /// <summary>
    /// <request type></request>
    /// </summary>
    class Request
    {
        /// <summary>
        /// Enumerator moznych typov requestov
        /// TESTS_LIST - zoznam vsetkych vytvorenych testov
        /// GET - stiahnutie zadaneho testu 
        /// DISCONNECT - Odpojenie hosta
        /// LOGIN - prihlasenie sa pod uctom
        /// EVAL - vyhodnotenie testu
        /// </summary>
        public enum REQUEST_TYPE{
            TESTS_LIST, GET, DISCONNECT, LOGIN, EVAL
        }

        public REQUEST_TYPE type { private set; get; }
        public string data { get; private set; }

        public Request(REQUEST_TYPE type, string data)
        {
            this.type = type;
            this.data = data;
        }

        public string ConvertToString()
        {
            XElement tree = new XElement("Request", data);
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
            XElement tree = XElement.Parse(req_s);
            REQUEST_TYPE t = (REQUEST_TYPE)Int32.Parse(tree.Attribute("type").Value);
            string data = tree.Value;
            return new Request(t, data);
        }
    }
}
