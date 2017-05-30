using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Registration_Form
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : INotifyPropertyChanged
    {         
        private string _statusBarString;

        public string StatusBarString
        {
            get { return _statusBarString; }
            set
            {
                _statusBarString = value;
                OnPropertyChanged("StatusBarString");
            }
        }

        #region Property changed event handler
        // property changed event
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion

        public Registration()
        {
            InitializeComponent();
            DataContext = this;
            StatusBarString = string.Empty;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionHelper.CheckName(name.Text) && ConnectionHelper.CheckSurname(surname.Text) &&
                ConnectionHelper.CheckEmail(email.Text) && ConnectionHelper.CheckUsername(username.Text) &&
                ConnectionHelper.CheckPassword(password1.Password, password2.Password))
            {
                try
                {
                    FocusManager.SetFocusedElement(this, null);

                    if (ConnectionHelper.RegisterUser(name.Text, surname.Text, email.Text, username.Text, password1.Password, password2.Password))
                    {
                        if(EmailClientHelper.SMTP != null)
                        { 
                            EmailClientHelper.SendEmail(email.Text, name.Text, surname.Text, username.Text, password1.Password);
                        }   

                        MessageBox.Show("Congratulations!\nYou are successfully registered!", "Registration", MessageBoxButton.OK, MessageBoxImage.Information,MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                        //Close();                              
                    }
                }
                catch (SqlException exception)
                {
                    MessageBox.Show("Database insert error:\n" + exception.Message, "Registration", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    StatusBarString = Properties.Settings.Default.StatusReady;
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }        
    }
}
