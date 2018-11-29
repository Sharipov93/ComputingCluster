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
using System.Windows.Shapes;

namespace ControllerNode.View
{
    /// <summary>
    /// Логика взаимодействия для ComputeNodeWindow.xaml
    /// </summary>
    public partial class ComputeNodeWindow : Window
    {
        private Controller _context;

        public ComputeNodeWindow(Controller context)
        {
            InitializeComponent();
            DataContext = _context = context;
        }

        /// <summary>
        /// Событие для проверки ввода пароля
        /// </summary>
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_context.FirstIpPart) || string.IsNullOrEmpty(_context.SecondIpPart)
                || string.IsNullOrEmpty(_context.ThirdIpPart) || string.IsNullOrEmpty(_context.FourthIpPart))
            {
                new InformationMessageBox("Пожалуйста, введите ip - адрес узла").ShowDialog();
                return;
            }

            DialogResult = true;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
