using System;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Windows;
using System.Windows.Controls;

namespace Registration_Form
{
    /// <summary>
    /// Interaction logic for PersonalData.xaml
    /// </summary>
    public partial class PersonalData : Window, INotifyPropertyChanged
    {
        private string _userName;
        private string _password;

        private string _statusBarString;

        public string StatusBarString
        {
            get { return _statusBarString; }
            set
            {
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

        public PersonalData(string userFirstName,
                            string userSurname,
                            DateTime userBirthDate,
                            string userAddress,
                            string userEmail,
                            string userPhone,
                            string userName,
                            string password)
        {
            InitializeComponent();
            DataContext = this;

            firstName.Text = userFirstName;
            surname.Text = userSurname;

            if (userBirthDate > SqlDateTime.MinValue.Value)
            {
                birth_date.SelectedDate = userBirthDate;
            }

            address.Text = userAddress;
            email.Text = userEmail;
            phone.Text = userPhone;

            _userName = userName;
            _password = password;

            StatusBarString = string.Empty;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (firstName.DataChanged || surname.DataChanged || birth_date.DataChanged || address.DataChanged || email.DataChanged || phone.DataChanged)
            {
                if (ConnectionHelper.CheckName(firstName.Text) && ConnectionHelper.CheckSurname(surname.Text) && ConnectionHelper.CheckEmail(email.Text) && ConnectionHelper.CheckPhoneNumber(phone.Text))
                {
                    try
                    {
                        ConnectionHelper.UpdateUserData(firstName.Text, surname.Text, birth_date.SelectedDate, address.Text, email.Text, phone.Text, _userName, _password);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database error:\n" + ex.Message, "Personal data update", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        firstName.DataChanged = surname.DataChanged = birth_date.DataChanged = address.DataChanged = email.DataChanged = phone.DataChanged = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("There is no changes in your personal data!", "Personal data update", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            StatusBarString = Properties.Settings.Default.StatusReady;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }      
    }
}
