using System;
using System.Net;
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
using System.ServiceModel;
using System.IO;
using System.ComponentModel;
using System.Windows.Browser;

namespace ControlLibrary
{
    public class AnimatedGifImage : BasicControl
    {
        private Stretch _StretchMode;

        [Category("AnimatedGifImage")]
        public Stretch StretchMode
        {
            get { return _StretchMode; }
            set 
            { 
                _StretchMode = value;
                ai.Stretch = _StretchMode;
            }
        }
        
        private string _ImageURL;

        [Category("AnimatedGifImage")]
        public string ImageURL
        {
            get { return _ImageURL; }
            set
            {
                string ext = value.Substring(value.LastIndexOf('.') + 1);
                if(ext != "gif" && ext != "bmp" && ext != "jpg" && ext != "png")
                    return;
                _ImageURL = value;
                ultil.OnGetImageAsyncCompleted += new Ultility.GetImageAsyncCompletedHandler(ultil_OnGetImageAsyncCompleted);
                ultil.GetImageAsync(_ImageURL);
            }
        }

        void ultil_OnGetImageAsyncCompleted(byte[] result)
        {
            buffer = result;
            try
            {
                BitmapImage bm = new BitmapImage();
                bm.SetSource(new MemoryStream(buffer));
                ai.Source = bm;
                _IsAnimated = false;
                img = null;
            }
            catch (Exception ex)
            {
                img = new ImageTools.Image();
                img.SetSource(new MemoryStream(buffer));
                img.AnimationSpeed = (_AnimationSpeed == 0) ? 0 : 1000 / _AnimationSpeed;                
                img.LoadingCompleted += new EventHandler(img_LoadingCompleted);
            }
        }

        private bool _IsAnimated;

        [Category("AnimatedGifImage")]
        public bool IsAnimated
        {
            get { return _IsAnimated; }
        }

        private int _AnimationSpeed;
        [Category("AnimatedGifImage")]
        public int AnimationSpeed
        {
            get
            {
                if (_IsAnimated == true)
                    return _AnimationSpeed;
                return 0;
            }
            set
            {
                _AnimationSpeed = value;
                if (_IsAnimated == true)
                {                    
                    img = new ImageTools.Image();
                    img.AnimationSpeed = (_AnimationSpeed == 0) ? 0 : 1000 / _AnimationSpeed;
                    img.SetSource(new MemoryStream(buffer));
                    img.LoadingCompleted += new EventHandler(img_LoadingCompleted);
                }
            }
        }

        void img_LoadingCompleted(object sender, EventArgs e)
        {
            _IsAnimated = img.IsAnimated;
            Dispatcher.BeginInvoke(SetImageSource);
        }
        private void SetImageSource()
        {
            ai.AnimationMode = ImageTools.Controls.AnimationMode.Repeat;
            if(img.AnimationSpeed == 0)
                ai.AnimationMode = ImageTools.Controls.AnimationMode.None;
            ai.Source = img;
        }

        ImageTools.Controls.AnimatedImage ai = new ImageTools.Controls.AnimatedImage();
        ImageTools.Image img;
        byte[] buffer;
        Ultility ultil = new Ultility();
        public AnimatedGifImage()
        {
            parameterNameList.Add("ImageURL");
            parameterNameList.Add("AnimationSpeed");
            
            Content = ai;
            Width = Height = 200;
            _AnimationSpeed = 10;
            
            _StretchMode = Stretch.Fill;

            ImageTools.IO.Decoders.AddDecoder<ImageTools.IO.Gif.GifDecoder>();
            ImageTools.IO.Decoders.AddDecoder<ImageTools.IO.Bmp.BmpDecoder>();            
        }
    }
}
