using System.Windows;
using System.Windows.Media;

namespace SilverlightColorPicker
{
    public class ColorChangedEventArgs : RoutedEventArgs
    {
        internal ColorChangedEventArgs(Color selectedColor)
	    {
            this.SelectedColor = selectedColor;
	    }

        public Color SelectedColor
        {
            get;
            private set;
        }
    }
}
