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
    public class FontFamilyValueEditor : ComboBoxEditorBase
    {
        public FontFamilyValueEditor(PropertyGridLabel label, PropertyItem property)
            : base(label, property)
        {
        }
        public override void InitializeCombo()
        {
            string[] fontName = new string[]{
                "Portable User Interface",
                "Arial", 
                "Arial Black", 
                "Comic Sans MS", 
                "Courier New", 
                "Goergia", 
                "Lucida Grande", 
                "Lucida Grande Unicode", 
                "Times New Roman", 
                "Trebuchet MS", 
                "Verdana"};
            ComboBoxItem item = null;
            foreach (string s in fontName)
            {
                item = new ComboBoxItem();
                item.Content = s;
                item.FontFamily = new FontFamily(s);
                this.cbo.Items.Add(item);
            }     
            //this.LoadItems(new List<object>(fontName));
        }

        protected override bool CompareItemValue(object value, object itemValue)
        {
            return (((FontFamily)value).Source == (((ComboBoxItem)itemValue).FontFamily).Source);
        }
        protected override void SetCurrentValue(object value)
        {
            base.SetCurrentValue(((ComboBoxItem)value).FontFamily);
        }
    }
    #endregion
}
