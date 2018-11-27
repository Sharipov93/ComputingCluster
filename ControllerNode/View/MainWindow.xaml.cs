using ControllerNode.View;
using ControllerNode.ViewModel;
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

namespace ControllerNode
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new Controller();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var exitWindow = new ExitWindow
            {
                Owner = this
            };

            if (!(bool)exitWindow.ShowDialog())
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
