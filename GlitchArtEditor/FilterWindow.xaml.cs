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
    public partial class FilterWindow : Window
    {
        String elementType;
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

        private void ApplyFilter(object sender, RoutedEventArgs e)
        {
            if (elementType == "MenuItem")
            {
                string filterName = FilterTitle.Text;
                ((MainWindow)this.Owner).AddFilter(filterName);
            }
            this.Close();
        }

        private void CancelFilter(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RemoveFilter(object sender, RoutedEventArgs e)
        {
            // Use filter name or something else to denote the selected filter
            ((MainWindow)this.Owner).RemoveFilter(elementType);
            this.Close();
        }

    }
}