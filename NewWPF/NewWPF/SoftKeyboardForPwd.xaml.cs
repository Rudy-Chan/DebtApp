using System;
using System.Collections.Generic;
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

namespace NewWPF
{
    /// <summary>
    /// SoftKeyboardForPwd.xaml 的交互逻辑
    /// </summary>
    public partial class SoftKeyboardForPwd : UserControl
    {
        private static List<KeyValuePair<char, char>> Symbol = new List<KeyValuePair<char, char>>();
        private static char[] alphabet = { 'j', 'k', 'l', 'm', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'n', 'o' };
        private String valueString;

        internal String ValueString
        {
            get { return valueString; }
        }

        public SoftKeyboardForPwd()
        {
            InitializeComponent();
            SetBasicSymbol();
            valueString = string.Empty;
        }

        private void SetBasicSymbol()
        {
            if (Symbol == null)
            {
                Symbol = new List<KeyValuePair<char, char>>();
            }
            Symbol.Clear();
            Symbol.Add(new KeyValuePair<char, char>('`', '~'));
            Symbol.Add(new KeyValuePair<char, char>('1', '!'));
            Symbol.Add(new KeyValuePair<char, char>('2', '@'));
            Symbol.Add(new KeyValuePair<char, char>('3', '#'));
            Symbol.Add(new KeyValuePair<char, char>('4', '$'));
            Symbol.Add(new KeyValuePair<char, char>('5', '%'));
            Symbol.Add(new KeyValuePair<char, char>('6', '^'));
            Symbol.Add(new KeyValuePair<char, char>('7', '&'));
            Symbol.Add(new KeyValuePair<char, char>('8', '*'));
            Symbol.Add(new KeyValuePair<char, char>('9', '('));
            Symbol.Add(new KeyValuePair<char, char>('0', ')'));
            Symbol.Add(new KeyValuePair<char, char>('\\', '|'));
            Symbol.Add(new KeyValuePair<char, char>('[', '{'));
            Symbol.Add(new KeyValuePair<char, char>(']', '}'));
            Symbol.Add(new KeyValuePair<char, char>(';', ':'));
            Symbol.Add(new KeyValuePair<char, char>('\'', '\"'));
            Symbol.Add(new KeyValuePair<char, char>(',', '<'));
            Symbol.Add(new KeyValuePair<char, char>('.', '>'));
            Symbol.Add(new KeyValuePair<char, char>('/', '?'));
            Symbol.Add(new KeyValuePair<char, char>('-', '_'));
            Symbol.Add(new KeyValuePair<char, char>('=', '+'));

            RandomSort(ref Symbol);
            RandomSortForAlphabet(ref alphabet);
            ResetButtonContent();
        }

        //随机排序
        private void RandomSort<T>(ref List<T> list)
        {
            list = list.OrderBy(c => Guid.NewGuid()).ToList();
        }

        private void RandomSortForAlphabet(ref char[] data)
        {
            data = data.OrderBy(c => Guid.NewGuid()).ToArray();
        }

        private void ResetButtonContent()
        {
            int count = ButtonGrid.Children.Count;
            for (int i = 0; i < Symbol.Count; i++)
            {
                Button buttonTemp = ButtonGrid.Children[i] as Button;
                buttonTemp.Content = Symbol[i].Key.ToString();
            }
            for (int i = Symbol.Count, j = 0; i < count - 3; i++, j++)
            {
                Button buttonTemp = ButtonGrid.Children[i] as Button;
                buttonTemp.Content = alphabet[j].ToString();
            }
        }

        //通过判断按钮的content属性来做对应处理，以简化大量按钮的编程
        private void ButtonGrid_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)e.OriginalSource;    //获取click事件触发源，即按了的按钮
            if ((String)clickedButton.Content == "<-")
            {
                if (valueString.Length > 0)
                {
                    valueString = valueString.Substring(0, valueString.Length - 1);
                    Pbx_Password.Password = valueString;
                }
            }
            else if ((String)clickedButton.Content == "CapsLk")
            {
                int count = ButtonGrid.Children.Count;
                Btn_CapsLk.Background = ((ButtonGrid.Children[0] as Button).Content as String)[0] > 90 ? Brushes.White : Brushes.Blue;
                for (int i = Symbol.Count; i < count - 3; i++)
                {
                    Button buttonTemp = ButtonGrid.Children[i] as Button;
                    String contentTemp = buttonTemp.Content as String;
                    buttonTemp.Content = contentTemp[0] > 90 ? contentTemp.ToUpper() : contentTemp.ToLower();
                }
            }
            else if ((String)clickedButton.Content == "Shift")
            {
                String temp = (ButtonGrid.Children[0] as Button).Content as String;
                bool tag = Symbol.Exists(item => item.Value == temp[0]);
                Btn_Shift.Background = tag ? Brushes.White : Brushes.Blue;

                for (int i = 0; i < Symbol.Count; i++)
                {
                    Button buttonTemp = ButtonGrid.Children[i] as Button;
                    String contentTemp = buttonTemp.Content as String;
                    buttonTemp.Content = tag ? Symbol[i].Key.ToString() : Symbol[i].Value.ToString();
                }
            }
            //else if ((String)clickedButton.Content == "AC")
            //{
            //    tbValue.Text = "";
            //}
            //else if ((String)clickedButton.Content == "Enter")
            //{
            //    valueString = tbValue.Text;
            //    this.Close();
            //}
            else
            {
                valueString = (String)clickedButton.Content;
                Pbx_Password.Password += valueString;
            }
        }

        internal void Show()
        {
            throw new NotImplementedException();
        }

        private void Btn_SoftKeyboard_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonGrid.Visibility == Visibility.Visible)
                ButtonGrid.Visibility = Visibility.Hidden;
            else
                ButtonGrid.Visibility = Visibility.Visible;
        }
    }
}
