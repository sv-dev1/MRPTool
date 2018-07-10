using MRPTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FedexTracking
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        private void btn_print_Click_1(object sender, RoutedEventArgs e)
        {
            //if(Date )

            if (string.IsNullOrEmpty(txtUsername.Text) && (!string.IsNullOrEmpty(txtPassword.Password)))
            {
                System.Windows.MessageBox.Show("Please Enter Username ");
            }
            else if (!(string.IsNullOrEmpty(txtUsername.Text)) && string.IsNullOrEmpty(txtPassword.Password))
            {
                System.Windows.MessageBox.Show("Please Enter Password");
            }

            else if (string.IsNullOrEmpty(txtUsername.Text) && string.IsNullOrEmpty(txtPassword.Password))
            {
                System.Windows.MessageBox.Show("Please Enter Username and Password");
            }
            else if (txtUsername.Text != "admin" || txtPassword.Password != "admin123")
            {
                System.Windows.MessageBox.Show("Invalid Credentials.");
            }
            else if (txtUsername.Text == "admin" && txtPassword.Password == "admin123")
            {
                UnleashedWindow obj = new UnleashedWindow();
                obj.Show();
                this.Close();
            }


        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.btn_print.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
