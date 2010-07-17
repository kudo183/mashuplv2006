using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Liquid;
namespace MyMenu
{
    public class SevenMenu : BasicLibrary.Menu.BasicMenu
    {
        Grid LayoutRoot = new Grid() { };
        StackPanel spMenu;

        #region menu property
        Brush _Background;
        [Category("Menu")]
        public new Brush Background
        {
            get { return _Background; }
            set
            {
                _Background = value;
                LayoutRoot.Background = Background;
            }
        }

        byte _TransitionAlpha;
        [Category("Menu")]
        public byte TransitionAlpha
        {
            get { return _TransitionAlpha; }
            set
            {
                _TransitionAlpha = value;
                foreach (EffectLibrary.GlowWinSeven g in lstGS)
                    g.TransitionAlpha = _TransitionAlpha;
            }
        }

        Color _TransitionColor;
        [Category("Menu")]
        public Color TransitionColor
        {
            get { return _TransitionColor; }
            set
            {
                _TransitionColor = value;
                foreach (EffectLibrary.GlowWinSeven g in lstGS)
                    g.TransitionColor = _TransitionColor;
            }
        }

        double _ItemWidth;
        [Category("Menu")]
        public double ItemWidth
        {
            get { return _ItemWidth; }
            set
            {
                _ItemWidth = value;
                foreach (BasicLibrary.EffectableControl eff in lstEff)
                    eff.Width = _ItemWidth;
            }
        }

        double _ItemHeight;
        [Category("Menu")]
        public double ItemHeight
        {
            get { return _ItemHeight; }
            set
            {
                _ItemHeight = value;
                foreach (BasicLibrary.EffectableControl eff in lstEff)
                    eff.Height = _ItemHeight;
            }
        }
        #endregion

        #region text block
        FontFamily _Font;
        [Category("MenuText")]
        public FontFamily Font
        {
            get { return _Font; }
            set
            {
                _Font = value;
                tbTooptip.FontFamily = _Font;
            }
        }
        double _TextSize;
        [Category("MenuText")]
        public double TextSize
        {
            get { return _TextSize; }
            set
            {
                _TextSize = value;
                tbTooptip.FontSize = _TextSize;
            }
        }

        Brush _TextColor;
        [Category("MenuText")]
        public Brush TextColor
        {
            get { return _TextColor; }
            set
            {
                _TextColor = value;
                tbTooptip.Foreground = _TextColor;
            }
        }

        Brush _TextBackground;
        [Category("MenuText")]
        public Brush TextBackground
        {
            get { return _TextBackground; }
            set
            {
                _TextBackground = value;
                tbTooptip.Foreground = _TextBackground;
            }
        }

        double _TextAreaWidth;
        [Category("MenuText")]
        public double TextAreaWidth
        {
            get { return _TextAreaWidth; }
            set
            {
                _TextAreaWidth = value;
                tbTooptip.Width = _TextAreaWidth;
            }
        }
        #endregion
        public SevenMenu()
            : base()
        {
            parameterNameList.Add("Background");
            parameterNameList.Add("TransitionAlpha");
            parameterNameList.Add("TransitionColor");
            parameterNameList.Add("ItemWidth");
            parameterNameList.Add("ItemHeight");

            parameterNameList.Add("TextAreaWidth");
            parameterNameList.Add("Font");
            parameterNameList.Add("TextSize");
            parameterNameList.Add("TextColor");
            parameterNameList.Add("TextBackground");

            _Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x9A, 0x09, 0x09));
            _TransitionAlpha = 0x64;
            _TransitionColor = Colors.Blue;
            _ItemWidth = 70;
            _ItemHeight = 50;

            _Font = new System.Windows.Media.FontFamily("Portable User Interface");
            _TextSize = 14;
            _TextColor = new SolidColorBrush(Colors.White);
            _TextAreaWidth = 120;
            Content = LayoutRoot;
            LayoutRoot.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            LayoutRoot.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            LayoutRoot.Background = Background;

            spMenu = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.Transparent) };
            spMenu.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            spMenu.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            LayoutRoot.Children.Add(spMenu);

            tbTooptip = new TextBlock()
            {
                Width = _TextAreaWidth,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                IsHitTestVisible = false,
                TextWrapping = TextWrapping.Wrap,
                TextTrimming = TextTrimming.WordEllipsis,
                FontSize = _TextSize,
                Foreground = _TextColor
            };
            spMenu.Children.Add(tbTooptip);
            tbTooptip.Text = "Tooltip";
        }
        TextBlock tbTooptip;

        public TextBlock TbTooptip
        {
            get { return tbTooptip; }
            private set { }
        }
        List<EffectLibrary.GlowWinSeven> lstGS = new List<EffectLibrary.GlowWinSeven>();
        List<BasicLibrary.EffectableControl> lstEff = new List<BasicLibrary.EffectableControl>();
        public override void ClearMenu()
        {
            spMenu.Children.Clear();
            spMenu.Children.Add(tbTooptip);
            tbTooptip.Text = "Tooltip";
        }
        public override void AddMenuItem(BasicLibrary.Menu.IMenuItem item)
        {
            BasicLibrary.EffectableControl eff = new BasicLibrary.EffectableControl(item as SevenMenuItem);
            eff.Width = _ItemWidth;
            eff.Height = _ItemHeight;
            lstGS.Add(new EffectLibrary.GlowWinSeven(eff) { TransitionAlpha = _TransitionAlpha, TransitionColor = _TransitionColor });
            spMenu.Children.Add(eff);
            lstEff.Add(eff);
        }

        public override BasicLibrary.Menu.IMenuItem CreateMenuItem(System.Xml.Linq.XElement e)
        {
            SevenMenuItem mmi = new SevenMenuItem(this);
            mmi.ItemSelected += new SevenMenuItem.ItemSelectedHandler(mmi_ItemSelected);
            System.Xml.Linq.XAttribute att;
            att = e.Attribute("text");
            if (att != null)
                mmi.Title = att.Value;
            att = e.Attribute("icon");
            if (att != null)
                mmi.ImageURL = att.Value;
            att = e.Attribute("url");
            if (att != null)
                mmi.URL = att.Value;
            return mmi;
        }

        void mmi_ItemSelected(object sender, string url)
        {
            OnLinkClicked(url);
        }

        public override BasicLibrary.Menu.ISubMenu CreateSubMenu()
        {
            SevenSubMenu msm = new SevenSubMenu();
            return msm;
        }

        public override BasicLibrary.Menu.ISubMenuItem CreateSubMenuItem(System.Xml.Linq.XElement e)
        {
            SevenSubMenuItem msmi = new SevenSubMenuItem();
            msmi.ItemSelected += new SevenSubMenuItem.ItemSelectedHandler(msmi_ItemSelected);
            System.Xml.Linq.XAttribute att;
            att = e.Attribute("text");
            if (att != null)
                msmi.Item.Text = att.Value;
            att = e.Attribute("icon");
            if (att != null)
                msmi.Item.Icon = att.Value;
            att = e.Attribute("url");
            if (att != null)
                msmi.URL = att.Value;
            return msmi;
        }

        void msmi_ItemSelected(object sender, string url)
        {
            OnLinkClicked(url);
        }
    }
}
