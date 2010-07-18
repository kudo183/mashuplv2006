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
using System.ComponentModel;
using System.Globalization;

namespace SL30PropertyGrid.Converters
{
    public class CornerRadiusConverter:TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            CornerRadius cr = new CornerRadius();
            string[] s = value.ToString().Split(',');
            bool success = false;

            if (s.Length == 1)
            {
                double val;
                success = double.TryParse(s[0], out val);
                cr.TopLeft = cr.TopRight = cr.BottomLeft = cr.BottomRight = val;
                if (!success)
                    return value;
                return cr;
            }
            if (s.Length == 4)
            {
                double val1, val2, val3, val4;
                success = double.TryParse(s[0], out val1);
                success = double.TryParse(s[1], out val2);
                success = double.TryParse(s[2], out val3);
                success = double.TryParse(s[3], out val4);
                cr.TopLeft = val1;
                cr.TopRight = val2;
                cr.BottomRight = val3;
                cr.BottomLeft = val4;                
                if (!success)
                    return value;
                return cr;
            }
            return value;
        }
    }
}
