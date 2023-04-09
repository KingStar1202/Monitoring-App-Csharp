using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Camera.context;
using Camera.repository;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;

using Telerik.Windows.Controls;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Camera
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : RadRibbonWindow
    {
        private LoginRepository loginRepository = LoginRepository.getInstance();
        static Login()
        {
            Login.IsWindowsThemeEnabled = false;
        }

        public Login()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                // Call the login button click event
                login_Click(sender, e);
            }
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {

            string username = radWatermarkTextBox.Text;
            string password = RadPasswordBox.Password;


            bool loginOK = false;
            try
            {
                loginOK = loginRepository.isUserAndPasswordCorrect(username, password);
                if (loginOK)
                {
                    loginRepository.saveLogin(username);
                    string role = loginRepository.getUserRole(username);
                    if (role == "admin")
                    {
                        AdditionalSetup mainWindow = new AdditionalSetup(username);
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        
                        new CameraOverView(username).Show();
                        this.Close();

                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Wrong User Name and Password!!!");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
