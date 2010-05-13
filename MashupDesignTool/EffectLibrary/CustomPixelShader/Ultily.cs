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
using System.Reflection;

namespace EffectLibrary
{
    public class Ultily
    {
        public static Uri MakePackUri(string relativeFile)
        {
            string uriString = "/" + AssemblyShortName + ";component/CustomPixelShader/HLSL/" + relativeFile;
            return new Uri(uriString, UriKind.Relative);
        }

        private static string AssemblyShortName
        {
            get
            {
                if (_assemblyShortName == null)
                {
                    Assembly a = typeof(Ultily).Assembly;
                    // Pull out the short name.
                    _assemblyShortName = a.ToString().Split(',')[0];
                }
                return _assemblyShortName;
            }
        }

        private static string _assemblyShortName;
    }
}
