using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Effects;
using EchoEffect;
using AmplifyEffect;
using BassBoostEffect;
using PhaserEffect;
using FadeEffect;
using System.IO;

using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Input;

namespace GlitchArtEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Main GUI
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MAX_FILTERS = 5;

        public System.Windows.Controls.Image sourceImage;
        private BitmapImage originalImage;
        private ScaleTransform scaleTransform;
        private int numFilters;
        private string filename;
        public Dictionary<String, FilterInfo> filtersList = new Dictionary<String, FilterInfo>();

        /// <summary>
        /// Struct for information of the filter.
        /// Contains filter name and parameters.
        /// </summary>
        public struct FilterInfo
        {
            public string effect;
            public EffectParameters param;

            public FilterInfo(string eff, EffectParameters par)
            {
                effect = eff;
                param = par;
            }
        }

        /// <summary>
        /// Initializes main GUI window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            scaleTransform = new ScaleTransform();
            numFilters = 0;
        }

        /// <summary>
        /// This method opens file for user to select a jpeg image.
        /// The image and filename are stored for filter use. This
        /// method is called when user selects File-Open.
        /// </summary>
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a File";
            op.Filter = "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg";
            if (op.ShowDialog() == true)
            {
                long length = new System.IO.FileInfo(op.FileName).Length;
                if (length > 500000)
                {
                    MessageBoxResult result = MessageBox.Show("Image exceeds 500KB and application may run slow. Continue?",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                int oldFilterNum = numFilters;
                for (int i = 1; i <= oldFilterNum; i++)
                {
                    String currentFilter = "Filter1";
                    RemoveFilter(currentFilter);
                }

                numFilters = 0;

                BitmapImage bit = new BitmapImage(new Uri(op.FileName));

                imgPhoto.Source = bit;
                imgPhoto.Width = bit.Width;
                imgPhoto.Height = bit.Height;
                imgPhoto.LayoutTransform = scaleTransform;

                sourceImage = imgPhoto;
                originalImage = bit;
                filename = op.FileName;

                StatusText.Content = "Select a filter to apply to image. ";
                SetZoom();
            }
        }

        /// <summary>
        /// This method saves the image as a jpeg to the user's selected path.
        /// This method is called when user selects File-Save.
        /// </summary>
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (sourceImage != null)
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
        }

        /// <summary>
        /// This method closes the GUI. This method is called when user
        /// selected File-Exit.
        /// </summary>
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This method sets the default zoom amount for the newly opened
        /// image and links it to the zoom functionality.
        /// </summary>
        private void SetZoom()
        {
            if (sourceImage != null)
            {
                // Link image to zoom slider
                Binding bindX = new Binding();
                bindX.Source = ZoomSlider;
                bindX.Path = new PropertyPath("Value");
                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, bindX);

                Binding bindY = new Binding();
                bindY.Source = ZoomSlider;
                bindY.Path = new PropertyPath("Value");
                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, bindY);

                // Set default zoom to proper value
                double wRatio = Math.Round(ImageScroll.ActualWidth / imgPhoto.Width, 2) - 0.03;
                double hRatio = Math.Round(ImageScroll.ActualHeight / imgPhoto.Height, 2) - 0.03;

                if (wRatio < 1 || hRatio < 1)
                {
                    ZoomSlider.Value = wRatio < hRatio ? wRatio : hRatio;
                    ZoomTxt.Text = ZoomSlider.Value * 100 + "%";
                }
            }
        }

        /// <summary>
        /// This method changes the zoom of the image in the GUI.
        /// Zoom can be changed between 10% to 200% using the Slider
        /// or TextBox at the bottom of the GUI.
        /// </summary>
        private void ZoomChanged(object sender, RoutedEventArgs e)
        {
            if (ZoomSlider != null && ZoomTxt != null)
            {
                Control control = (Control)sender;

                if (control.Name.Equals("ZoomSlider"))
                {
                    ZoomTxt.Text = Math.Round(ZoomSlider.Value * 100, 0) + "%";
                }
                else if (control.Name.Equals("ZoomTxt"))
                {
                    String zoomAmount = ZoomTxt.Text.ToString().Replace("%", "");
                    ZoomSlider.Value = Convert.ToDouble(zoomAmount) / 100;
                }
            }
        }

        /// <summary>
        /// This method is triggered when the ENTER button is pressed
        /// while using the zoom TextBox. It fires the ZoomChanged
        /// method and removes focus from the Control.
        /// </summary>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && ((Control)sender).Name.Equals("ZoomTxt"))
            {
                ZoomChanged(sender, e);
                Keyboard.ClearFocus();
            }
        }

        /// <summary>
        /// This method is called when the user selects any filter in
        /// the Filters menu. It checks if the maximum number of filters
        /// has already been applied and sends error message to screen.
        /// If under the max, it stores which filter has been selected
        /// and makes a call to open that filter's parameter window.
        /// </summary>
        private void FilterSelect(object sender, RoutedEventArgs e)
        {
            if (sourceImage != null)
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

                    filterType = ((MenuItem)sender).Header.ToString();
                    elementType = "MenuItem";
                }
                else if (sender.GetType() == typeof(Button))
                {
                    filterType = ((Button)sender).Content.ToString();
                    elementType = ((Button)sender).Name.ToString();
                }

                OpenFilterWindow(filterType, elementType);
            }
            else
            {
                MessageBox.Show("Must have an image opened.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// This method opens a selected filter's window where the user
        /// can set the parameters for the filter. Once the parameters 
        /// are set, the user can then apply, cancel, or remove the filter.
        /// This method is called when a user selects a new filter under
        /// the filters menu or when a user selects an already applied filter
        /// in the filter queue.
        /// </summary>
        private void OpenFilterWindow(String filterType, String inputElement)
        {
            EffectParameters parameters;

            if(inputElement.Equals("MenuItem"))
            {
                switch (filterType)
                {
                    case "Echo":
                        parameters = new EchoParameters();
                        break;
                    case "Amplify":
                        parameters = new AmplifyParameters();
                        break;
                    case "Bass Boost":
                        parameters = new BassBoostParameters();
                        break;
                    case "Phaser":
                        parameters = new PhaserParameters();
                        break;
                    case "Fade":
                        parameters = new FadeParameters();
                        break;
                    default:
                        StatusText.Content = filterType + " is an invalid filter. ";
                        return;
                }
            }
            else
            {
                parameters = filtersList[inputElement].param;
            }

            FilterWindow winFilter = new FilterWindow(filterType, inputElement, parameters);
            winFilter.Owner = this;
            winFilter.Show();
        }

        /// <summary>
        /// This method adds the selected filter to the image. This
        /// method is called once the user has set the parameters in
        /// the filter's window and hit Apply. This method takes the
        /// parameters and makes the call to apply the filter to the
        /// image.
        /// </summary>
        public void AddFilter(String filterType, EffectParameters parameters)
        {
            numFilters++;

            Button filter = (Button)this.FindName("Filter" + numFilters);
            filter.Content = filterType;
            filter.Visibility = Visibility.Visible;

            FilterInfo info = new FilterInfo(filterType, parameters);
            filtersList.Add("Filter" + numFilters, info);

            ApplyFilters(filterType);
        }

        /// <summary>
        /// This method updates the selected filter. This
        /// method is called once the user has hit the filter
        /// button in the filter queue on the MainWindow.
        /// </summary>
        public void UpdateFilter(String filterType, String inputElement, EffectParameters parameters)
        {
            FilterInfo info = new FilterInfo(filterType, parameters);
            filtersList[inputElement] = info;

            ApplyFilters(filterType);
        }

        /// <summary>
        /// This method removes the selected filter from the image.
        /// This method can only be used when a filter has already
        /// been applied and is called when the user hits the Remove
        /// button in the filter's window.
        /// </summary>
        public void RemoveFilter(String filterName)
        {
            Button filter = (Button)this.FindName(filterName);
            filter.Content = "";
            filter.Visibility = Visibility.Hidden;

            numFilters--;

            //removes filter from the list of filters
            filtersList.Remove(filterName);

            //sets image back to the original image
            //to reapply remaining filters
            imgPhoto.Source = originalImage;

            //resets filter queue and applies filters
            ResetQueue();
        }

        /// <summary>
        /// This method resets the filter queue when a filter has
        /// been removed. It shifts the remaining filters up the
        /// queue. 
        /// </summary>
        private void ResetQueue()
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

                        //Reapplies filter to image and stores filter info in new place number
                        FilterInfo info = new FilterInfo(filtersList["Filter" + nextPlacement].effect, filtersList["Filter" + nextPlacement].param);
                        filtersList.Add("Filter" + i, info);
                        filtersList.Remove("Filter" + nextPlacement);

                        filter.Visibility = Visibility.Visible;
                        nextFilter.Visibility = Visibility.Hidden;
                    }
                }

                ApplyFilters("");
            }

            StatusText.Content = "Removed the filter. ";
        }

        /// <summary>
        /// Applies the filters to an image in the order the list.
        /// </summary>
        private void ApplyFilters(String filterType)
        {
            // Create temporary filename and current image to its original
            String temp = filename + "_temp";
            imgPhoto.Source = originalImage;

            int filterCount = numFilters + 1;
            for (int i = 1; i < filterCount; i++)
            {
                // Save temporary file that function can keep referring to
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgPhoto.Source));
                FileStream stream = new FileStream(temp, FileMode.Create);
                encoder.Save(stream);
                stream.Close();

                //Converts image to bitmap
                Bitmap bitmap = new Bitmap(temp);
                FloatToInt[] bmvals = ConvertImagetoArray(bitmap);

                //Creates floattoint array for filter output
                FloatToInt[] output = new FloatToInt[bmvals.Length];

                RunFilter(filtersList["Filter" + i].effect, filtersList["Filter" + i].param, ref bmvals, ref output, bmvals.Length);
                bmvals = output;

                BitmapImage image = ConvertArraytoImage(bmvals, bitmap);
                bitmap.Dispose();

                //Sets filtered image to source image
                imgPhoto.Source = image;

                if (!filterType.Equals(""))
                {
                    StatusText.Content = "Applied the " + filterType + " filter. ";
                }
            }

            File.Delete(temp);
        }

        /// <summary>
        /// Runs the filter on the image using the specific filter 
        /// type's ProcessBlock method.
        /// </summary>
        private void RunFilter(String filter, EffectParameters param, ref FloatToInt[] input, ref FloatToInt[] output, int length)
        {
            switch (filter)
            {
                case "Echo":
                    Echo echo = new Echo((EchoParameters)param);
                    echo.ProcessBlock(ref input, ref output, input.Length);
                    break;
                case "Amplify":
                    Amplify amplify = new Amplify((AmplifyParameters)param);
                    amplify.ProcessBlock(ref input, ref output, input.Length);
                    break;
                case "Bass Boost":
                    BassBoost BassBoost = new BassBoost((BassBoostParameters)param);
                    BassBoost.ProcessBlock(ref input, ref output, input.Length);
                    break;
                case "Phaser":
                    Phaser Phaser = new Phaser((PhaserParameters)param);
                    Phaser.ProcessBlock(ref input, ref output, input.Length);
                    break;
                case "Fade":
                    Fade Fade = new Fade((FadeParameters)param);
                    Fade.ProcessBlock(ref input, ref output, input.Length);
                    break;
                default:
                    StatusText.Content = "Invalid filter";
                    break;
            }
        }

        /// <summary>
        /// This method converts the image to an array.
        /// </summary>
        private FloatToInt[] ConvertImagetoArray(Bitmap bitmap)
        {
            //Creates floattoint array with image size
            FloatToInt[] bmvals = new FloatToInt[bitmap.Width * bitmap.Height];

            //Converts bitmap image to array
            int index = 0;

            for (int h = 0; h < bitmap.Height; h++)
            {
                for (int w = 0; w < bitmap.Width; w++)
                {
                    bmvals[index].IntVal = bitmap.GetPixel(w, h).ToArgb();
                    index++;
                }
            }

            return bmvals;
        }

        /// <summary>
        /// This method converts the array to an image.
        /// </summary>
        private BitmapImage ConvertArraytoImage(FloatToInt[] array, Bitmap bitmap)
        {
            int index = 0;

            //Converts array with filter back to bitmap
            for (int h = 0; h < bitmap.Height; h++)
            {
                for (int w = 0; w < bitmap.Width; w++)
                {
                    bitmap.SetPixel(w, h, System.Drawing.Color.FromArgb(array[index].IntVal));
                    index++;
                }
            }

            //Converts bitmap back to image
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }
    }
}