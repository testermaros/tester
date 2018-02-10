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
using tester_server.Connection.Authentification;
using Tester_server_GUI.Data;

namespace Tester_server_GUI.UI
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private string current;
        public ChangePasswordWindow(string current_username)
        {
            InitializeComponent();
            this.current = current_username;
        }

        private void ChangePassowdAction(object sender, RoutedEventArgs e)
        {
            string old_pass = oldPassword.Password.Trim();
            string new_password = newPassword.Password.Trim();
            string ret_new_password = retNewPassword.Password.Trim();
            //Ak je uzivatel prihlaseny nie je mozne zmenit heslo
            if (Dataset.Instance().login_manager.IsLoggedIn(current)) {
                MessageBox.Show("Nie je mozne zmenit heslo. Uzivatel je prihlaseny.");
                return;
            }
            //hesla sa zhoduju
            if (new_password.Equals(ret_new_password))
            {
                if (Dataset.Instance().login_manager.ChangePassword(current, old_pass, new_password)) {
                    MessageBox.Show("Heslo bolo zmenene.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Zadane heslo nie je spravne");
                }
            }
            else
            {
                MessageBox.Show("Hesla sa nezhoduju.");
            }   
        }
    }
}
