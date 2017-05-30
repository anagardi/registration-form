using System;
using System.Windows.Controls;

namespace Registration_Form
{
    public class BirthDateField : DatePicker
    {
        private DateTime ? _oldValue = null;
        public bool DataChanged = false;

        public BirthDateField()
        {
            SelectedDateChanged += BirthDateField_SelectedDateChanged;            
        }

        private void BirthDateField_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DataChanged = (_oldValue != SelectedDate);
            _oldValue = SelectedDate;
        }
    }
}
