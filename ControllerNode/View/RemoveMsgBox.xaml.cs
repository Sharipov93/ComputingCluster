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
using System.Windows.Shapes;

namespace ControllerNode.View
{
    /// <summary>
    /// Логика взаимодействия для RemoveMsgBox.xaml
    /// </summary>
    public partial class RemoveMsgBox : Window
    {
        public RemoveMsgBox(string message)
        {
            InitializeComponent();
            exceptionTxt.Text = message;
        }

        private void Button_Click_Ok(object sender, RoutedEventArgs e) => DialogResult = true;

        private void Button_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
