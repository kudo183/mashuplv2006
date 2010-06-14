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
using BasicLibrary;
using EffectLibrary;

namespace ControlLibrary
{
    public class DataListControl:BasicDataListControl
    {
        public DataListControl()
            : base()
        {
            Width = Height = 100;
            listEffect = new EffectLibrary.CoverFlow(this);
        }
    }
}
