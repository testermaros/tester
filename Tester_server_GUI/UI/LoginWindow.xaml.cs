using System.Windows;
using tester_server.Connection.Authentification;
using Tester_server_GUI.Data;
using Tester_server_GUI.UI;

namespace Tester_server_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginAction(object sender, RoutedEventArgs e)
        {
            string username = userName.Text.Trim();
            string password = pass_box.Password.Trim();
            //ak su prazdne
            if (username.Length == 0 || password.Length == 0)
                return;
            Account temp = Dataset.Instance().admin;
            //ak je heslo a meno spravne
            if (temp.password_hash == password.GetHashCode() && username == temp.user_name)
            {
                //nastav admina na prihlaseneho
                temp.Is_LogedOn = true;
                //zmen scenu
                AdminControlWindow admin_win = new AdminControlWindow();
                App.Current.MainWindow = admin_win;
                this.Close();
                admin_win.Show();
            }
            else {
                MessageBox.Show("Meno alebo sa nezhoduje");
            }
        }
    }
}
