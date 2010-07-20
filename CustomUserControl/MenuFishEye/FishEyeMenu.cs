using System;
using System.Net;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BasicLibrary;
using ISNet.Silverlight.WebAqua;

namespace MyMenu
{
    public class FishEyeMenu : BasicLibrary.Menu.BasicMenu
    {
        #region property
        #region menu background
        BitmapImage _BackgroundImageLeft;
        [Category("MenuBackground")]
        public BitmapImage BackgroundImageLeft
        {
            get { return _BackgroundImageLeft; }
            set
            {
                _BackgroundImageLeft = value;
                _menu.BackgroundImageLeft = new Image() { Source = value };
            }
        }

        BitmapImage _BackgroundImageCenter;
        [Category("MenuBackground")]
        public BitmapImage BackgroundImageCenter
        {
            get { return _BackgroundImageCenter; }
            set
            {
                _BackgroundImageCenter = value;
                _menu.BackgroundImageCenter = new Image() { Source = value };
            }
        }

        BitmapImage _BackgroundImageRight;
        [Category("MenuBackground")]
        public BitmapImage BackgroundImageRight
        {
            get { return _BackgroundImageRight; }
            set
            {
                _BackgroundImageRight = value;
                _menu.BackgroundImageRight = new Image() { Source = value };
            }
        }
        #endregion

        #region menu effect
        bool _Jump;
        [Category("MenuEffect")]
        public bool Jump
        {
            get { return _Jump; }
            set
            {
                _Jump = value;
                _menu.JumpingEffectEnabled = (value) ? WebFishEyeBooleanValue.Yes : WebFishEyeBooleanValue.No;
            }
        }

        bool _Glow;
        [Category("MenuEffect")]
        public bool Glow
        {
            get { return _Glow; }
            set
            {
                _Glow = value;
                _menu.GlowingEffectEnabled = (value) ? WebFishEyeBooleanValue.Yes : WebFishEyeBooleanValue.No;
            }
        }

        bool _Flip;
        [Category("MenuEffect")]
        public bool Flip
        {
            get { return _Flip; }
            set
            {
                _Flip = value;
                _menu.FlippingEffectEnabled = (value) ? WebFishEyeBooleanValue.Yes : WebFishEyeBooleanValue.No;
            }
        }
        #endregion

        WebFishEyeStackMode _SubMenuMode;
        [Category("Menu")]
        public WebFishEyeStackMode SubMenuMode
        {
            get { return _SubMenuMode; }
            set
            {
                _SubMenuMode = value;
                _menu.StackMode = value;
            }
        }
        #endregion

        WebFishEye _menu;

        public FishEyeMenu()
        {
            parameterNameList.Add("BackgroundImageLeft");
            parameterNameList.Add("BackgroundImageCenter");
            parameterNameList.Add("BackgroundImageRight");
            parameterNameList.Add("Jump");
            parameterNameList.Add("Glow");
            parameterNameList.Add("Flip");
            parameterNameList.Add("SubMenuMode");

            _menu = new WebFishEye();
            _menu.ButtonClick += new WebFishEyeButtonEventHandler(_menu_ButtonClick);
            BackgroundImageCenter = new BitmapImage(new Uri("http://img690.imageshack.us/img690/243/sirius2dockcenter.png", UriKind.RelativeOrAbsolute));
            BackgroundImageLeft = new BitmapImage(new Uri("http://img337.imageshack.us/img337/5193/sirius2dockleft.png", UriKind.RelativeOrAbsolute));
            BackgroundImageRight = new BitmapImage(new Uri("http://img84.imageshack.us/img84/3558/sirius2dockright.png", UriKind.RelativeOrAbsolute));

            SubMenuMode = WebFishEyeStackMode.GridStyle;

            _menu.BackgroundMode = WebFishEyeBackgroundMode.ComplexImages;
            _menu.Width = 400;
            _menu.Height = 120;

            Content = _menu;
            SizeChanged += new SizeChangedEventHandler(FishEyeMenu_SizeChanged);
        }

        void _menu_ButtonClick(object sender, WebFishEyeButtonEventArgs e)
        {
            OnLinkClicked(e.Button.TargetUrl);
        }

        void FishEyeMenu_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _menu.Width = ActualWidth;
            _menu.Height = ActualHeight;
        }

        public override void AddMenuItem(BasicLibrary.Menu.IMenuItem item)
        {
            _menu.Buttons.Add(item as FishEyeMenuItem);
        }

        public override void ClearMenu()
        {
            _menu.Buttons.Clear();
        }

        public override BasicLibrary.Menu.IMenuItem CreateMenuItem(System.Xml.Linq.XElement e)
        {
            FishEyeMenuItem mmi = new FishEyeMenuItem();
            
            System.Xml.Linq.XAttribute att;
            att = e.Attribute("text");
            if (att != null)
                mmi.Text = att.Value;
            att = e.Attribute("icon");
            if (att != null)
                mmi.ImageSource = new BitmapImage(new Uri(att.Value, UriKind.RelativeOrAbsolute));
            att = e.Attribute("url");
            if (att != null)
                mmi.TargetUrl = att.Value;
            return mmi;
        }

        public override BasicLibrary.Menu.ISubMenu CreateSubMenu()
        {
            FishEyeSubMenu ms = new FishEyeSubMenu();
            return ms;
        }

        public override BasicLibrary.Menu.ISubMenuItem CreateSubMenuItem(System.Xml.Linq.XElement e)
        {
            FishEyeSubMenuItem msmi = new FishEyeSubMenuItem();
            System.Xml.Linq.XAttribute att;
            att = e.Attribute("text");
            if (att != null)
                msmi.Text = att.Value;
            att = e.Attribute("icon");
            if (att != null)
                msmi.ImageSource = new BitmapImage(new Uri(att.Value, UriKind.RelativeOrAbsolute));
            att = e.Attribute("url");
            if (att != null)
                msmi.TargetUrl = att.Value;
            return msmi;
        }
    }
}
