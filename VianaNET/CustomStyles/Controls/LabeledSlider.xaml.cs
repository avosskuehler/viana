using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace VianaNET
{
    public partial class LabeledSlider : UserControl
    {
        public string Label
        {
            get { return ElementLabel.Text; }
            set { ElementLabel.Text = value; }
        }

        public double Minimum
        {
            get { return ElementSlider.Minimum; }
            set { ElementSlider.Minimum = value; }
        }

        public double Maximum
        {
            get { return ElementSlider.Maximum; }
            set { ElementSlider.Maximum = value; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Not easily confused with DependencyObject.GetValue().")]
        public double Value
        {
            get { return _value; }
            set { ElementSlider.Value = value; }
        }
        private double _value;

        public int Decimals
        {
            get { return _decimals; }
            set
            {
                _decimals = value;
                OnValueChanged(false);
            }
        }
        private int _decimals;

        public bool IsCheckable
        {
            get { return (Visibility.Visible == ElementCheckBox.Visibility); }
            set
            {
                ElementCheckBox.Visibility = (value ? Visibility.Visible : Visibility.Collapsed);
                ElementSlider.IsEnabled = !IsCheckable || IsChecked;
            }
        }

        public bool IsChecked
        {
            get { return ElementCheckBox.IsChecked(); }
            set { ElementCheckBox.IsChecked = value; }
        }

        public event EventHandler ValueChanged;

        public event EventHandler CheckedChanged;

        public LabeledSlider()
        {
            InitializeComponent();
            ElementSlider.IsEnabled = !IsCheckable || IsChecked;
            OnValueChanged(true);
        }

        private void ElementCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ElementSlider.IsEnabled = ElementCheckBox.IsChecked();
            OnCheckedChanged();
        }

        private void ElementSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OnValueChanged(false);
        }

        private void OnCheckedChanged()
        {
            CheckedChanged.InvokeEmpty(this);
        }

        private void OnValueChanged(bool force)
        {
            var oldValue = _value;
            _value = Math.Round(ElementSlider.Value, _decimals);
            if ((oldValue != _value) || force)
            {
                ElementValue.Text = _value.ToString(CultureInfo.CurrentCulture);
                ValueChanged.InvokeEmpty(this);
            }
        }
    }
}
