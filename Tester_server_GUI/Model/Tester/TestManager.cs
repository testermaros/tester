using System.Collections.Generic;
using System.Linq;

namespace tester_server.Tester
{
    //len jedna mozna instancia triedy
    public class TestManager
    {
       
        //zoznam pouzitelnych tetov
        public List<TestTemplate> available_tests { get; private set; }

        public TestManager()
        {
            available_tests = new List<TestTemplate>();
        }

        public void AddTest(TestTemplate temp)
        {
            available_tests.Add(temp);
        }

        public void RemoveTest(int ID)
        {
            var found_test = from test in available_tests where test.ID == ID select test;
            available_tests.Remove(found_test.First());
        }

        public string GetTestsNames()
        {
            return (from test in available_tests select (test.ID + "." + test.name+"\n")).ToString();
        }

        public TestTemplate GetTest(int ID)
        {
            var selected = from test in available_tests where test.ID == ID select test;
            return selected.First();
        }

        public double Evaluate(ResultForm form)
        {
            //dummy
            return 0.0;

        }
    }
}
