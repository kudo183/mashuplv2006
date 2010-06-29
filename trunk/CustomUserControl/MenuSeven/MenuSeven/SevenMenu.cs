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
using Liquid;
namespace MyMenu
{
    public class SevenMenu : BasicLibrary.Menu.BasicMenu
    {
        Grid LayoutRoot = new Grid() { Background = new SolidColorBrush(Colors.White) };
        StackPanel spMenu;
        Image img;
        ImageSource _backgroundImage;

        public ImageSource BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                _backgroundImage = value;
                img.Source = _backgroundImage;
            }
        }


        public SevenMenu()
            : base()
        {
            parameterNameList.Add("BackgroundImage");
            Content = LayoutRoot;
            LayoutRoot.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            LayoutRoot.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            img = new Image() { Stretch = Stretch.Fill };
            LayoutRoot.Children.Add(img);

            spMenu = new StackPanel() { Orientation = Orientation.Horizontal, Background = new SolidColorBrush(Colors.Transparent) };
            spMenu.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            spMenu.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            LayoutRoot.Children.Add(spMenu);
        }

        public override void ClearMenu()
        {
            spMenu.Children.Clear();
        }
        public override void AddMenuItem(BasicLibrary.Menu.IMenuItem item)
        {
            BasicLibrary.EffectableControl eff = new BasicLibrary.EffectableControl(item as SevenMenuItem);
            eff.Width = 70;
            eff.Height = 50;
            eff.ChangeEffect("MainEffect", typeof(EffectLibrary.GlowWinSeven));
            spMenu.Children.Add(eff);
        }

        public override BasicLibrary.Menu.IMenuItem CreateMenuItem(System.Xml.Linq.XElement e)
        {
            SevenMenuItem mmi = new SevenMenuItem(this);
            System.Xml.Linq.XAttribute att;
            att = e.Attribute("Text");
            mmi.Title = att.Value;
            att = e.Attribute("Icon");
            mmi.ImageURL = att.Value;
            return mmi;
        }

        public override BasicLibrary.Menu.ISubMenu CreateSubMenu()
        {
            SevenSubMenu msm = new SevenSubMenu();
            return msm;
        }

        public override BasicLibrary.Menu.ISubMenuItem CreateSubMenuItem(System.Xml.Linq.XElement e)
        {
            SevenSubMenuItem msmi = new SevenSubMenuItem();
            System.Xml.Linq.XAttribute att;
            att = e.Attribute("Text");
            msmi.Item.Text = att.Value;
            att = e.Attribute("Icon");
            msmi.Item.Icon = att.Value;
            return msmi;
        }
    }
}
