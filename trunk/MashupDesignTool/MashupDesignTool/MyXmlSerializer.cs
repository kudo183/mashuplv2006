using System;
using System.Text;
using System.Xml;
using System.Reflection;

using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
namespace MashupDesignTool
{
    public class MyXmlSerializer
    {
        public static string Serialize(object o)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter xm = XmlWriter.Create(sb);
            xm.WriteStartElement(o.GetType().Name);
            Serialize(o, xm);
            xm.WriteEndElement();
            xm.Flush();
            xm.Close();
            return sb.ToString();
        }

        #region serialize list and collection
        private static void SerializeList(object o, XmlWriter xm)
        {
            object lst = o;
            if (lst == null)
                return;
            int count = (int)(lst.GetType().GetProperty("Count").GetValue(lst, null));
            if (count == 0)
                return;
            object[] index = { 0 };
            object myObject = lst.GetType().GetProperty("Item").GetValue(lst, index);
            string elementType = myObject.GetType().FullName;

            xm.WriteStartElement("Child", o.GetType().FullName);
            xm.WriteAttributeString("Type", elementType);
            for (int i = 0; i < count; i++)
            {
                index[0] = i;
                myObject = lst.GetType().GetProperty("Item").GetValue(lst, index);
                if (myObject.GetType().IsGenericType)
                    SerializeList(myObject, null, xm);
                xm.WriteStartElement("Child");
                xm.WriteAttributeString("Type", elementType);
                Serialize(myObject, xm);
                xm.WriteEndElement();
            }
            xm.WriteEndElement();
        }
        private static void SerializeList(object o, PropertyInfo pi, XmlWriter xm)
        {
            object lst = pi.GetValue(o, null);
            if (lst == null)
                return;
            int count = (int)(lst.GetType().GetProperty("Count").GetValue(lst, null));
            if (count == 0)
                return;
            object[] index = { 0 };
            object myObject = lst.GetType().GetProperty("Item").GetValue(lst, index);
            string elementType = myObject.GetType().FullName;

            xm.WriteStartElement(pi.Name);
            xm.WriteAttributeString("Type", pi.PropertyType.FullName);
            for (int i = 0; i < count; i++)
            {
                index[0] = i;
                myObject = lst.GetType().GetProperty("Item").GetValue(lst, index);
                if (myObject.GetType().IsGenericType)
                    SerializeList(myObject, xm);
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
        #endregion

        #region serialize array
        private static void SerializeArray(object o, XmlWriter xm)
        {
            object lst = o;
            if (lst == null)
                return;
            int count = (int)(lst.GetType().GetProperty("Length").GetValue(lst, null));
            Array a = (Array)lst;
            string elementType = a.GetType().GetElementType().FullName;

            xm.WriteStartElement("Child");
            xm.WriteAttributeString("Type", o.GetType().FullName);
            for (int i = 0; i < count; i++)
            {
                xm.WriteStartElement("Child");
                xm.WriteAttributeString("Type", elementType);
                Serialize(a.GetValue(i), xm);
                xm.WriteEndElement();
            }
            xm.WriteEndElement();
        }
        private static void SerializeArray(object o, PropertyInfo pi, XmlWriter xm)
        {
            object lst = pi.GetValue(o, null);
            if (lst == null)
                return;
            int count = (int)(lst.GetType().GetProperty("Length").GetValue(lst, null));
            Array a = (Array)lst;
            string elementType = a.GetType().GetElementType().FullName;

            xm.WriteStartElement("Child");
            xm.WriteAttributeString("Type", pi.PropertyType.FullName);
            if (a.GetType().GetElementType().IsArray)
            {
                for (int i = 0; i < count; i++)
                {
                    SerializeArray(a.GetValue(i), xm);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    xm.WriteStartElement("Child");
                    xm.WriteAttributeString("Type", elementType);
                    Serialize(a.GetValue(i), xm);
                    xm.WriteEndElement();
                }
            }
            xm.WriteEndElement();

        }
        #endregion

        private static void Serialize(object o, XmlWriter xm)
        {
            if (o == null)
                return;

            Type t = o.GetType();

            //write element value, stop recursive
            if (t.IsPrimitive)
                xm.WriteValue(o.ToString());

            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (pi.PropertyType.IsArray)
                {
                    SerializeArray(o, pi, xm);

                    continue;
                }

                if (pi.PropertyType.IsGenericType == true || (typeof(IList).IsAssignableFrom(pi.PropertyType)) == true)
                {
                    SerializeList(o, pi, xm);

                    continue;
                }

                //skip dictionary type
                if (pi.GetIndexParameters().Length > 0)
                    continue;

                if (pi.CanWrite == false || pi.GetSetMethod() == null)
                    continue;

                object value = pi.GetValue(o, null);

                if (value == null)
                    continue;
                xm.WriteStartElement(pi.Name);
                xm.WriteAttributeString("Type", value.GetType().FullName);
                Serialize(value, xm);
                xm.WriteEndElement();
            }
        }
        
    }
}
