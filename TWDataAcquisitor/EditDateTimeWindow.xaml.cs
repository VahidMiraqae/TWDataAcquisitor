using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

namespace TWDataAcquisitor
{
    /// <summary>
    /// Interaction logic for EditDateTimeWindow.xaml
    /// </summary>
    public partial class EditDateTimeWindow : Window
    {

        public EditDateTimeWindow(byte[] imageBytes, string text1)
        {
            InitializeComponent();
            textBox.Text = text1; 
            image.Source = LoadImage(imageBytes);
        }

        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        public string EditedText { get; internal set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditedText = textBox.Text;
            Close();
        }
    }
}
