using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace MashupDesignTool
{
    public class Utility
    {
        private static Type[] frameworkTypes = { typeof(Border), typeof(Button), typeof(CheckBox),typeof(ComboBox),
                                        typeof(DataGrid), typeof(Image), typeof(Label), typeof(ListBox),
                                        typeof(RadioButton), typeof(Rectangle), typeof(TabControl), 
                                        typeof(TextBlock), typeof(TextBox) };

        public static bool IsFrameworkControl(FrameworkElement control)
        {
            Type type = control.GetType();
            return frameworkTypes.Contains(type);
        }

        public static bool IsFrameworkControl(Type type)
        {
            return frameworkTypes.Contains(type);
        }
    }
}
