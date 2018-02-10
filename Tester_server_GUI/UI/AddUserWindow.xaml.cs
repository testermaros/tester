using System;
using System.Collections.Generic;
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
using Tester_server_GUI.Data;

namespace Tester_server_GUI.UI
{
    /// <summary>
    /// Interaction logic for AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
        }

        private void AddUserAction(object sender, RoutedEventArgs e)
        {
            string username = user_name.Text.Trim();
            string _password = password.Password.Trim();
            string ret_password = retypePassword.Password.Trim();
            //ak je kolonka nevyplnena
            if (username.Length == 0 || _password.Length == 0 || ret_password.Length == 0) {
                MessageBox.Show("Je nutne vyplnit vsetky okna");
                return;
            }
            if (!_password.Equals(ret_password)) {
                MessageBox.Show("Hesla sa nezhoduju");
                return;
            }
            Dataset.Instance().login_manager.AddUser(username,_password);
            //uzatvorenie okna
            this.Close();
        }
    }
}
