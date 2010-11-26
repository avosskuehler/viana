using System;
using System.Windows;
using System.Windows.Controls;

namespace VianaNET
{
    public partial class DualTextBox : UserControl
    {
        public bool IsFirstTextActive
        {
            get
            {
                return _isFirstTextActive;
            }
            set
            {
                _isFirstTextActive = value;
                UpdateIsEnabled();
            }
        }
        private bool _isFirstTextActive;

        public string TextA
        {
            get { return ElementTextBoxA.Text; }
            set { ElementTextBoxA.Text = value; }
        }

        public string TextB
        {
            get { return ElementTextBoxB.Text; }
            set { ElementTextBoxB.Text = value; }
        }

        public bool IsChecked
        {
            get { return ElementCheckBox.IsChecked(); }
            set { ElementCheckBox.IsChecked = value; }
        }

        public event EventHandler TextChanged;

        public event EventHandler CheckedChanged;

        public DualTextBox()
        {
            InitializeComponent();
            UpdateIsEnabled();
        }

        private void UpdateIsEnabled()
        {
            ElementTextBoxA.IsEnabled = IsChecked && _isFirstTextActive;
            ElementTextBoxB.IsEnabled = IsChecked;
        }

        private void ElementCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateIsEnabled();
            OnCheckedChanged();
        }

        private void ElementTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnTextChanged();
        }

        private void ElementTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
        }

        private void OnCheckedChanged()
        {
            CheckedChanged.InvokeEmpty(this);
        }

        private void OnTextChanged()
        {
            TextChanged.InvokeEmpty(this);
        }
    }
}
