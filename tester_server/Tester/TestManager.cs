using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tester_server.Tester
{
    class TestManager
    {
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
            return (from test in available_tests select (test.ID + "." + test.name)).ToString();
        }
    }
}
