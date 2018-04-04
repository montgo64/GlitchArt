using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Effects;
using EchoEffect;
using System.IO;

using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;

namespace GlitchArtEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MAX_FILTERS = 5;

        public System.Windows.Controls.Image sourceImage;
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


                Bitmap bm = new Bitmap(op.FileName);

                FloatToInt[] bmvals = new FloatToInt[bm.Width * bm.Height];
                int index = 0;


                    for (int h = 0; h < bm.Height; h++)
                    {
                    for (int w = 0; w < bm.Width; w++)
                    {
                        bmvals[index].IntVal = bm.GetPixel(w, h).ToArgb();
                        index++;
                    }
                }

                FloatToInt[] output = new FloatToInt[bmvals.Length];

                test.ProcessBlock(ref bmvals, ref output, bmvals.Length);
                index = 0;

                for (int h = 0; h < bm.Height; h++)
                {
                    for (int w = 0; w < bm.Width; w++)
                {

                        bm.SetPixel(w, h, System.Drawing.Color.FromArgb(output[index].IntVal));
                        index++;
                    }
                }

                var stream = new MemoryStream();
                bm.Save(stream, ImageFormat.Png);
                var image = new BitmapImage();
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