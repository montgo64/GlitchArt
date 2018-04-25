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
using Effects;
using EchoEffect;
using AmplifyEffect;
using BassBoostEffect;
using PhaserEffect;
using FadeEffect;
using DistortionEffect;
using System.Threading;

namespace GlitchArtEditor
{
    /// <summary>
    /// Interaction logic for FilterWindow.xaml
    /// GUI for setting filters for an image.
    /// </summary>
    public partial class FilterWindow : Window
    {
        private String filterType;
        private String inputElement;
        private EffectParameters parameters;

        /// <summary>
        /// Initializes filter window. If call comes from
        /// the filter menu, the remove button is unenabled.
        /// </summary>
        public FilterWindow(String filterType, String inputElement, EffectParameters parameters)
        {
            InitializeComponent();
            this.filterType = filterType;
            this.inputElement = inputElement;
            this.parameters = parameters;

            FilterTitle.Text = filterType;

            if (inputElement == "MenuItem")
            {
                RemoveButton.IsEnabled = false;
            }
            else
            {
                RemoveButton.IsEnabled = true;
            }

            Parameter1.Visibility = Visibility.Hidden;
            value1.Visibility = Visibility.Hidden;
            Parameter2.Visibility = Visibility.Hidden;
            value2.Visibility = Visibility.Hidden;
            Parameter3.Visibility = Visibility.Hidden;
            value3.Visibility = Visibility.Hidden;
            Parameter4.Visibility = Visibility.Hidden;
            value4.Visibility = Visibility.Hidden;
            Parameter5.Visibility = Visibility.Hidden;
            value5.Visibility = Visibility.Hidden;
            Parameter6.Visibility = Visibility.Hidden;
            value6.Visibility = Visibility.Hidden;
            Parameter7.Visibility = Visibility.Hidden;
            value7.Visibility = Visibility.Hidden;
            Parameter8.Visibility = Visibility.Hidden;
            value8.Visibility = Visibility.Hidden;

            int count = 1;
            TextBlock text;
            Slider slide;

            foreach (Parameter parameter in parameters.GetParams().Values)
            {
                text = (TextBlock)this.FindName("Parameter" + count);
                text.Text = parameter.name;
                text.Visibility = Visibility.Visible;

                slide = (Slider)this.FindName("value" + count);
                slide.Value = parameter.value;
                slide.Minimum = parameter.minValue;
                slide.Maximum = parameter.maxValue;
                slide.TickFrequency = parameter.frequency;
                slide.Visibility = Visibility.Visible;

                count++;
            }
        }

        /// <summary>
        /// This method adds the selected filter to the image. This
        /// method is called when the user hits the Apply button. 
        /// This method takes the parameters and makes the call to
        /// apply the filter to the image.
        /// </summary>
        private void ApplyFilter(object sender, RoutedEventArgs e)
        {
            switch (filterType)
            {
                case "Echo":
                    parameters = new EchoParameters(value1.Value, (float)value2.Value, Convert.ToInt32(value3.Value));
                    break;
                case "Amplify":
                    parameters = new AmplifyParameters((float)value1.Value);
                    break;
                case "Bass Boost":
                    parameters = new BassBoostParameters(value1.Value);
                    break;
                case "Phaser":
                    parameters = new PhaserParameters((int)value1.Value, (int)value2.Value, value3.Value, value4.Value, (int)value5.Value, (int)value6.Value, value7.Value);
                    break;
                case "Fade":
                    parameters = new FadeParameters((int)value1.Value, (int)value2.Value, (int)value3.Value);
                    break;
                case "Distortion":
                    parameters = new DistortionParameters((float)value1.Value, (int)value2.Value, value3.Value, value4.Value, value5.Value, (float)value6.Value, (float)value7.Value, (int)value8.Value);
                    break;
                default:
                    // Not a valid Filter
                    break;
            }

            MainWindow window = (MainWindow)this.Owner;

            if (inputElement == "MenuItem")
            {
                Dispatcher.BeginInvoke( new ThreadStart(() => window.AddFilter(filterType, parameters)));
            }
            else
            {
                Dispatcher.BeginInvoke(new ThreadStart(() => window.UpdateFilter(filterType, inputElement, parameters)));
            }

            this.Close();
        }

        /// <summary>
        /// This method closes out of the filter window. This
        /// method is called when the user hits the Cancel button.
        /// </summary>
        private void CancelFilter(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This method removes the selected filter from the image.
        /// This method is called when the user hits the Remove
        /// button in the filter's window. It is unenabled when a
        /// new filter is being added.
        /// </summary>
        private void RemoveFilter(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(() => ((MainWindow)this.Owner).RemoveFilter(inputElement)));

            this.Close();
        }
    }
}