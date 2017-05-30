using System.Windows;

namespace Registration_Form
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Host.DataChanged && !Port.DataChanged && !Address.DataChanged && !Password.DataChanged)
            {
                this.Close();
            }

            if (ConnectionHelper.CheckHost(Host.Text) &&
                ConnectionHelper.CheckPort(Port.Text) &&
                ConnectionHelper.CheckEmail(Address.Text) &&
                ConnectionHelper.CheckPassword(Password.Text))
            {
                EmailClientHelper.WriteConfigurations(Host.Text, Port.Text, Address.Text, Password.Text);
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EmailClientHelper.ReadConfigurations();
            Host.Text = EmailClientHelper.Host;
            Port.Text = EmailClientHelper.Port == 0 ? string.Empty : EmailClientHelper.Port.ToString();
            Address.Text = EmailClientHelper.FromAddress == null ? string.Empty : EmailClientHelper.FromAddress.Address;
            Password.Text = EmailClientHelper.FromPassword;
        }
    }
}
