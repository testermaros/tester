using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using tester_server.Connection.Authentification;
using tester_server.Tester;
using Tester_server_GUI.Data;

namespace Tester_server_GUI.UI
{
    /// <summary>
    /// Interaction logic for AdminControlWindow.xaml
    /// </summary>
    public partial class AdminControlWindow : Window
    {
        public AdminControlWindow()
        {
            InitializeComponent();
            UpdateUserData();
            UpdateTestData();
        }

        private void UpdateUserData()
        {
            List<Account> users = Dataset.Instance().login_manager.accounts;
            usersList.ItemsSource = users;
            usersList.Items.Refresh();
        }

        private void UpdateTestData()
        {
            testList.ItemsSource = Dataset.Instance().test_manager.available_tests;
        }

        private void AddUserAction(object sender, RoutedEventArgs e)
        {
            AddUserWindow win = new AddUserWindow();
            win.ShowDialog();
            UpdateUserData();
        }

        private void ChangePasswordAction(object sender, RoutedEventArgs e)
        {
            Account username = (Account)usersList.SelectedItem;
            if (username == null) return;
            ChangePasswordWindow chang = new ChangePasswordWindow(username.user_name);
            chang.ShowDialog();
            UpdateUserData();
        }

        private void RemoveUserAction(object sender, RoutedEventArgs e)
        {
            Account username = (Account) usersList.SelectedItem;
            if (username == null) return;
            if (!Dataset.Instance().login_manager.RemoveUser(username.user_name))
                MessageBox.Show("Uzivatela sa nepodarilo odstranit");
            UpdateUserData();
        }

        private void AddTestAction(object sender, RoutedEventArgs e)
        {
            AddTestWindow win = new AddTestWindow();
            win.ShowDialog();
            UpdateTestData();
        }

        private void EditTestAction(object sender, RoutedEventArgs e)
        {
            EditTestWindow win = new EditTestWindow();
            win.ShowDialog();
            UpdateTestData();
        }

        private void RemoveTestAction(object sender, RoutedEventArgs e)
        {
            TestTemplate test = (TestTemplate) testList.SelectedItem;
            if (test == null) return;
            Dataset.Instance().test_manager.RemoveTest(test.ID);
            UpdateTestData();
        }

        private void testList_SelectionChanged(object sender, MouseButtonEventArgs e ) {
            TestTemplate test = (TestTemplate)testList.SelectedItem;
            if (test == null) return;
            DetailsWindow win = new DetailsWindow(test);
            win.ShowDialog();
            UpdateTestData();
        }

        private void DetailsTestAction(object sender, RoutedEventArgs e)
        {
            TestTemplate test = (TestTemplate)testList.SelectedItem;
            if (test == null) return;
            DetailsWindow win = new DetailsWindow(test);
            win.ShowDialog();
            UpdateTestData();
        }
    }
}
