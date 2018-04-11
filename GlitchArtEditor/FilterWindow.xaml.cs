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
    /// Interaction logic for FilterWindow.xaml
    /// GUI for setting filters for an image.
    /// </summary>
    public partial class FilterWindow : Window
    {
        String elementType;

        /// <summary>
        /// Initializes filter window. If call comes from
        /// the filter menu, the remove button is unenabled.
        /// </summary>
        public FilterWindow(String type)
        {
            InitializeComponent();
            elementType = type;

            if (elementType == "MenuItem")
            {
                RemoveButton.IsEnabled = false;
            }
            else
            {
                RemoveButton.IsEnabled = true;
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
            if (elementType == "MenuItem")
            {
                string filterName = FilterTitle.Text;
                ((MainWindow)this.Owner).AddFilter(filterName, (double) value1.Value, (float)value2.Value, (int)value3.Value);
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
            // Use filter name or something else to denote the selected filter
            ((MainWindow)this.Owner).RemoveFilter(elementType);
            this.Close();
        }

    }
}