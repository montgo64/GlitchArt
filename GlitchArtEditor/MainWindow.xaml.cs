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
        private const int MAX_FILTERS = 5;

        public Image sourceImage;
        private ScaleTransform scaleTransform;
        private int numFilters;

        public MainWindow()
        {
            InitializeComponent();
            scaleTransform = new ScaleTransform();
            numFilters = 0;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a File";
            op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            if (op.ShowDialog() == true)
            {
                BitmapImage bit = new BitmapImage(new Uri(op.FileName));

                imgPhoto.Source = bit;
                imgPhoto.Width = bit.Width;
                imgPhoto.Height = bit.Height;
                imgPhoto.LayoutTransform = scaleTransform;

                sourceImage = imgPhoto;
            }
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Select a File";
            save.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";

            if (save.ShowDialog() == true)
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)sourceImage.Source));
                using (FileStream stream = new FileStream(save.FileName, FileMode.Create))
                    encoder.Save(stream);
            }
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sourceImage != null)
            {
                Binding bindX = new Binding();
                bindX.Source = ZoomSlider;
                bindX.Path = new PropertyPath("Value");
                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, bindX);

                Binding bindY = new Binding();
                bindY.Source = ZoomSlider;
                bindY.Path = new PropertyPath("Value");
                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, bindY);
            }

        }

        public void AddFilter(String filterType)
        {
            numFilters++;

            Button filter = (Button)this.FindName("Filter" + numFilters);
            filter.Content = filterType;
            filter.Visibility = Visibility.Visible;
        }

        public void RemoveFilter(String filterName)
        {
            Button filter = (Button)this.FindName(filterName);
            filter.Content = "";
            filter.Visibility = Visibility.Hidden;

            numFilters--;
        }

        private void FilterSelect(object sender, RoutedEventArgs e)
        {
            String filterType = "";
            String elementType = "";

            if (sender.GetType() == typeof(MenuItem))
            {
                if (numFilters + 1 > MAX_FILTERS)
                {
                    MessageBox.Show("Already have maximum number of filters.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                filterType = ((MenuItem)sender).Name.ToString();
                elementType = "MenuItem";
            }
            else if (sender.GetType() == typeof(Button))
            {
                filterType = ((Button)sender).Content.ToString();
                elementType = ((Button)sender).Name.ToString(); ;
            }
            OpenFilterWindow(filterType, elementType);
        }

        private void OpenFilterWindow(String type, String elementType)
        {
            FilterWindow winFilter = new FilterWindow(elementType);
            winFilter.Owner = this;
            winFilter.FilterTitle.Text = type;
            winFilter.Show();

        }
    }
}