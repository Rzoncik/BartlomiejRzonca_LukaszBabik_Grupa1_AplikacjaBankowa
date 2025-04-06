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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace app_bank_register
{
    public partial class PageLogin : Page
    {
        public PageLogin()
        {
            InitializeComponent();
        }

        private void btn_validate_login_Click(object sender, RoutedEventArgs e)
        {
            var mainPanel = new MainPanel();
            string id = inputId.Text;
            string pass = inputPass.Password;

            if (id == "" || pass == "")
            {
                MessageBox.Show("Nie wprowadzono wszystkich danych", "Porażka", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (id == "admin" && pass == "123")
            {
                mainPanel.Show();
                Window MainWindow = Window.GetWindow(this);
                MainWindow.Close();
            }
            else
            {
                MessageBox.Show("Wprowadzono niepoprawne dane admina", "Porażka", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new PageRegister());
        }
    }
}
