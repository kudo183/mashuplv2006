using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Diagnostics;

namespace EffectLibrary.CustomPixelShader
{
    public class RippleShader : CustomPixelShaderBase
	{

		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(RippleShader), 0);

		public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(RippleShader), new System.Windows.PropertyMetadata(new double(), PixelShaderConstantCallback(0)));

        public RippleShader()
		{
            parameterNameList.Add("Input");
            parameterNameList.Add("Progress");

            this.PixelShader = new PixelShader(); 
            this.PixelShader.UriSource = Ultily.MakePackUri(@"Ripple.ps");
			this.UpdateShaderValue(InputProperty);
			this.UpdateShaderValue(ProgressProperty);
		}

		[System.ComponentModel.BrowsableAttribute(false)]
		public Brush Input
		{
			get
			{
				return ((System.Windows.Media.Brush)(GetValue(InputProperty)));
			}
			set
			{
				SetValue(InputProperty, value);
			}
		}

		public double Progress
		{
			get
			{
				return ((double)(GetValue(ProgressProperty)));
			}
			set
			{
				SetValue(ProgressProperty, value);
			}
		}
	}
}
