using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Registration_Form
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : INotifyPropertyChanged
    {
        #region Private properties       

        private SolidColorBrush _regFontColor = new SolidColorBrush(Colors.Blue);
        private FontWeight _regFontWeight;
        private Cursor _regMouseCursor;
        private string _statusBarString;

        #endregion

        #region Properties
        public SolidColorBrush RegFontColor
        {
            get { return _regFontColor; }
            set
            {
                _regFontColor = value;
                OnPropertyChanged("RegFontColor");
            }
        }

        public FontWeight RegFontWeight
        {
            get { return _regFontWeight; }
            set
            {
                _regFontWeight = value;
                OnPropertyChanged("RegFontWeight");
            }
        }

        public Cursor RegMouseCursor
        {
            get { return _regMouseCursor; }
            set
            {
                _regMouseCursor = value;
                OnPropertyChanged("RegMouseCursor");
            }
        }

        public string StatusBarString
        {
            get { return _statusBarString; }
            set
            {
                _statusBarString = value;
                OnPropertyChanged("StatusBarString");
            }
        }

        #endregion

        public Login()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Mouse events

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionHelper.GetRegisteredUser(username.Text, passwordBox.Password))
            {
                ResetLoginData();
            }
        }

        private void RegisterMouseEnter(object sender, MouseEventArgs e)
        {
            SetProperties(new SolidColorBrush(Colors.Chartreuse), FontWeights.Bold, Cursors.Hand);
        }

        private void RegisterMouseLeave(object sender, MouseEventArgs e)
        {
            SetProperties(new SolidColorBrush(Colors.Blue), FontWeights.Normal, Cursors.Arrow);
        }

        private void RegisterMouseDown(object sender, MouseButtonEventArgs e)
        {
            ResetLoginData();
            ShowRegistrationWindow();
        }  
        #endregion

        #region Utilities

        private static void ShowRegistrationWindow()
        {
            Registration registration = new Registration();
            registration.ShowDialog();
        }

        private void SetProperties(SolidColorBrush color, FontWeight fontweight, Cursor cursor)
        {
            RegFontColor = color;
            RegFontWeight = fontweight;
            RegMouseCursor = cursor;
        }

        private void ResetLoginData()
        {
            username.Text = passwordBox.Password = string.Empty;
        }

        #endregion

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

        #region Menu

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            ShowRegistrationWindow();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            Window settings = new Settings();
            settings.ShowDialog();
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            string appRoot = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = System.IO.Path.Combine(appRoot, "Help.chm");
            if (!System.IO.File.Exists(filePath))
            {
                // The path to the Help folder.
                string directory = System.IO.Path.Combine(appRoot, @"../..");
                // The path to the Help file.
                filePath = System.IO.Path.Combine(directory, "Help/Help.chm");
            }
            // Launch the Help file.
            try
            {
                Process.Start(filePath);
            }
            catch (Exception)
            {
                MessageBox.Show("Help.chm file not found:\n", "Show Help file", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            using (AboutBox aboutBox = new AboutBox())
            {
                aboutBox.ShowDialog();
            }
        }

        #endregion

        #region Keyboard events

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                switch (e.Key)
                {
                    //handle L key
                    case Key.L:
                    Login_Click(sender, e);
                    break;                    
                    //handle R key
                    case Key.R:
                    ShowRegistrationWindow();
                    break;
                    default:
                    e.Handled = true;
                    break;
                }
            }
            //handle F1 key
            if (e.Key == Key.F1)
            {
                MenuItemHelp_Click(sender, e);
                e.Handled = true;
            }
        }

        private void register_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SetProperties(new SolidColorBrush(Colors.Chartreuse), FontWeights.Bold, Cursors.Arrow);
        }

        private void register_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            SetProperties(new SolidColorBrush(Colors.Blue), FontWeights.Normal, Cursors.Arrow);
        }

        private void register_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ShowRegistrationWindow();
            }
        }

        private void enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Click(sender, e);
            }
        }

        #endregion

        
    }
}