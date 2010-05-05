using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Reflection;

namespace Effect
{
    internal class Magnifier : ShaderEffectBase
    {
        private static PixelShader pixelShader = new PixelShader();

        /// <summary>
        /// Gets or sets the magnifier center.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register( "Center", typeof( Point ), typeof( Magnifier ),
                new PropertyMetadata( new Point( 0.5, 0.5 ), PixelShaderConstantCallback( 0 ) ) );

        /// <summary>
        /// Gets or sets the magnifier inner radius.
        /// </summary>
        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register( "InnerRadius", typeof( double ), typeof( Magnifier ),
                new PropertyMetadata( .2, PixelShaderConstantCallback( 2 ) ) );

        /// <summary>
        /// Gets or sets the magnification.
        /// </summary>
        public static readonly DependencyProperty MagnificationProperty =
            DependencyProperty.Register( "Magnification", typeof( double ), typeof( Magnifier ),
                new PropertyMetadata( 2.0, PixelShaderConstantCallback( 3 ) ) );

        /// <summary>
        /// Gets or sets the magnifier outer radius.
        /// </summary>
        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register( "OuterRadius", typeof( double ), typeof( Magnifier ),
                new PropertyMetadata( .27, PixelShaderConstantCallback( 4 ) ) );

        static Magnifier()
        {
            pixelShader.UriSource = Global.MakePackUri( "Source/Magnifier.ps" );
        }

        public Magnifier()
        {
            this.PixelShader = pixelShader;
            this.UpdateShaderValue( CenterProperty );
            this.UpdateShaderValue( InnerRadiusProperty );
            this.UpdateShaderValue( OuterRadiusProperty );
            this.UpdateShaderValue( MagnificationProperty );
        }

        /// <summary>
        /// Gets or sets the magnifier center.
        /// </summary>
        public Point Center
        {
            get
            {
                return ( Point ) this.GetValue( CenterProperty );
            }
            set
            {
                this.SetValue( CenterProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the magnifier inner radius.
        /// </summary>
        public double InnerRadius
        {
            get
            {
                return ( double ) this.GetValue( InnerRadiusProperty );
            }
            set
            {
                this.SetValue( InnerRadiusProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the magnification.
        /// </summary>
        public double Magnification
        {
            get
            {
                return ( double ) this.GetValue( MagnificationProperty );
            }
            set
            {
                this.SetValue( MagnificationProperty, value );
            }
        }

        /// <summary>
        /// Gets or sets the magnifier outer radius.
        /// </summary>
        public double OuterRadius
        {
            get
            {
                return ( double ) this.GetValue( OuterRadiusProperty );
            }
            set
            {
                this.SetValue( OuterRadiusProperty, value );
            }
        }
    }

    internal static class Global
    {
        public static Uri MakePackUri(string relativeFile)
        {
            string uriString = "/" + AssemblyShortName + ";component/" + relativeFile;
            return new Uri(uriString, UriKind.Relative);
        }

        private static string AssemblyShortName
        {
            get
            {
                if (_assemblyShortName == null)
                {
                    Assembly a = typeof(Global).Assembly;
                    // Pull out the short name.
                    _assemblyShortName = a.ToString().Split(',')[0];
                }
                return _assemblyShortName;
            }
        }

        private static string _assemblyShortName;
    }
}
