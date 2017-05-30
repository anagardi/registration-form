using System;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Windows;

namespace Registration_Form
{
    class EmailClientHelper
    {
        private static MailAddress ToAddress;
        public static MailAddress FromAddress;

        public static string FromPassword;

        public static string Host;
        public static int Port;

        public static SmtpClient SMTP;

        const string _subject = "Registration";

        public static bool WriteConfigurations(string host, string port, string address, string password)
        {
            string appRoot = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = System.IO.Path.Combine(appRoot, "config.ini");

            if (System.IO.File.Exists(filePath))
            {
                Configuration.WriteValue("SmtpClient", "Host", host, filePath);
                Configuration.WriteValue("SmtpClient", "Port", port, filePath);
                Configuration.WriteValue("MailCredentials", "Address", address, filePath);
                Configuration.WriteValue("MailCredentials", "Password", password, filePath);
                return true;
            }

            MessageBox.Show("Configuration file not found!");
            return false;
        }
        public static void ReadConfigurations()
        {
            string appRoot = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = System.IO.Path.Combine(appRoot, "config.ini");

            if (System.IO.File.Exists(filePath))
            {
                Host = Configuration.ReadValue("SmtpClient", "Host", filePath);
                string portString = Configuration.ReadValue("SmtpClient", "Port", filePath);  
                string addressString = Configuration.ReadValue("MailCredentials", "Address", filePath); 
                FromPassword = Configuration.ReadValue("MailCredentials", "Password", filePath);

                if (!string.IsNullOrEmpty(portString))
                {
                    Port = int.Parse(portString);
                }

                if (string.IsNullOrEmpty(Host) ||
                    string.IsNullOrEmpty(portString) ||
                    string.IsNullOrEmpty(addressString) ||
                    string.IsNullOrEmpty(FromPassword))
                {
                    return;
                }                

                FromAddress = new MailAddress(addressString, "Registration Form");

                SMTP = new SmtpClient
                {
                    Host = Host,
                    Port = Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(FromAddress.Address, FromPassword)
                };
                return;
            }
            MessageBox.Show("Configuration file not found!");
        }


        public static void SendEmail(string toaddress, string firstname, string lastname, string username, string password)
        {
            ToAddress = new MailAddress(toaddress, firstname + " " + lastname);
            string body = String.Format("Dear {0} {1},\n\nThank you for registering!\n\nUsername: {2}\nPassword: {3}\n\nBest regards,\nTeam Registraion Form", firstname, lastname, username, password);

            using (var message = new MailMessage(FromAddress, ToAddress)
            {
                Subject = _subject,
                Body = body
            })
                try
                {
                    SMTP.Send(message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Sending email", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
    }
}
