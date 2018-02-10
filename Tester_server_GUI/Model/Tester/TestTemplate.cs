using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace tester_server.Tester
{
    public class TestTemplate
    {
        public List<QuestionTemplate> questions { get;  set; }
        public int ID { get;  set; }
        public string name { get;  set; }
        public TestTemplate() { }
        public TestTemplate(int ID, string name)
        {
            questions = new List<QuestionTemplate>();
            this.ID = ID;
        }
        /// <summary>
        /// Pridanie otazky do testu
        /// </summary>
        /// <param name="temp"></param>
        public void AddQuestion(QuestionTemplate temp)
        {
            questions.Add(temp);
        }

        public void AddQuestions(List<QuestionTemplate> temp_list)
        {
            questions.AddRange(temp_list);
        }

        public static TestTemplate Parse(string text)
        {
            try
            {
                XElement xml_tree = XElement.Parse(text);
                int testID = Int32.Parse(xml_tree.Attribute("ID").Value);
                string test_name = xml_tree.Attribute("name").Value;
                TestTemplate temp = new TestTemplate(testID, test_name);

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

        /// <summary>
        /// Prevod aktualneho objektu na string 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public string ConvertToParsedString()
        {
            return null;
        }

        
        public override string ToString()
        {
            return "ID: " + ID + " Name: " + name + "Number of questions: " + questions.Count;
        }
    }
}