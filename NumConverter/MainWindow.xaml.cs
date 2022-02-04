using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RadioButton? checkedRadioButton = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void radioButton_Click(object sender, RoutedEventArgs e)
        {
            if((sender as RadioButton) is RadioButton radioButton)
            {
                checkedRadioButton = radioButton;
            }
        }

        private void calcButton_Click(object sender, RoutedEventArgs e)
        {
            if(checkedRadioButton as RadioButton is RadioButton button)
            {
                bool isValid = System.Numerics.BigInteger.TryParse(inputBox.Text, out System.Numerics.BigInteger parsedInt);
                if (!isValid)
                {
                    isValid = Decimal.TryParse(inputBox.Text, out decimal parsedVal);
                    if (isValid)
                    {
                        MessageBox.Show("Floating point conversion is not currently supported");
                    } else
                    {
                        MessageBox.Show("Input value is invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // from https://stackoverflow.com/questions/14048476/biginteger-to-hex-decimal-octal-binary-strings
                    switch (button.Name)
                    {
                        case "hexRadioButton":
                            resultBox.Text = parsedInt.ToString("X");
                            break;
                        case "binRadioButton":
                            {
                                var bytes = parsedInt.ToByteArray();
                                var idx = bytes.Length - 1;

                                var base2 = new StringBuilder(bytes.Length * 8);
                                var binary = Convert.ToString(bytes[idx], 2);

                                base2.Append(binary);

                                for (idx--; idx >= 0; idx--)
                                {
                                    base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
                                }

                                resultBox.Text = base2.ToString();
                                break;
                            }
                        case "octRadioButton":
                            {
                                var bytes = parsedInt.ToByteArray();
                                var idx = bytes.Length - 1;

                                var base8 = new StringBuilder((bytes.Length / 3 + 1) * 8);
                                var extra = bytes.Length % 3;
                                if (extra == 0) extra = 3;

                                int int24 = 0;
                                for (; extra != 0; extra--)
                                {
                                    int24 <<= 8;
                                    int24 += bytes[idx--];
                                }
                                
                                var octal = Convert.ToString(int24, 8);
                                if(octal[0] != 0 && parsedInt.Sign == 1)
                                    base8.Append('0');
                                base8.Append(octal);

                                for(; idx >= 0; idx -= 3)
                                {
                                    int24 = (bytes[idx] << 16) + (bytes[idx - 1] << 8) + bytes[idx - 2];
                                    base8.Append(Convert.ToString(int24, 8).PadLeft(8, '0'));
                                }

                                resultBox.Text = base8.ToString();
                                break;
                            }
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("Conversion Type not specified.");
            }
        }

        private void inputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // from https://stackoverflow.com/questions/4085471/allow-only-numeric-entry-in-wpf-text-box/4085607
            if((sender as TextBox) is TextBox inputBox) 
            {
                var selectionStart = inputBox.SelectionStart;
                var newText = new StringBuilder();
                var hasPoint = false;

                foreach(var c in inputBox.Text.ToCharArray())
                {
                    if(Char.IsDigit(c) || Char.IsControl(c) || (c == '.' && !hasPoint))
                    {
                        newText.Append(c);
                        if(c == '.')
                            hasPoint = true;
                    }
                }

                inputBox.Text = newText.ToString();
                inputBox.SelectionStart = selectionStart <= inputBox.Text.Length ? selectionStart : inputBox.Text.Length;
            } 
            else
            {
                throw new InvalidOperationException("Input box should be a object that can be referred to.");
            }
        }
    }
}
