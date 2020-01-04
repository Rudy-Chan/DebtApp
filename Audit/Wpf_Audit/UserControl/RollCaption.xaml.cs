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

namespace Wpf_Audit
{
    /// <summary>
    /// RollCaption.xaml 的交互逻辑
    /// </summary>
    public partial class RollCaption : UserControl
    {
        public RollCaption()
        {
            InitializeComponent();
        }

        private void Control_Caption_Loaded(object sender, RoutedEventArgs e)
        {
            double width = Control_Caption.Width;
            TextWidth.To = -width;
        }
    }
}
