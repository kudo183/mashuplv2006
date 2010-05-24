using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Xml;

namespace BasicLibrary
{
    public class ImageListControl : BasicListControl
    {
        public ImageListControl()
            : base()
        {
            int count = 5;
            Width = Height = 150;
            for (int i = 0; i < count; i++)
                AddItem(new ImageListControlItems() { Width = 50, Height = 50 });
        }
    }
}
