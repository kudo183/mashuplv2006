using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Media;

namespace Effect
{
    /// <summary>
    /// Base class for all shader effects used in the application.
    /// </summary>
    internal abstract class ShaderEffectBase : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty( "Input", typeof( ShaderEffectBase ), 0, SamplingMode.NearestNeighbor );

        protected ShaderEffectBase()
        {
            this.UpdateShaderValue( InputProperty );
        }

        /// <summary>
        /// Gets, Sets the effect input.
        /// </summary>
        public Brush Input
        {
            get
            {
                return this.GetValue( InputProperty ) as Brush;
            }
            set
            {
                this.SetValue( InputProperty, value );
            }
        }

        public string Name
        {
            get;
            set;
        }
    }
}
