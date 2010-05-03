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
    public class ThicknessConverter:TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            Thickness tn = new Thickness();
            string[] s = value.ToString().Split(',');
            bool success = false;

            if (s.Length == 1)
            {
                double val;
                success = double.TryParse(s[0], out val);
                tn.Bottom = tn.Top = tn.Left = tn.Right = val;
                if (!success)
                    return value;
                return tn;
            }
            if (s.Length == 2)
            {
                double val1, val2;
                success = double.TryParse(s[0], out val1);
                success = double.TryParse(s[1], out val2);
                tn.Left = tn.Right = val1;
                tn.Bottom = tn.Top = val2;
                if (!success)
                    return value;
                return tn;
            }
            if (s.Length == 4)
            {
                double val1, val2, val3, val4;
                success = double.TryParse(s[0], out val1);
                success = double.TryParse(s[1], out val2);
                success = double.TryParse(s[2], out val3);
                success = double.TryParse(s[3], out val4);
                tn.Left = val1;
                tn.Top = val2;
                tn.Right = val3;
                tn.Bottom = val4;                
                if (!success)
                    return value;
                return tn;
            }
            return value;
        }
    }
}
