using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tester_server.Tester
{
    public class QuestionTemplate
    {
        public string question_text { get; private set; }
        public List<string> options { get; private set; }
        public List<string> correct_answers { get; private set; }
        public int ord_num { get; private set; }
        public QuestionTemplate(int ord_number, string question_text, List<string> options, List<string> correct_answers)
        {
            this.ord_num = ord_number;
            this.question_text = question_text;
            this.options = options;
            this.correct_answers = correct_answers;
        }
    }
}
