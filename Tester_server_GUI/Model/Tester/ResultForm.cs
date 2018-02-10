using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tester_server.Tester
{
    public class ResultForm
    {
        public Dictionary<int, List<string>> answers { get; private set; }
        public int test_ID;

        public ResultForm(int test_ID)
        {
            this.test_ID = test_ID;
            answers = new Dictionary<int, List<string>>();
        }

        public void AddAnswer(int question_num, string answer)
        {
            List<string> temp_list;
            //ak je list odpovedi prazdny
            if(!answers.TryGetValue(question_num, out temp_list))
            {
                temp_list = new List<string>();
            }
            temp_list.Add(answer);
            answers.Add(question_num, temp_list);
        }

        /// <summary>
        /// Format:
        /// <Result test_id>
        /// <q q_id><answer>prve</answer><answer>druhe</answer></q>
        /// <q q_id><answer>prve</answer><answer>druhe</answer></q>
        /// .....
        /// </Result>
        /// </summary>
        /// <returns></returns>

        public override string ToString()
        {
            XElement tree = new XElement("Result");
            tree.SetAttributeValue("test_id", test_ID);
            foreach(var item in answers)
            {
                XElement temp = new XElement("q", item.Value);
                temp.SetAttributeValue("q_id", item.Key);
                tree.Add(temp);
            }
            return tree.ToString();
        }

        public static ResultForm Convert(string text)
        {
            try
            {
                XElement tree = XElement.Parse(text);
                int test_id = Int32.Parse(tree.Attribute("test_id").Value);
                ResultForm form = new ResultForm(test_id);
                foreach (var item in tree.Elements("q"))
                {
                    int q_num = Int32.Parse(item.Attribute("q_id").Value);
                    string value = item.Value;
                    form.AddAnswer(q_num, value);
                }
                return form;
            }
            catch (Exception) { return null; }
        }
    }
}