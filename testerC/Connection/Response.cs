﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace testerC.Connection
{
    public enum RESULT_CODE
    {
        BAD_REQUEST, PERM_DENIED, SUCCESS, UNAVAILABLE
    }
    public class Response
    {
        public SERVICE_TYPE type { get; private set; }
        public RESULT_CODE code;
        public string data;
        public Response(SERVICE_TYPE type, string data, RESULT_CODE r)
        {
            this.type = type;
            this.data = data;
            this.code = r;
        }

        public override string ToString()
        {
            XElement tree = new XElement("Response", data);
            tree.SetAttributeValue("type", type);
            tree.SetAttributeValue("code", code);
            return tree.ToString();
        }

        /// <summary>
        /// Dummy parsovanie
        /// </summary>
        /// <param name="req_s"> string predstavujuci sparsovany objekt</param>
        /// <returns></returns>
        public static Response ConvertToResponse(string req_s)
        {
            XElement tree = XElement.Parse(req_s);
            SERVICE_TYPE t = (SERVICE_TYPE)Enum.Parse(typeof(SERVICE_TYPE), tree.Attribute("type").Value);
            RESULT_CODE r = (RESULT_CODE)Enum.Parse(typeof(RESULT_CODE), tree.Attribute("code").Value);
            string data = tree.Value;
            return new Response(t, data, r);
        }
    }
}
