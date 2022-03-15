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

namespace TWDataAcquisitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainVm _mainVm;

        public MainWindow()
        {
            InitializeComponent();
            _mainVm = new MainVm();
            DataContext = _mainVm;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                e.Handled = false;
                this.DragMove();
            }
        }

        private async void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            try
            {
                await _mainVm.Start();
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("error.txt", ex.ToString());
            }
        }

        private async void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            await _mainVm.Stop();
        }

        private async void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            await _mainVm.OpenChartDataDir();
        }

        private async void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            await _mainVm.StartCorrectionGannFinder();
        }
    }
}
