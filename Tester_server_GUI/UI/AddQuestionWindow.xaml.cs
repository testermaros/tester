using System.ComponentModel;
using System.Windows;
using tester_server.Tester;

namespace Tester_server_GUI.UI
{
    /// <summary>
    /// Interaction logic for AddQuestionWindow.xaml
    /// </summary>
    public partial class AddQuestionWindow : Window
    {
        public QuestionTemplate created { get; set; }
        public AddQuestionWindow( int num)
        {
            InitializeComponent();
        }
    }

    public class CheckedListItem<T> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isChecked;
        private T item;

        public CheckedListItem()
        { }

        public CheckedListItem(T item, bool isChecked = false)
        {
            this.item = item;
            this.isChecked = isChecked;
        }

        public T Item
        {
            get { return item; }
            set
            {
                item = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Item"));
            }
        }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }
    }
}
