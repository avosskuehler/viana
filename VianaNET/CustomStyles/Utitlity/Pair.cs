using System;
using System.ComponentModel;
using System.Globalization;

namespace VianaNET
{
    public class Pair : INotifyPropertyChanged
    {
        public object First
        {
            get
            {
                return _first;
            }
            set
            {
                if (!object.Equals(_first, value))
                {
                    _first = ConvertedValue(value);
                    OnPropertyChanged("First");
                }
            }
        }
        private object _first;

        public object Second
        {
            get
            {
                return _second;
            }
            set
            {
                if (!object.Equals(_second, value))
                {
                    _second = ConvertedValue(value);
                    OnPropertyChanged("Second");
                }
            }
        }
        private object _second;

        private static object ConvertedValue(object value)
        {
            object convertedValue = value;
            var valueString = value as string;
            if(null != valueString)
            {
                double valueDouble;
                DateTime valueDateTime;
                if (double.TryParse(valueString, NumberStyles.Number, CultureInfo.InvariantCulture, out valueDouble))
                {
                    convertedValue = valueDouble;
                }
                else if (DateTime.TryParse(valueString, CultureInfo.InvariantCulture, DateTimeStyles.None, out valueDateTime))
                {
                    convertedValue = valueDateTime;
                }
            }
            return convertedValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (null != handler)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
