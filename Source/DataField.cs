using System.Windows.Controls;

namespace Registration_Form
{
    public class DataField : TextBox
    {
        private string _oldValue = string.Empty;
        public bool DataChanged = false;

        public DataField()
        {
            GotFocus += DataField_GotFocus;
            LostFocus += DataField_LostFocus;
        }

        private void DataField_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            _oldValue = Text;
        }

        private void DataField_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {          
            DataChanged = (_oldValue != Text);            
        }
    }
}
