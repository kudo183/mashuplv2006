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
    public class FontStyleValueEditor : ComboBoxEditorBase
    {
        public FontStyleValueEditor(PropertyGridLabel label, PropertyItem property)
            : base(label, property)
        {
        }
        public override void InitializeCombo()
        {
            ComboBoxItem item = null;
            List<FontStyle> temp = new List<FontStyle>();
            temp.Add(FontStyles.Normal);
            temp.Add(FontStyles.Italic);            
            foreach (FontStyle f in temp )
            {
                item = new ComboBoxItem();
                item.Content = f.ToString();
                item.FontStyle = f;
                this.cbo.Items.Add(item);
            }     
            //this.LoadItems(new List<object>(fontName));
        }

        protected override bool CompareItemValue(object value, object itemValue)
        {
            return ((FontStyle)value == ((ComboBoxItem)itemValue).FontStyle);
        }
        protected override void SetCurrentValue(object value)
        {
            base.SetCurrentValue(((ComboBoxItem)value).FontStyle);
        }
    }
    #endregion
}
