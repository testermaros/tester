using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tester_server.Tester.Tools
{
    public class TestParser
    {
        /*
        <test ID name>
        <user password_hash username>
        </user>
        <questions>
        <q ord_n=? question="">
	        <option> moznost </option>
	        <option> moznost </option>
	        <option> moznost </option>
        </q>
        <q ord_n=? question_text="">
	        <option> moznost </option>
	        <option> moznost </option>
	        <option> moznost </option>
        </q>
        <\questions>
        </test>
        */
        public TestTemplate ConvertStringToTest(string text)
        {
            try
            {
                XElement xml_tree = XElement.Parse(text);
                int testID = Int32.Parse(xml_tree.Attribute("ID").Value);
                string test_name = xml_tree.Attribute("name").Value;
                TestTemplate temp = new TestTemplate(testID, test_name);
                //ziskanie username
                var user_element = xml_tree.Element("user");
                string password_hash = user_element.Attribute("password_hash").Value;
                string username = user_element.Attribute("username").Value;
                // nasleduje tag s otazkami
                var questions_element = xml_tree.Element("questions");
                //prechadzanie jednotlivych otazok
                foreach (var current in questions_element.Elements("q"))
                {
                    int question_ord_num = Int32.Parse(current.Attribute("ord_n").Value);
                    string question_text = current.Attribute("question_text").Value;
                    List<string> options = (from a in current.Elements("option") select a.Value.ToString()).ToList();
                    QuestionTemplate quest_temp = new QuestionTemplate(question_ord_num, question_text, options, null);
                    temp.AddQuestion(quest_temp);
                }
                return temp;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        } 

        public ResultForm ConvertStringToResult(string text)
        {
            return null;
        }

        public string ConvertTestToString(TestTemplate template)
        {

            return null;
        }

        public string ConvertResultToString(ResultForm form)
        {
            return null;
        }
    }
}
