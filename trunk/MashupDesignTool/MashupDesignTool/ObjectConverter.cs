using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

namespace luanvan
{
    public class ObjectConverter
    {
        public static string ToXaml(object obj)
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings xwSet = new XmlWriterSettings();
            xwSet.OmitXmlDeclaration = true;
            XmlWriter xw = XmlWriter.Create(sb, xwSet);

            ToXaml(obj, xw);

            xw.Flush();
            xw.Close();

            string xaml = sb.ToString();

            Type t = obj.GetType();
            xaml = xaml.Substring(t.Name.Length + 1);
            xaml = xaml.Replace(t.Name, "me:" + t.Name);
            xaml = buildXamlPrefix(t.Name, t.Namespace, t.Assembly.FullName.Split(',')[0]) + " xmlns=\"http://schemas.microsoft.com/client/2007\"" + xaml;

            xaml = xaml.Replace("<ItemCollection>", "");
            xaml = xaml.Replace("</ItemCollection>", "");
            return xaml;
        }

        private static void ToXaml(object obj, XmlWriter xw)
        {
            Type t = obj.GetType();

            xw.WriteStartElement(t.Name);
            object value = new object();

            List<string> lstName = new List<string>();
            List<object> lstValue = new List<object>();

            #region object is an indexer type
            int count = -1;
            PropertyInfo p = t.GetProperty("Item");
            if (p != null)
            {
                if (t.GetProperty("Count") != null &&
                    t.GetProperty("Count").PropertyType == typeof(System.Int32))
                {
                    count = (int)t.GetProperty("Count").GetValue(obj, null);
                }
            }
            if (count > 0)
            {
                object[] index = new object[1];
                for (int i = 0; i < count; i++)
                {
                    index[0] = i;
                    value = p.GetValue(obj, index);
                    if (ObjectConverter.WriteAttribute(p.Name, value, xw) == false)
                    {
                        if (IsSkipProperty(p.Name) == false)
                        {
                            lstName.Add("");
                            lstValue.Add(value);
                        }
                    }
                }
            }
            #endregion

            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (pi.CanWrite == true && pi.GetSetMethod() != null)
                {
                    if (pi.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    value = pi.GetValue(obj, null);

                    if (ObjectConverter.WriteAttribute(pi.Name, value, xw) == false)
                    {
                        if (IsSkipProperty(pi.Name) == false)
                        {
                            lstName.Add(t.Name + "." + pi.Name);
                            lstValue.Add(value);
                        }
                    }
                }
                else
                {
                    value = pi.GetValue(obj, null);
                    if (value != null && value.GetType().GetProperty("Item") != null)
                    {
                        if (IsSkipProperty(pi.Name) == false)
                        {
                            lstName.Add(t.Name + "." + pi.Name);
                            lstValue.Add(value);
                        }
                    }
                }
            }

            for (int i = 0; i < lstName.Count; i++)
                ObjectConverter.WriteChildElement(lstValue[i], lstName[i], xw);

            xw.WriteEndElement();
        }

        private static bool IsSkipProperty(string propertyName)
        {
            string[] skipProperty = new string[] { "Resources", "Language", "Template", "Triggers", "SyncRoot" };
            foreach (string str in skipProperty)
                if (str == propertyName)
                    return true;

            return false;
        }

        private static string buildXamlPrefix(string className, string controlNamespace, string assemblyName)
        {
            return "<me:" + className + " xmlns:me=\"clr-namespace:" + controlNamespace + ";assembly=" + assemblyName + "\"";
        }

        private static bool WriteAttribute(string name, object value, XmlWriter xw)
        {
            if (value == null)
                return true;

            Type t = value.GetType();

            string strValue = ObjectConverter.ToString(value, t);
            if (strValue == "")
                return true;

            if (strValue != null)
            {
                xw.WriteAttributeString(name, strValue);
                return true;
            }
            return false;
        }

