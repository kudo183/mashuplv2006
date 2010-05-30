using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace Liquid
{
    public partial class RichTextEditor : UserControl
    {
        #region Private Properties

        private bool _ignoreFormattingChanges = false;
        private SolidColorBrush _buttonFillStyleNotApplied = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        private SolidColorBrush _buttonFillStyleApplied = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        #endregion

        #region Public Properties

        public string Html
        {
            get
            {
                return rtb.Save(Format.HTML, RichTextSaveOptions.InlineStyles | RichTextSaveOptions.ExcludeCustomStyles);
            }
            set
            {
                rtb.Load(Format.HTML, value);
            }
        }

        public bool IsReadOnly
        {
            get { return rtb.IsReadOnly; }
            set { rtb.IsReadOnly = value; }
        }

        public RichTextBox TextBox
        {
            get { return rtb; }
        }

        #endregion
        FloatableWindow f;
        public RichTextEditor()
        {
            InitializeComponent();
            Setup();
        }

        RichTextBox editedRtb;
        public RichTextEditor(Panel p, RichTextBox r)
        {
            InitializeComponent();
            Setup();
            f = new FloatableWindow();
            f.ParentLayoutRoot = p;
            f.Content = this;
            f.HasCloseButton = true;
            f.Title = "Rich text editor";
            f.Width = 600;
            f.Height = 400;
            f.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(f_Closing);
            Html = r.HTML;
            editedRtb = r;
        }

        void f_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            editedRtb.RichText = rtb.RichText;
        }

        public void ShowDialog()
        {
            f.ShowDialog();
        }

        private void Setup()
        {
            rtb.EnableGlobalLinkStyle = true;

            linkDialog.CreateButton(DialogButtons.Custom, "Remove", "remove", true);
            //URLDialog.CreateButton(DialogButtons.Custom, "Remove", "remove", true);
            rtb.EnablePatternRecognition = true;
            rtb.SelectionChanged += new RichTextBoxEventHandler(rtb_SelectionChanged);
            rtb.TextPatternMatch += new RichTextBoxEventHandler(rtb_TextPatternMatch);
            rtb.ContentChanged += new RichTextBoxEventHandler(rtb_ContentChanged);

            SetupPatternMatching();
            SetupColors(selectForegroundColor);

            selectForegroundColor.CustomClick += new RoutedEventHandler(selectForegroundColor_CustomClick);
            selectBackgroundColor.CustomClick += new RoutedEventHandler(selectBackgroundColor_CustomClick);
        }

        private void selectForegroundColor_CustomClick(object sender, RoutedEventArgs e)
        {
            colorPickerForeground.Build((ColorSelector)sender);
            colorPickerForegroundDialog.ShowAsModal();
        }

        private void selectBackgroundColor_CustomClick(object sender, RoutedEventArgs e)
        {
            colorPickerBackground.Build((ColorSelector)sender);
            colorPickerBackgroundDialog.ShowAsModal();
        }

        private void SetupColors(ColorSelector picker)
        {
            uint[] colors = new uint[] {
                0xFF000000, 0x00FFFFFF, 0xFF333300, 0xFF003300, 0xFF003366, 0xFF000080, 0xFF333399, 0xFF333333,
                0xFF800000, 0xFFFF6600, 0xFF808000, 0xFF008000, 0xFF008080, 0xFF0000FF, 0xFF666699, 0xFF808080,
                0xFFFF0000, 0xFFFF9900, 0xFF99CC00, 0xFF339966, 0xFF33CCCC, 0xFF3366FF, 0xFF800080, 0xFF999999,
                0xFFFF00FF, 0xFFFFCC00, 0xFFFFFF00, 0xFF00FF00, 0xFF00FFFF, 0xFF00CCFF, 0xFF993366, 0xFFC0C0C0,
                0xFFFF99CC, 0xFFFFCC99, 0xFFFFFF99, 0xFFCCFFCC, 0xFFCCFFFF, 0xFF99CCFF, 0xFFCC99FF, 0xFFFFFFFF
            };

            picker.Set(colors, false);
        }

        private void rtb_ContentChanged(object sender, RichTextBoxEventArgs e)
        {
        }

        private void rtb_SelectionChanged(object sender, RichTextBoxEventArgs e)
        {
            UpdateFormattingControls();
        }

        private void SetupPatternMatching()
        {
            rtb.TextPatterns.Add(":)");
            rtb.TextPatterns.Add(";)");
            rtb.TextPatterns.Add(":(");
            rtb.TextPatterns.Add("(c)");
            rtb.TextPatterns.Add("(C)");
        }

        private void rtb_TextPatternMatch(object sender, RichTextBoxEventArgs e)
        {
            switch (e.Parameter.ToString())
            {
                case ":)":
                    e.Parameter = new Image() { Source = new BitmapImage(new Uri("images/happy.png", UriKind.Relative)) };
                    break;
                case ":(":
                    e.Parameter = new Image() { Source = new BitmapImage(new Uri("images/unhappy.png", UriKind.Relative)) };
                    break;
                case ";)":
                    e.Parameter = new Image() { Source = new BitmapImage(new Uri("images/wink.png", UriKind.Relative)) };
                    break;
                case "(c)":
                case "(C)":
                    e.Parameter = "©";
                    break;
            }
        }

        #region Formatting

        public void UpdateFormattingControls()
        {
            makeBold.Background = _buttonFillStyleNotApplied;
            makeItalic.Background = _buttonFillStyleNotApplied;
            makeUnderline.Background = _buttonFillStyleNotApplied;
            makeLeft.Background = _buttonFillStyleNotApplied;
            makeCenter.Background = _buttonFillStyleNotApplied;
            makeRight.Background = _buttonFillStyleNotApplied;
            link.Background = _buttonFillStyleNotApplied;
            makeSuperscript.Background = _buttonFillStyleNotApplied;
            makeSubscript.Background = _buttonFillStyleNotApplied;
            makeStrike.Background = _buttonFillStyleNotApplied;
            bulletList.Background = _buttonFillStyleNotApplied;
            numberList.Background = _buttonFillStyleNotApplied;

            if (rtb.SelectionMetadata != null)
            {
                if (rtb.SelectionMetadata.IsLink)
                {
                    link.Background = _buttonFillStyleApplied;
                }
            }

            if (rtb.SelectionStyle != null)
            {
                SetSelected(selectFontFamily, rtb.SelectionStyle.Family);
                SetSelected(selectFontSize, rtb.SelectionStyle.Size.ToString());

                if (rtb.SelectionStyle.Weight == FontWeights.Bold)
                {
                    makeBold.Background = _buttonFillStyleApplied;
                }

                if (rtb.SelectionStyle.Style == FontStyles.Italic)
                {
                    makeItalic.Background = _buttonFillStyleApplied;
                }

                if (rtb.SelectionStyle.Decorations == TextDecorations.Underline)
                {
                    makeUnderline.Background = _buttonFillStyleApplied;
                }

                if (rtb.SelectionAlignment == HorizontalAlignment.Left)
                {
                    makeLeft.Background = _buttonFillStyleApplied;
                }
                else if (rtb.SelectionAlignment == HorizontalAlignment.Center)
                {
                    makeCenter.Background = _buttonFillStyleApplied;
                }
                else if (rtb.SelectionAlignment == HorizontalAlignment.Right)
                {
                    makeRight.Background = _buttonFillStyleApplied;
                }

                if (rtb.SelectionStyle.Effect == TextBlockPlusEffect.Strike)
                {
                    makeStrike.Background = _buttonFillStyleApplied;
                }

                if (rtb.SelectionStyle.Special == RichTextSpecialFormatting.Superscript)
                {
                    makeSuperscript.Background = _buttonFillStyleApplied;
                }

                if (rtb.SelectionStyle.Special == RichTextSpecialFormatting.Subscript)
                {
                    makeSubscript.Background = _buttonFillStyleApplied;
                }

                if (rtb.SelectionStyle.Foreground != null)
                {
                    selectForegroundColor.Selected = ((SolidColorBrush)rtb.SelectionStyle.Foreground).Color;
                }
                if (rtb.SelectionStyle.Background != null)
                {
                    selectBackgroundColor.Selected = ((SolidColorBrush)rtb.SelectionStyle.Background).Color;
                }
            }
            if (rtb.SelectionListType != null)
            {
                if (rtb.SelectionListType.Type == BulletType.Bullet)
                {
                    bulletList.Background = _buttonFillStyleApplied;
                }
                else if (rtb.SelectionListType.Type == BulletType.Number)
                {
                    numberList.Background = _buttonFillStyleApplied;
                }
            }
        }

        private void SetSelected(ComboBox combo, string value)
        {
            bool found = false;

            if (value != null)
            {
                _ignoreFormattingChanges = true;

                foreach (ComboBoxItem item in combo.Items)
                {
                    if (item.Content.ToString().ToLower() == value.ToLower())
                    {
                        combo.SelectedItem = item;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    combo.SelectedIndex = -1;
                }

                _ignoreFormattingChanges = false;
            }
        }

        private void SelectFontFamily_ItemSelected(object sender, EventArgs e)
        {
            if (selectFontFamily != null)
            {
                ExecuteFormatting(Formatting.FontFamily, ((ComboBoxItem)selectFontFamily.SelectedItem).FontFamily.Source);
            }
        }

        private void SelectFontSize_ItemSelected(object sender, EventArgs e)
        {
            if (selectFontSize != null)
            {
                ExecuteFormatting(Formatting.FontSize, double.Parse(((ComboBoxItem)selectFontSize.SelectedItem).Content.ToString()));
            }
        }

        private void MakeBold_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeBold.Background == _buttonFillStyleNotApplied ? Formatting.Bold : Formatting.RemoveBold);

            ExecuteFormatting(format, null);
        }

        private void MakeItalic_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeItalic.Background == _buttonFillStyleNotApplied ? Formatting.Italic : Formatting.RemoveItalic);

            ExecuteFormatting(format, null);
        }

        private void MakeUnderline_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeUnderline.Background == _buttonFillStyleNotApplied ? Formatting.Underline : Formatting.RemoveUnderline);

            ExecuteFormatting(format, null);
        }

        private void MakeLeft_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.AlignLeft, null);
        }

        private void MakeCenter_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.AlignCenter, null);
        }

        private void MakeRight_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.AlignRight, null);
        }

        private void Indent_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.Indent, null);
        }

        private void Outdent_Click(object sender, RoutedEventArgs e)
        {
            ExecuteFormatting(Formatting.Outdent, null);
        }

        private void BulletList_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (bulletList.Background == _buttonFillStyleNotApplied ? Formatting.BulletList : Formatting.RemoveBullet);

            ExecuteFormatting(format, null);
        }

        private void NumberList_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (numberList.Background == _buttonFillStyleNotApplied ? Formatting.NumberList : Formatting.RemoveNumber);

            ExecuteFormatting(format, null);
        }

        private void Strike_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeStrike.Background == _buttonFillStyleNotApplied ? Formatting.Strike : Formatting.RemoveStrike);

            ExecuteFormatting(format, null);
        }

        private void Superscript_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeSuperscript.Background == _buttonFillStyleNotApplied ? Formatting.SuperScript : Formatting.RemoveSpecial);

            ExecuteFormatting(format, null);
        }

        private void Subscript_Click(object sender, RoutedEventArgs e)
        {
            Formatting format = (makeSubscript.Background == _buttonFillStyleNotApplied ? Formatting.SubScript : Formatting.RemoveSpecial);

            ExecuteFormatting(format, null);
        }

        private void selectForegroundColor_ItemSelected(object sender, EventArgs e)
        {
            if (selectForegroundColor != null)
            {
                ExecuteFormatting(Formatting.Foreground, selectForegroundColor.Selected.ToString());
            }
        }

        private void selectBackgroundColor_SelectionChanged(object sender, EventArgs e)
        {
            if (selectBackgroundColor != null)
            {
                ExecuteFormatting(Formatting.Background, selectBackgroundColor.Selected.ToString());
            }
        }

        public void ExecuteFormatting(Formatting format, object param)
        {
            rtb.ApplyFormatting(format, param);
            rtb.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "cut":
                    rtb.Cut();
                    break;
                case "copy":
                    rtb.Copy();
                    break;
                case "paste":
                    rtb.Paste();
                    break;
                case "undo":
                    rtb.Undo();
                    break;
                case "redo":
                    rtb.Redo();
                    break;
                case "link":
                    linkDialog.ShowAsModal();
                    break;
                case "image":
                    URLDialog.ShowAsModal();
                    break;
                case "painter":
                    rtb.Painter();
                    break;
            }
        }

        private void linkDialog_Closed(object sender, DialogEventArgs e)
        {
            string link = "http://" + ElementLink.Text.Replace("http://", "");

            if (e.Tag.ToString() == "remove")
            {
                ExecuteFormatting(Formatting.RemoveLink, link);
            }
            else if (e.Tag.ToString() == "ok")
            {
                ExecuteFormatting(Formatting.Link, link);
            }
        }

        #endregion

        #region Color Picker Dialog

        private void colorPickerForegroundDialog_Closed(object sender, DialogEventArgs e)
        {
            if (colorPickerForegroundDialog.Result == DialogButtons.OK)
            {
                uint col = (uint)((colorPickerForeground.Selected.A << 24) | (colorPickerForeground.Selected.R << 16) | (colorPickerForeground.Selected.G << 8) | colorPickerForeground.Selected.B);
                ColorSelector.Custom[ColorSelector.NextCustomSlot] = col;

                selectForegroundColor.Set(ColorSelector.Custom.ToArray(), true);
                colorPickerForeground.Selector.Select(colorPickerForeground.Selected);
            }
        }

        private void colorPickerBackgroundDialog_Closed(object sender, DialogEventArgs e)
        {
            if (colorPickerBackgroundDialog.Result == DialogButtons.OK)
            {
                uint col = (uint)((colorPickerBackground.Selected.A << 24) | (colorPickerBackground.Selected.R << 16) | (colorPickerBackground.Selected.G << 8) | colorPickerBackground.Selected.B);
                ColorSelector.Custom[ColorSelector.NextCustomSlot] = col;

                selectBackgroundColor.Set(ColorSelector.Custom.ToArray(), true);
                colorPickerBackground.Selector.Select(colorPickerBackground.Selected);
            }
        }
        #endregion

        private void URLDialog_Closed(object sender, DialogEventArgs e)
        {
            string s = "<Xaml><Image Source=\"URL\"/></Xaml>";
            if (URLDialog.Result == DialogButtons.OK)
            {
                s = s.Replace("URL", URL.Text);
                rtb.Insert(s);
            }
        }

        private void sliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (rtb != null)
                rtb.Zoom = e.NewValue;
        }

    }
}
