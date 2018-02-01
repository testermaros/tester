using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tester_server.Connection
{
    /// <summary>
    /// REQUEST - poziadavka na server
    /// KEEP - udrzanie spojenia posielanie periodicky
    /// </summary>
    public enum MESSAGE_TYPE {
        REQUEST, KEEP
    }
    class Message
    {
        /// <summary>
        /// <message type>data</message>
        /// </summary>
        public MESSAGE_TYPE mess_type { get; private set; }
        public string data { get; private set; }

        Message( MESSAGE_TYPE t, string data)
        {
            mess_type = t;
            this.data = data;
        }
        
        public override string ToString()
        {
            XElement element = new XElement("message", data);
            element.SetAttributeValue("type", mess_type);
            return element.ToString();
        }

        public static Message Parse(string text)
        {
            try
            {
                XElement tree = XElement.Parse(text);
                MESSAGE_TYPE t = (MESSAGE_TYPE)Int32.Parse(tree.Attribute("type").Value);
                Message m = new Message(t, tree.Value);
                return m;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