        public static string ToString(object value, Type t)
        {
            if (value == null)
                return null;

            if (t.IsEnum)
                return Format((Enum)value);

            string type = t.FullName;
            switch (type)
            {
                case "System.String":
                    return value.ToString();
                case "System.Boolean":
                    return Format((bool)value);
                case "System.Int16":
                    return Format((short)value);
                case "System.Int32":
                    return Format((int)value);
                case "System.Double":
                    return Format((double)value);
                case "System.Windows.Point":
                    return Format((Point)value);
                case "System.Windows.Rect":
                    return Format((Rect)value);
                case "System.Windows.Size":
                    return Format((Size)value);
                case "System.Windows.Thickness":
                    return Format((Thickness)value);
                case "System.Windows.CornerRadius":
                    return Format((CornerRadius)value);
                case "System.Windows.GridLength":
                    return Format((GridLength)value);
                case "System.Windows.Input.Cursor":
                    return Format((Cursor)value);
                case "System.Windows.Media.Color":
                    return Format((Color)value);
                case "System.Uri":
                    return Format((Uri)value);
                case "System.Windows.Media.FontFamily":
                    return Format((FontFamily)value);
                case "System.Windows.FontStretch":
                    return Format((FontStretch)value);
                case "System.Windows.FontStyle":
                    return Format((FontStyle)value);
                case "System.Windows.FontWeight":
                    return Format((FontWeight)value);
                case "System.Windows.TextDecorationCollection":
                    return Format((TextDecorationCollection)value);
                case "System.Windows.Media.DoubleCollection":
                    return Format((DoubleCollection)value);
                case "System.Windows.Media.PointCollection":
                    return Format((PointCollection)value);
                case "System.Windows.Media.Imaging.BitmapImage":
                    return Format((BitmapImage)value);
                case "System.Windows.Media.SolidColorBrush":
                    return Format((SolidColorBrush)value);
            }
            return null;
        }

        private static void WriteChildElement(object value, string name, XmlWriter xw)
        {
            if (name == "")
            {
                ToXaml(value, xw);
                return;
            }
            
            xw.WriteStartElement(name);
            ToXaml(value, xw);
            xw.WriteEndElement();
        }

        #region Control Attribute
        public static string Format(bool value)
        {
            return value ? "true" : "false";
        }

        public static string Format(short value)
        {
            return value.ToString();
        }

        public static string Format(int value)
        {
            return value.ToString();
        }

        public static string Format(double value)
        {
            return value.ToString();
        }

        public static string Format(Enum value)
        {
            return value.ToString();
        }

        public static string Format(Point value)
        {
            return string.Format("{0},{1}", value.X, value.Y);
        }

        public static string Format(Rect value)
        {
            return string.Format("{0},{1},{2},{3}",
                    value.X,
                    value.Y,
                    value.Width,
                    value.Height);
        }

        public static string Format(Size value)
        {
            return string.Format("{0},{1}", value.Width, value.Height);
        }

        public static string Format(Thickness value)
        {
            if (value.Left == value.Right && value.Right == value.Top && value.Top == value.Bottom)
            {
                return value.Left.ToString();
            }
            else if (value.Left == value.Right && value.Top == value.Bottom)
            {
                return string.Format("{0},{1}", value.Left, value.Top);
            }
            return string.Format("{0},{1},{2},{3}", value.Left, value.Top, value.Right, value.Bottom);
        }

        public static string Format(CornerRadius value)
        {
            if (value.BottomLeft == value.BottomRight && value.BottomRight == value.TopLeft && value.TopLeft == value.TopRight)
            {
                return value.BottomLeft.ToString();
            }
            return string.Format("{0},{1},{2},{3}", value.TopLeft, value.TopRight, value.BottomRight, value.BottomLeft);
        }

        public static string Format(GridLength value)
        {
            string result;

            if (value.GridUnitType == GridUnitType.Auto)
            {
                result = "Auto";
            }
            else if (value.GridUnitType == GridUnitType.Pixel)
            {
                return value.Value.ToString();
            }
            else
            {
                if (value.Value == 1)
                {
                    result = "*";
                }
                else
                {
                    result = string.Format("{0}*", value.Value);
                }
            }
            return result;
        }

        public static string Format(Cursor value)
        {
            if (value == null)
            {
                return "Default";
            }
            return value.ToString();
        }

        public static string Format(Color value)
        {
            return value.ToString();
        }

        public static string Format(Uri value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.ToString();
        }

        public static string Format(FontFamily value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return value.Source.ToString();
        }

        public static string Format(FontStretch value)
        {
            return value.ToString();
        }

        public static string Format(FontStyle value)
        {
            return value.ToString();
        }

        public static string Format(FontWeight value)
        {
            return value.ToString();
        }

        public static string Format(TextDecorationCollection value)
        {
            if (value == null)
            {
                return "None";
            }
            else
            {
                return "Underline";
            }
        }

        public static string Format(DoubleCollection value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < value.Count; i++)
            {
                if (i > 0)
                {
                    b.Append(",");
                }
                b.Append(value[i]);
            }
            return b.ToString();
        }

        public static string Format(PointCollection value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < value.Count; i++)
            {
                if (i > 0)
                {
                    b.Append(" ");
                }
                b.Append(value[i]);
            }
            return b.ToString();
        }

        public static string Format(BitmapImage value)
        {
            return Format(value.UriSource);
        }

        public static string Format(SolidColorBrush value)
        {
            return Format(value.Color);
        }

        #endregion
    }
}
