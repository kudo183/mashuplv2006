using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SL30PropertyGrid
{
    #region Using Directives
	using System.Windows.Controls;
	using SL30PropertyGrid.Converters;

	#endregion

    #region FontValueEditor
    /// <summary>
    /// An editor for a Boolean Type
    /// </summary>
    public class FontWeightValueEditor : ComboBoxEditorBase
    {
        public FontWeightValueEditor(PropertyGridLabel label, PropertyItem property)
            : base(label, property)
        {
        }
        public override void InitializeCombo()
        {
            ComboBoxItem item = null;
            List<FontWeight> temp = new List<FontWeight>();
            temp.Add(FontWeights.Thin);
            temp.Add(FontWeights.ExtraLight);
            temp.Add(FontWeights.Light);
            temp.Add(FontWeights.Normal);
            temp.Add(FontWeights.Medium);
            temp.Add(FontWeights.SemiBold);
            temp.Add(FontWeights.Bold);
            temp.Add(FontWeights.ExtraBold);
            temp.Add(FontWeights.Black);
            temp.Add(FontWeights.ExtraBlack);
            
            foreach (FontWeight f in temp )
            {
                item = new ComboBoxItem();
                item.Content = f.ToString();
                item.FontWeight = f;
                this.cbo.Items.Add(item);
            }     
            //this.LoadItems(new List<object>(fontName));
        }

        protected override bool CompareItemValue(object value, object itemValue)
        {
            return ((FontWeight)value == ((ComboBoxItem)itemValue).FontWeight);
        }
        protected override void SetCurrentValue(object value)
        {
            base.SetCurrentValue(((ComboBoxItem)value).FontWeight);
        }
    }
    #endregion
}
