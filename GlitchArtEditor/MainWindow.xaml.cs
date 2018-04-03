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
using EchoEffect;

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

                Echo test = new Echo();
                //sourceImage.Source;

                imgPhoto.Source = bit;
                imgPhoto.Width = bit.Width;
                imgPhoto.Height = bit.Height;
                imgPhoto.LayoutTransform = scaleTransform;
                

                sourceImage = imgPhoto;

                MemoryStream memStream = new MemoryStream();
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bit));
                encoder.Save(memStream);
                byte[] input = memStream.ToArray();
                float[] output = new float[input.Length / 4];

                //var newInput = new float[input.Length / 4];

                float[] newInput = new float[input.Length / 4];
                bit.CopyPixels(newInput, 7680, 0);
                Buffer.BlockCopy(input, 0, newInput, 0, newInput.Length);

                //test.ProcessBlock(ref newInput, ref output, newInput.Length);

                var final = new byte[output.Length * 4];
                Buffer.BlockCopy(output, 0, final, 0, final.Length);

                MemoryStream stream = new MemoryStream(final);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();

                imgPhoto.Source = image;


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

            resetQueue();
        }

        private void resetQueue()
        {
            int filterCount = numFilters + 1;
            if (filterCount > 1)
            {
                for (int i = 1; i < filterCount; i++)
                {
                    Button filter = (Button)this.FindName("Filter" + i);
                    int nextPlacement = i + 1;

                    if (filter.Visibility == Visibility.Hidden && i != filterCount)
                    {
                        Button nextFilter = (Button)this.FindName("Filter" + nextPlacement);
                        filter.Content = nextFilter.Content;

                        filter.Visibility = Visibility.Visible;
                        nextFilter.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
    }
}