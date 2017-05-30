using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.Windows;

namespace Registration_Form
{
    class ConnectionHelper
    {
        private static SqlConnection _connection;

        private static void OpenConnection()
        {
            string connectionString = Properties.Settings.Default.UsersdataConnectionString;
            _connection = new SqlConnection(connectionString);
            try
            {
                _connection.Open();
            }
            catch (SqlException exception)
            {
                MessageBox.Show("An error occured while performing operation:\n" + exception, "Connection to server...", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                try
                {
                    _connection.Close();
                }
                catch (SqlException exception)
                {
                    MessageBox.Show("An error occured while performing operation:\n" + exception.Message, "Connection to server...", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static bool RegisterUser(string name,
                                         string surname,
                                         string email,
                                         string username,
                                         string password1,
                                         string password2)
        {
            try
            {
                OpenConnection();

                using (
                    SqlCommand command =
                        new SqlCommand(
                            "INSERT INTO users(name,surname,email,username,password) VALUES(@NAME,@SURNAME,@EMAIL,@USERNAME,@PASSWORD)",
                            _connection))
                {
                    command.Parameters.AddWithValue("@NAME", name);
                    command.Parameters.AddWithValue("@SURNAME", surname);
                    command.Parameters.AddWithValue("@EMAIL", email);
                    command.Parameters.AddWithValue("@USERNAME", username);
                    command.Parameters.AddWithValue("@PASSWORD", password1);

                    int result = command.ExecuteNonQuery();

                    return (result == 1);
                }
            }
            catch (SqlException exception)
            {
                if (exception.Number == 2601) // Cannot insert duplicate key row in object error
                {
                    // handle duplicate key error
                    MessageBox.Show("An error occured while performing operation:\nCannot insert duplicate key row.", "Registration form", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                else
                    throw; // throw exception if this exception is unexpected                
            }
            finally
            {
                CloseConnection();
            }
        }

        public static bool GetRegisteredUser(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                MessageBox.Show("The username or password is empty!", "Login", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                OpenConnection();

                using (
                    SqlCommand command =
                        new SqlCommand(
                               "SELECT * FROM users WHERE username=@username and password=@password",
                               _connection))
                {
                    command.Parameters.AddWithValue("@USERNAME", username);
                    command.Parameters.AddWithValue("@PASSWORD", password);

                    SqlDataReader sqlDataReader = command.ExecuteReader();

                    if (sqlDataReader.HasRows)
                    {
                        string firstName = string.Empty;
                        string surname = string.Empty;
                        DateTime birthDate = SqlDateTime.MinValue.Value;
                        string address = string.Empty;
                        string email = string.Empty;
                        string phone = string.Empty;

                        while (sqlDataReader.Read())
                        {
                            firstName = sqlDataReader["name"] == DBNull.Value ? string.Empty : (string)sqlDataReader["name"];
                            surname = sqlDataReader["surname"] == DBNull.Value ? string.Empty : (string)sqlDataReader["surname"];
                            birthDate = sqlDataReader["birth_date"] == DBNull.Value ? SqlDateTime.MinValue.Value : (DateTime)sqlDataReader["birth_date"];
                            address = sqlDataReader["address"] == DBNull.Value ? string.Empty : (string)sqlDataReader["address"];
                            email = sqlDataReader["email"] == DBNull.Value ? string.Empty : (string)sqlDataReader["email"];
                            phone = sqlDataReader["phone"] == DBNull.Value ? string.Empty : (string)sqlDataReader["phone"];
                        }

                        PersonalData personalData = new PersonalData(firstName,
                                                                     surname,
                                                                     birthDate,
                                                                     address,
                                                                     email,
                                                                     phone,
                                                                     username,
                                                                     password);

                        personalData.ShowDialog();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("The username or password is incorrect!", "Login", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
            catch (SqlException exception)
            {
                MessageBox.Show("An error occured while performing operation:\n" + exception.Message, "Login", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public static bool UpdateUserData(string name,
                                          string surname,
                                          DateTime? birth_date,
                                          string address,
                                          string email,
                                          string phone,
                                          string username,
                                          string password)
        {
            try
            {
                OpenConnection();

                using (
                    SqlCommand command =
                        new SqlCommand(
                             "Update users SET name=@name,surname=@surname,birth_date=@birth_date,address=@address,email=@email,phone=@phone WHERE username=@username and password=@password",
                            _connection))
                {
                    command.Parameters.AddWithValue("@NAME", name == null ? string.Empty : name);
                    command.Parameters.AddWithValue("@SURNAME", surname == null ? string.Empty : surname);
                    command.Parameters.AddWithValue("@BIRTH_DATE", birth_date == null ? SqlDateTime.MinValue.Value : birth_date);
                    command.Parameters.AddWithValue("@ADDRESS", address == null ? string.Empty : address);
                    command.Parameters.AddWithValue("@EMAIL", email == null ? string.Empty : email);
                    command.Parameters.AddWithValue("@PHONE", phone == null ? string.Empty : phone);
                    command.Parameters.AddWithValue("@USERNAME", username);
                    command.Parameters.AddWithValue("@PASSWORD", password);

                    command.ExecuteNonQuery();

                    MessageBox.Show("Your data has been successfully updated!", "Personal data update", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (SqlException exception)
            {
                MessageBox.Show("An error occured while performing operation:\n" + exception.Message, "Personal data update", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CloseConnection();
            }
            return false;
        }

        public static Boolean CheckName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("The name is empty!", "Register", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            return true;
        }

        public static Boolean CheckSurname(string surname)
        {
            if (String.IsNullOrEmpty(surname))
            {
                MessageBox.Show("The surname is empty!", "Register", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            return true;
        }

        public static Boolean CheckEmail(string emailString)
        {
            if (String.IsNullOrEmpty(emailString))
            {
                var result = MessageBox.Show("The email address is empty!", "Register", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                if (result == MessageBoxResult.OK)
                { return false; }
                return true;
            }
            if (!Regex.IsMatch(emailString, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                MessageBox.Show("Check the email address!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            return true;
        }

        public static Boolean CheckUsername(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                MessageBox.Show("The username is empty!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            if (username.Length < 6)
            {
                MessageBox.Show("Username must be at least 6 characters long!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        public static Boolean CheckPassword(string pswString1, string pswString2)
        {
            if (String.IsNullOrEmpty(pswString1))
            {
                MessageBox.Show("The password is empty!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            if (String.IsNullOrEmpty(pswString2))
            {
                MessageBox.Show("Re-enter the password!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            if (pswString1 != pswString2)
            {
                MessageBox.Show("Re-enter the password!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            return true;
        }

        public static Boolean CheckPhoneNumber(string phoneNumber)
        {
            if (!Regex.IsMatch(phoneNumber, @"^[+]?([0-9]*[\.\s\-\(\)]|[0-9]+){3,24}$", RegexOptions.IgnoreCase))
            {
                MessageBox.Show("Check the phone number!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            return true;
        }

        public static Boolean CheckHost(string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                MessageBox.Show("The host address is empty!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            if (Uri.CheckHostName(host) == UriHostNameType.Unknown)
            {
                MessageBox.Show("Host address is not valid!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        public static Boolean CheckPort(string port)
        {
            if (string.IsNullOrEmpty(port))
            {
                MessageBox.Show("The port number is empty!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }

            Regex numeric = new Regex(@"^[0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (numeric.IsMatch(port))
            {
                try
                {
                    if (Convert.ToInt32(port) < 65536)
                        return true;
                }
                catch (OverflowException)
                {
                }
            }
            MessageBox.Show("Port number is not valid!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        public static Boolean CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("The password is empty!", "Registratioin form", MessageBoxButton.OK, MessageBoxImage.Error, 0, MessageBoxOptions.DefaultDesktopOnly);
                return false;
            }
            return true;
        }
    }
}
