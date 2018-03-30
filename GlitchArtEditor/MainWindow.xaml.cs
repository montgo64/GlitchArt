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

using System.IO;

using Microsoft.Win32;

namespace GlitchArtEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ImportImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            if (op.ShowDialog() == true)
            {
                BitmapImage bit = new BitmapImage(new Uri(op.FileName));
                imgPhoto.Source = bit;
                imgPhoto.Width = bit.Width;
                imgPhoto.Height = bit.Height;
            }
        }

        private void ExportImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Select a picture";
            save.Filter ="JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            
            if (save.ShowDialog() == true)
            {
                //SaveUsingEncoder(imgPhoto, save.FileName);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgPhoto.Source));
                using (FileStream stream = new FileStream(save.FileName, FileMode.Create))
                    encoder.Save(stream);
            }
        }
    }
}