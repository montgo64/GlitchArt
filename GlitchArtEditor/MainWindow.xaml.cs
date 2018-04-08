﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Effects;
using EchoEffect;
using AmplifyEffect;
using System.IO;

using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;

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
        private Bitmap bitmap;
        private ScaleTransform scaleTransform;
        private int numFilters;
        private string filename;
       
        /// <summary>
        /// Struct for information of the filter.
        /// Contains filter name and parameters.
        /// </summary>
        public struct filterInfo
        {
            public string effect;
            public EffectParameters param;

            public filterInfo(string eff, EffectParameters par)
            {   
                effect = eff;
                param = par;
            }
        }
        public Dictionary<String, filterInfo> filtersList = new Dictionary<String, filterInfo>();

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
                BitmapImage bit = new BitmapImage(new Uri(op.FileName));

                imgPhoto.Source = bit;
                imgPhoto.Width = bit.Width;
                imgPhoto.Height = bit.Height;
                imgPhoto.LayoutTransform = scaleTransform;

                sourceImage = imgPhoto;
                originalImage = bit;
                filename = op.FileName;
                bitmap = new Bitmap(filename);
            }
        }

        /// <summary>
        /// This method saves the image as a jpeg to the user's selected path.
        /// This method is called when user selects File-Save.
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
        /// This method closes the GUI. This method is called when user
        /// selected File-Exit.
        /// </summary>
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This method changes the zoom of the image in the GUI.
        /// Zoom can be changed between 10% to 200% using slider
        /// at the bottom of the GUI.
        /// </summary>
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

        /// <summary>
        /// This method is called when the user selects any filter in
        /// the Filters menu. It checks if the maximum number of filters
        /// has already been applied and sends error message to screen.
        /// If under the max, it stores which filter has been selected
        /// and makes a call to open that filter's parameter window.
        /// </summary>
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

        /// <summary>
        /// This method opens a selected filter's window where the user
        /// can set the parameters for the filter. Once the parameters 
        /// are set, the user can then apply, cancel, or remove the filter.
        /// This method is called when a user selects a new filter under
        /// the filters menu or when a user selects an already applied filter
        /// in the filter queue.
        /// </summary>
        private void OpenFilterWindow(String type, String elementType)
        {
            FilterWindow winFilter = new FilterWindow(elementType);
            winFilter.Owner = this;

            // Will need to expand this to a new function, for each filter
            // Currently only applies to Echo filter
            winFilter.FilterTitle.Text = type;
            if (type.Equals("Echo"))
            {
                winFilter.Parameter1.Text = "Delay";
                winFilter.value1.Value = 1;

                winFilter.Parameter2.Text = "Decay";
                winFilter.value2.Value = 0.5;

                winFilter.Parameter3.Text = "History Length";
                winFilter.value3.Value = 10;
            }

            if (type.Equals("Amplify"))
            {
                winFilter.Parameter1.Text = "Amplification (dB)";
                winFilter.value1.Value = 3.0;

                winFilter.Parameter2.Visibility = Visibility.Hidden;
                winFilter.Parameter3.Visibility = Visibility.Hidden;
            }
            winFilter.Show();
        }

        /// <summary>
        /// This method adds the selected filter to the image. This
        /// method is called once the user has set the parameters in
        /// the filter's window and hit Apply. This method takes the
        /// parameters and makes the call to apply the filter to the
        /// image.
        /// </summary>
        public void AddFilter(String filterType, double param1, float param2, int param3)
        {
            numFilters++;

            Button filter = (Button)this.FindName("Filter" + numFilters);
            filter.Content = filterType;
            filter.Visibility = Visibility.Visible;

            //Currently only applies to Echo filter
            if (filterType.Equals("Echo"))
            {
                EffectParameters parameters = new EchoParameters(param1, param2, param3*1000);
                applyEcho("Filter" + numFilters, parameters);
            }

            if (filterType.Equals("Amplify"))
            {
                EffectParameters parameters = new AmplifyParameters(param1, 0, 0);
                applyAmplify("Filter" + numFilters, parameters);
            }
        }

        /// <summary>
        /// This method adds the filters to the image. This method is
        /// called when the filter queue is reset by a filter being
        /// removed. This applies the other filters back to the image.
        /// </summary>
        public void AddFilter(String filterType, EffectParameters param)
        {
            if (filterType.Equals("Echo"))
            {
                applyEcho("Filter" + numFilters, param);
            }
            if (filterType.Equals("Amplify"))
            {
                applyAmplify("Filter" + numFilters, param);
            }
            if (filterType.Equals("Amplify"))
            {

            }
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
            resetQueue();
        }

        /// <summary>
        /// This method resets the filter queue when a filter has
        /// been removed. It shifts the remaining filters up the
        /// queue. 
        /// </summary>
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

                        //Reapplies filter to image and stores filter info in new place number
                        filterInfo info = new filterInfo(filtersList["Filter" + nextPlacement].effect, filtersList["Filter" + nextPlacement].param);
                        AddFilter(info.effect, info.param);
                        filtersList.Remove("Filter" + nextPlacement);

                        filter.Visibility = Visibility.Visible;
                        nextFilter.Visibility = Visibility.Hidden;
                    }

                    else
                    {
                        AddFilter(filtersList["Filter" + i].effect, filtersList["Filter" + i].param);
                    }
                }
            }
        }

        /// <summary>
        /// This method converts the image to an array.
        /// </summary>
        private FloatToInt[] convertImagetoArray()
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
        private BitmapImage convertArraytoImage(FloatToInt[] array)
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

        /// <summary>
        /// This method applies the echo filter to the image.
        /// It gets the parameters from the echo filter window
        /// set by the user. This method is called once a user
        /// hits apply in the filter window. When this method
        /// is complete, the output image will now have the echo
        /// effect applied to it.
        /// </summary>
        private void applyEcho(string filterKey, EffectParameters parameters)
        {
            Echo echo = new Echo();

            //Adds filter and corresponding parameters to the filter list
            if (!filtersList.ContainsKey(filterKey)) 
            {
                filterInfo info = new filterInfo("Echo", parameters);
                filtersList.Add(filterKey, info);
            }

            echo.SetParameters(ref parameters);

            FloatToInt[] bmvals = convertImagetoArray();

            //Creates floattoint array for filter output
            FloatToInt[] output = new FloatToInt[bmvals.Length];

            //Calls to apply echo filter
            echo.ProcessBlock(ref bmvals, ref output, bmvals.Length);

            var image = convertArraytoImage(output);

            //Sets filtered image to source image
            imgPhoto.Source = image;
        }

        private void applyAmplify(string filterKey, EffectParameters parameters)
        {
            Amplify amplify = new Amplify();

            //Adds filter and corresponding parameters to the filter list
            if (!filtersList.ContainsKey(filterKey)) 
            {
                filterInfo info = new filterInfo("Amplify", parameters);
                filtersList.Add(filterKey, info);
            }

            amplify.SetParameters(ref parameters);

            FloatToInt[] bmvals = convertImagetoArray();

            //Creates floattoint array for filter output
            FloatToInt[] output = new FloatToInt[bmvals.Length];

            //Calls to apply echo filter
            amplify.ProcessBlock(ref bmvals, ref output, bmvals.Length);

            var image = convertArraytoImage(output);

            //Sets filtered image to source image
            imgPhoto.Source = image;
        }
    }
}