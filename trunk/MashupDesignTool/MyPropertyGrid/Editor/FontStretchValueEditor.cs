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
    public class FontStretchValueEditor : ComboBoxEditorBase
    {
        public FontStretchValueEditor(PropertyGridLabel label, PropertyItem property)
            : base(label, property)
        {
        }
        public override void InitializeCombo()
        {
            ComboBoxItem item = null;
            List<FontStretch> temp = new List<FontStretch>();
            temp.Add(FontStretches.UltraCondensed);
            temp.Add(FontStretches.ExtraCondensed);
            temp.Add(FontStretches.Condensed);
            temp.Add(FontStretches.SemiCondensed);
            temp.Add(FontStretches.Normal);
            temp.Add(FontStretches.SemiExpanded);
            temp.Add(FontStretches.Expanded);
            temp.Add(FontStretches.ExtraExpanded);
            temp.Add(FontStretches.UltraExpanded);
            
            foreach (FontStretch f in temp )
            {
                item = new ComboBoxItem();
                item.Content = f.ToString();
                item.FontStretch = f;
                this.cbo.Items.Add(item);
            }     
            //this.LoadItems(new List<object>(fontName));
        }

        protected override bool CompareItemValue(object value, object itemValue)
        {
            return ((FontStretch)value == ((ComboBoxItem)itemValue).FontStretch);
        }
        protected override void SetCurrentValue(object value)
        {
            base.SetCurrentValue(((ComboBoxItem)value).FontStretch);
        }
    }
    #endregion
}
