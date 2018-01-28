using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tester_server.Tester
{
    public class ResultForm
    {
        public Dictionary<int, List<string>> answers { get; private set; }

        public ResultForm()
        {
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
    }
}