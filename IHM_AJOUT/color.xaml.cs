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
using System.Windows.Shapes;

namespace IHM_BASE
{
    /// <summary>
    /// Logique d'interaction pour color.xaml
    /// </summary>
    public partial class color : Window
    {
        public color()
        {
            InitializeComponent();
        }

        private void ColorPicker_SelectedColorChanged(object sender,RoutedPropertyChangedEventArgs<Color?> e) {
            // Récupérer la couleur sélectionnée et la convertir en hexadécimal
            Color selectedColor = colorPicker.SelectedColor.GetValueOrDefault();
            hexTextBox.Text = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
        }
    }
}
