using System;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Windows.Media;
using System.Windows.Controls;
using System.Linq;

namespace BasicLibrary
{
    public class MyXmlSerializer
    {
        private static string[] skipPropertyList = new string[] 
                                                    {
                                                        "Template", "Resources", "Language", "RenderTransform"
                                                    };
        private static Type[] skipTypeList = new Type[] 
                                                    {
                                                    };

        public static string Serialize(object o)
        {
            return MyXmlSerializer.Serialize(o, "Root");
        }

        public static string Serialize(object o, string rootName)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter xm = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true });

            if (o.GetType().IsArray)
            {
                SerializeArray(o, xm, rootName);
                xm.Flush();
                xm.Close();
                return sb.ToString();
            }
            else if (typeof(IList).IsAssignableFrom(o.GetType()))
            {
                SerializeList(o, xm, rootName);
                xm.Flush();
                xm.Close();
                return sb.ToString();
            }
            
            xm.WriteStartElement(rootName);
            xm.WriteAttributeString("Type", o.GetType().AssemblyQualifiedName);
            Serialize(o, xm);

            xm.WriteEndElement();
            xm.Flush();
            xm.Close();
            return sb.ToString();
        }

        #region serialize list and collection
        private static void SerializeList(object o, XmlWriter xm)
        {
            SerializeList(o, xm, "Child");
        }
        private static void SerializeList(object o, XmlWriter xm, string rootName)
        {
            object lst = o;
            if (lst == null)
                return;
            int count = (int)(lst.GetType().GetProperty("Count").GetValue(lst, null));
            if (count == 0)
                return;
            object[] index = { 0 };
            object myObject = lst.GetType().GetProperty("Item").GetValue(lst, index);
            string elementType = myObject.GetType().AssemblyQualifiedName;

            xm.WriteStartElement(rootName);
            xm.WriteAttributeString("Type", o.GetType().AssemblyQualifiedName);
            for (int i = 0; i < count; i++)
            {
                index[0] = i;
                myObject = lst.GetType().GetProperty("Item").GetValue(lst, index);
                if (myObject.GetType().IsGenericType)
                {
                    SerializeList(myObject, xm);
                }
                else
                {
                    xm.WriteStartElement("Child");
                    xm.WriteAttributeString("Type", elementType);
                    Serialize(myObject, xm);
                    xm.WriteEndElement();
                }
            }
            xm.WriteEndElement();
        }
        //private static void SerializeList(object o, PropertyInfo pi, XmlWriter xm)
        //{
        //    object lst;
        //    int count;
        //    object[] index = { 0 };
        //    object myObject;
        //    string elementType;
        //    try
        //    {
        //        lst = pi.GetValue(o, null);
        //        if (lst == null)
        //            return;
        //        count = (int)(lst.GetType().GetProperty("Count").GetValue(lst, null));
        //        if (count == 0)
        //            return;
        //        myObject = lst.GetType().GetProperty("Item").GetValue(lst, index);
        //        elementType = myObject.GetType().AssemblyQualifiedName;
        //    }
        //    catch { return; }

        //    xm.WriteStartElement(pi.Name);
        //    xm.WriteAttributeString("Type", pi.PropertyType.AssemblyQualifiedName);
        //    for (int i = 0; i < count; i++)
        //    {
        //        index[0] = i;
        //        myObject = lst.GetType().GetProperty("Item").GetValue(lst, index);
        //        if (myObject.GetType().IsGenericType)
        //            SerializeList(myObject, xm);
        //        else
        //        {
        //            xm.WriteStartElement("Child");
        //            xm.WriteAttributeString("Type", elementType);
        //            Serialize(myObject, xm);
        //            xm.WriteEndElement();
        //        }
        //    }
        //    xm.WriteEndElement();
        //}
        #endregion

        #region serialize array
        private static void SerializeArray(object o, XmlWriter xm)
        {
            SerializeArray(o, xm, "Child");
        }
        private static void SerializeArray(object o, XmlWriter xm, string rootName)
        {
            object lst = o;
            if (lst == null)
                return;
            
            Array a = (Array)lst;
            if(a.Rank > 1)
                return;

            string elementType = a.GetType().GetElementType().AssemblyQualifiedName;
            
            xm.WriteStartElement(rootName);
            xm.WriteAttributeString("Type", o.GetType().AssemblyQualifiedName);

            if (a.GetType().GetElementType().IsArray)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    SerializeArray(a.GetValue(i), xm);
                }
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    xm.WriteStartElement("Child");
                    xm.WriteAttributeString("Type", elementType);
                    Serialize(a.GetValue(i), xm);
                    xm.WriteEndElement();
                }
            }
            xm.WriteEndElement();
        }
        //private static void SerializeArray(object o, PropertyInfo pi, XmlWriter xm)
        //{
        //    object lst = pi.GetValue(o, null);
        //    if (lst == null)
        //        return;
        //    int count = (int)(lst.GetType().GetProperty("Length").GetValue(lst, null));
        //    Array a = (Array)lst;
        //    string elementType = a.GetType().GetElementType().AssemblyQualifiedName;

        //    xm.WriteStartElement("Child");
        //    xm.WriteAttributeString("Type", pi.PropertyType.AssemblyQualifiedName);
        //    if (a.GetType().GetElementType().IsArray)
        //    {
        //        for (int i = 0; i < count; i++)
        //        {
        //            SerializeArray(a.GetValue(i), xm);
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < count; i++)
        //        {
        //            xm.WriteStartElement("Child");
        //            xm.WriteAttributeString("Type", elementType);
        //            Serialize(a.GetValue(i), xm);
        //            xm.WriteEndElement();
        //        }
        //    }
        //    xm.WriteEndElement();

        //}
        #endregion

        //private static void Serialize(object o, XmlWriter xm)
        //{
        //    if (o == null)
        //        return;

        //    Type t = o.GetType();

        //    //write element value, stop recursive
        //    if (t.IsPrimitive)
        //        xm.WriteValue(o.ToString());
        //    if (t == typeof(string) || typeof(Enum).IsAssignableFrom(t) || t == typeof(TimeSpan) || t == typeof(DateTime))
        //    {
        //        xm.WriteValue(o.ToString());
        //        return;
        //    }

        //    foreach (PropertyInfo pi in t.GetProperties())
        //    {
        //        bool b = false;
        //        if (skipPropertyList.Contains(pi.Name) || skipTypeList.Contains(pi.PropertyType))
        //            b = true;
        //        if (typeof(BasicLibrary.BasicEffect).IsAssignableFrom(pi.PropertyType))
        //            b = true;
        //        else if (typeof(BasicLibrary.BasicListEffect).IsAssignableFrom(pi.PropertyType))
        //            b = true;
        //        if (b == true)
        //            continue;

        //        if (pi.PropertyType.IsArray)
        //        {
        //            SerializeArray(o, pi, xm);

        //            continue;
        //        }

        //        if (typeof(IList).IsAssignableFrom(pi.PropertyType))
        //        {
        //            SerializeList(o, pi, xm);

        //            continue;
        //        }

        //        //skip dictionary type
        //        if (pi.GetIndexParameters().Length > 1)
        //            continue;

        //        if (pi.CanWrite == false || pi.GetSetMethod() == null)
        //            continue;

        //        object value = null;
        //        try { value = pi.GetValue(o, null); }
        //        catch { }

        //        if (value == null)
        //            continue;
        //        xm.WriteStartElement(pi.Name);
        //        xm.WriteAttributeString("Type", value.GetType().AssemblyQualifiedName);
        //        //if (value.GetType() == typeof(string) || typeof(Enum).IsAssignableFrom(value.GetType()))
        //        //    xm.WriteValue(value.ToString());
        //        //else
        //        Serialize(value, xm);
        //        xm.WriteEndElement();
        //    }
        //}
        private static void Serialize(object o, XmlWriter xm)
        {
            if (o == null)
                return;

            Type t = o.GetType();
            if (t == typeof(Type))
                return;
            if (t is Type)
            {
                int a = 0;
                a++;
            }
            else if (o is Type)
            {
                int a = 0;
                a++;
            }
            //write element value, stop recursive
            if (t.IsPrimitive)
                xm.WriteValue(o.ToString());
            if (t == typeof(string) || typeof(Enum).IsAssignableFrom(t) || t == typeof(TimeSpan) || t == typeof(DateTime))
            {
                xm.WriteValue(o.ToString());
                return;
            }

            foreach (PropertyInfo pi in t.GetProperties())
            {
                bool b = false;
                if (skipPropertyList.Contains(pi.Name) || skipTypeList.Contains(pi.PropertyType))
                    b = true;
                if (typeof(BasicLibrary.BasicEffect).IsAssignableFrom(pi.PropertyType))
                    b = true;
                else if (typeof(BasicLibrary.BasicListEffect).IsAssignableFrom(pi.PropertyType))
                    b = true;
                if (b == true)
                    continue;

                //skip indexed property
                if (pi.GetIndexParameters().Length > 0)
                    continue;

                if (pi.CanWrite == false || pi.GetSetMethod() == null)
                    continue;

                object value = null;
                try { value = pi.GetValue(o, null); }
                catch { }

                if (value == null)
                    continue;
                if (pi.PropertyType.IsArray)
                {
                    MyXmlSerializer.SerializeArray(value, xm);
                    continue;
                }
                if (typeof(IList).IsAssignableFrom(pi.PropertyType))
                {
                    MyXmlSerializer.SerializeList(value, xm);
                    continue;
                }
                xm.WriteStartElement(pi.Name);
                xm.WriteAttributeString("Type", value.GetType().AssemblyQualifiedName);
                Serialize(value, xm);
                xm.WriteEndElement();
            }
        }
        #region load
        public static object Deserialize(string xml)
        {
            object obj = null;
            try
            {
                XmlReader reader = XmlReader.Create(new StringReader(xml));
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                obj = Deserialize(root);
            }
            catch { }
            return obj;
        }

        public static object Deserialize(XElement root)
        {
            object obj = null;
            try
            {
                Type type = Type.GetType(root.Attribute("Type").Value);
                if (type.IsArray)
                    return DeserializeArray(null, root);

                if (typeof(IList).IsAssignableFrom(type))
                    return DeserializeList(root);

                obj = Activator.CreateInstance(type);
                if (root.HasElements)
                {
                    object value;
                    string propertyName;

                    foreach (XElement element in root.Elements())
                    {
                        propertyName = element.Name.ToString();
                        value = Deserialize(obj, element);
                        if (!typeof(IList).IsAssignableFrom(value.GetType()))
                            type.GetProperty(propertyName).SetValue(obj, value, null);
                    }
                }
                else
                {
                    return MyXmlSerializer.Deserialize(new object(), root);
                }
            }
            catch { }
            return obj;
        }

        public static void Deserialize(string xml, object obj)
        {
            try
            {
                XmlReader reader = XmlReader.Create(new StringReader(xml));
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                Type type;
                if (obj == null)
                {
                    type = Type.GetType(root.Attribute("Type").Value);
                    obj = Activator.CreateInstance(type);
                }
                else
                    type = obj.GetType();

                object value;
                string propertyName;
                foreach (XElement element in root.Elements())
                {
                    propertyName = element.Name.ToString();
                    value = Deserialize(obj, element);
                    if (!typeof(IList).IsAssignableFrom(value.GetType()))
                        type.GetProperty(propertyName).SetValue(obj, value, null);
                }
            }
            catch { }
        }

        private static object Deserialize(object obj, XElement element)
        {
            Type type = Type.GetType(element.Attribute("Type").Value);
            if (type.IsPrimitive)
            {
                return DeserializePrimitive(element);
            }
            else if (type.IsArray)
            {
                return DeserializeArray(obj, element);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                return DeserializeList(obj, element);
            }
            else
            {
                return DeserializeNonePrimitive(obj, element);
            }
        }

        private static object DeserializePrimitive(XElement element)
        {
            Type type = Type.GetType(element.Attribute("Type").Value);
            return Convert.ChangeType(element.Value, type, null);
        }

        private static object DeserializeArray(object obj, XElement element)
        {
            Type type = Type.GetType(element.Attribute("Type").Value);

            int count = 0;
            foreach (XElement child in element.Elements("Child"))
                count++;
            Array array = Array.CreateInstance(type.GetElementType(), count);

            count = 0;
            foreach (XElement child in element.Elements("Child"))
            {
                object temp = Deserialize(array, child);
                array.SetValue(temp, count);
                count++;
            }
            return array;
        }

        private static object DeserializeList(object obj, XElement element)
        {
            Type type = Type.GetType(element.Attribute("Type").Value);
            //IList list = (IList)Activator.CreateInstance(type);
            IList list = (IList)obj.GetType().GetProperty(element.Name.ToString()).GetValue(obj, null);

            foreach (XElement child in element.Elements("Child"))
            {
                //object temp = Load(list, child);
                object temp = Deserialize(child.ToString(SaveOptions.DisableFormatting));
                list.Add(temp);
            }
            return list;
        }

        private static object DeserializeList(XElement element)
        {
            Type type = Type.GetType(element.Attribute("Type").Value);
            IList list = (IList)Activator.CreateInstance(type);
            foreach (XElement child in element.Elements("Child"))
            {
                //object temp = Load(list, child);
                object temp = Deserialize(child.ToString(SaveOptions.DisableFormatting));
                list.Add(temp);
            }
            return list;
        }

        private static object DeserializeNonePrimitive(object obj, XElement element)
        {
            Type type = Type.GetType(element.Attribute("Type").Value);
            if (type == typeof(System.String))
                return element.Value;
            if (type == typeof(FontFamily))
            {
                string font = "Portable User Interface";
                if (element.Value != "")
                    font = element.Value;
                return new FontFamily(font);
            }
            if (typeof(Enum).IsAssignableFrom(type))
                return Enum.Parse(type, element.Value.ToString(), true);
            if (type == typeof(TimeSpan))
                return TimeSpan.Parse(element.Value);
            if (type == typeof(DateTime))
                return DateTime.Parse(element.Value);

            object obj1 = Activator.CreateInstance(type);
            string propertyName;
            object value;
            foreach (XElement child in element.Elements())
            {
                if (type == typeof(ControlTemplate))
                    continue;
                propertyName = child.Name.ToString();
                value = Deserialize(obj1, child);
                if (!typeof(IList).IsAssignableFrom(value.GetType()))
                    type.GetProperty(propertyName).SetValue(obj1, value, null);
            }

            return obj1;
        }
        #endregion load
    }
}
