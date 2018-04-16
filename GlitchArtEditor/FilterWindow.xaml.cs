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

            switch (filterType)
            {
                case "Echo":
                    Parameter1.Text = "Delay";
                    value1.Value = ((EchoParameters)parameters).delay;
                    Parameter2.Text = "Decay";
                    value2.Value = ((EchoParameters)parameters).decay;
                    Parameter3.Text = "History Length";
                    value3.Value = ((EchoParameters)parameters).histLen;
                    break;
                case "Amplify":
                    Parameter1.Text = "Amplification (dB)";
                    value1.Value = ((AmplifyParameters)parameters).mRatio;
                    Parameter2.Visibility = Visibility.Hidden;
                    value2.Visibility = Visibility.Hidden;
                    Parameter3.Visibility = Visibility.Hidden;
                    value3.Visibility = Visibility.Hidden;
                    break;
                case "Bass Boost":
                    Parameter1.Text = "Bass (dB)";
                    value1.Value = ((BassBoostParameters)parameters).bass;
                    Parameter2.Visibility = Visibility.Hidden;
                    value2.Visibility = Visibility.Hidden;
                    Parameter3.Visibility = Visibility.Hidden;
                    value3.Visibility = Visibility.Hidden;
                    break;
                default:
                    // Not a valid Filter
                    Parameter1.Visibility = Visibility.Hidden;
                    value1.Visibility = Visibility.Hidden;
                    Parameter2.Visibility = Visibility.Hidden;
                    value2.Visibility = Visibility.Hidden;
                    Parameter3.Visibility = Visibility.Hidden;
                    value3.Visibility = Visibility.Hidden;
                    break;
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
                    ((EchoParameters)parameters).delay = value1.Value;
                    ((EchoParameters)parameters).decay = (float)value2.Value;
                    ((EchoParameters)parameters).histLen = Convert.ToInt32(value3.Value);
                    break;
                case "Amplify":
                    ((AmplifyParameters)parameters).mRatio = (float)value1.Value;
                    break;
                case "Bass Boost":
                    ((BassBoostParameters)parameters).bass = value1.Value;
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