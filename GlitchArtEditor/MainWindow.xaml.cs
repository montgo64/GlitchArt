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

        /// <summary>
        /// Method to open file from user's computer. Must be JPEG file.
        /// Converts the JPEG to Bitmap Image
        /// </summary>
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a File";
            op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            if (op.ShowDialog() == true)
            {
                //Gets bitmap image from user
                BitmapImage bit = new BitmapImage(new Uri(op.FileName));

                //Converts Bitmap Image to byte array and then to wav file. Saves file as "test.wav"
                convertBMPtoWav(bit);


                //Following code is to test that conversions work.
                //Converts the wav file back to a byte array back to bitmap
                //Sets it to sourceImage so you can see the image when opened
                //BitmapImage bitt = convertWavtoBMP();
                //imgPhoto.Source = bitt;
                //imgPhoto.Width = bitt.Width;
                //imgPhoto.Height = bitt.Height;
                //imgPhoto.LayoutTransform = scaleTransform;
                //sourceImage = imgPhoto;

                //Sets user's image as sourceImage
                imgPhoto.Source = bit;
                imgPhoto.Width = bit.Width;
                imgPhoto.Height = bit.Height;
                imgPhoto.LayoutTransform = scaleTransform;
                sourceImage = imgPhoto;
            }
        }

        /// <summary>
        /// Method to convert Bitmap image to a wav file. This method is
        /// called when a filter needs to be added to the picture.
        /// </summary>
        private void convertBMPtoWav(BitmapImage bit)
        {
            //convert bmp to byte array
            byte[] byteArr = convertBMPtoByteArray(bit);

            //write bytes to wav file
            using (FileStream bytetoimage = File.Create("test.wav"))
            {
                bytetoimage.Write(byteArr, 0 , byteArr.Length);
            }
            
        }

        /// <summary>
        /// Method to convert wav file to Bitmap image. This method is called
        /// after a filter is applied to the audio file.
        /// </summary>
        private BitmapImage convertWavtoBMP() 
        {
            //convert wav to byte array
            byte[] audiobyte = File.ReadAllBytes("test.wav");
            
            //convert byte array to bitmap
            BitmapImage img = convertByteArraytoBMP(audiobyte);
      
            return img;
        }

        /// <summary>
        /// Method to convert Bitmap image to a byte array
        /// </summary>
        private byte[] convertBMPtoByteArray(BitmapImage bit)
        {
            byte[] byteArray;
            
            using(MemoryStream ms = new MemoryStream())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bit));
                encoder.Save(ms);
                byteArray = ms.ToArray();
                return byteArray;
            }

        }

        /// <summary>
        /// Method to convert byte array to Bitmap image
        /// </summary>
        private BitmapImage convertByteArraytoBMP(byte[] byteArray) 
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = new MemoryStream(byteArray);
            image.EndInit();
            return image;
        }

        /// <summary>
        /// Method to save the file
        /// </summary>
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

        /// <summary>
        /// Method to close application
        /// </summary>
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