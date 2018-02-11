using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using tester_server.Tester;
using Tester_server_GUI.Data;

namespace Tester_server_GUI.UI
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddTestWindow : Window
    {
        private int quest_ord_num = 0;

        public AddTestWindow()
        {
            InitializeComponent();
        }

        private void AddQuestionAction(object sender, RoutedEventArgs e)
        {
            AddQuestionWindow win = new AddQuestionWindow(quest_ord_num++);
            win.ShowDialog();
            QuestionTemplate temp = win.created;
            if(temp != null)
                questionList.Items.Add(temp);
            questionList.Items.Refresh();
        }

        private void RemoveQuestionAction(object sender, RoutedEventArgs e)
        {
            QuestionTemplate temp = (QuestionTemplate) questionList.SelectedItem;
            if (temp == null)
                return;
            questionList.Items.Remove(temp);
            questionList.Items.Refresh();
        }

        private void SaveTestAction(object sender, RoutedEventArgs e)
        {
            string name = test_name.Name.Trim();
            TestTemplate template = new TestTemplate(Dataset.Instance().TestID,name);
            template.AddQuestions(questionList.Items.OfType<QuestionTemplate>().ToList());
            Dataset.Instance().test_manager.AddTest(template);
            this.Close();
        }
    }
}
