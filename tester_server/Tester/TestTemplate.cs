using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tester_server.Tester
{
    public class TestTemplate
    {
        public List<QuestionTemplate> questions { get; private set; }
        public int ID { get; private set; }
        public TestTemplate(int ID)
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

        public void AddQuestions(List<QuestionTemplate> temp_list) {
            questions.AddRange(temp_list);
        }
    }
}
