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
using BasicLibrary;
using System.ServiceModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.ComponentModel;
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

                client.GetDataFromURLAsync(_ImageURL);
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
                if (_IsAnimated == true)
                {
                    _AnimationSpeed = (value == 0) ? 1 : 1000 / value;
                    img = new ImageTools.Image();
                    img.AnimationSpeed = _AnimationSpeed;
                    img.SetSource(new MemoryStream(buffer));
                    img.LoadingCompleted += new EventHandler(img_LoadingCompleted);
                }
            }
        }

        void client_GetDataFromURLCompleted(object sender, ServiceReference1.GetDataFromURLCompletedEventArgs e)
        {
            buffer = e.Result;
            try
            {
                BitmapImage bm = new BitmapImage();
                bm.SetSource(new MemoryStream(e.Result));
                ai.Source = bm;
                _IsAnimated = false;
                img = null;
            }
            catch (Exception ex)
            {
                img = new ImageTools.Image();                
                img.SetSource(new MemoryStream(e.Result));
                img.LoadingCompleted += new EventHandler(img_LoadingCompleted);
            }
        }

        void img_LoadingCompleted(object sender, EventArgs e)
        {
            _IsAnimated = img.IsAnimated;
            Dispatcher.BeginInvoke(SetImageSource);
        }
        private void SetImageSource()
        {
            ai.Source = img;
        }

        ImageTools.Controls.AnimatedImage ai = new ImageTools.Controls.AnimatedImage();
        ImageTools.Image img;
        ServiceReference1.Service1Client client;
        byte[] buffer;

        public AnimatedGifImage()
        {
            parameterNameList.Add("ImageURL");
            parameterNameList.Add("AnimationSpeed");
            parameterNameList.Add("IsAnimated");

            Content = ai;
            Width = Height = 200;
            _AnimationSpeed = 300;
            
            _StretchMode = Stretch.Fill;

            ImageTools.IO.Decoders.AddDecoder<ImageTools.IO.Gif.GifDecoder>();
            ImageTools.IO.Decoders.AddDecoder<ImageTools.IO.Bmp.BmpDecoder>();

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.MaxReceivedMessageSize = 1 << 22;
            EndpointAddress endpointAddress = new EndpointAddress("http://localhost:1728/Service1.svc");

            client = new ServiceReference1.Service1Client(basicHttpBinding, endpointAddress);
            client.GetDataFromURLCompleted += new EventHandler<ServiceReference1.GetDataFromURLCompletedEventArgs>(client_GetDataFromURLCompleted);
        }
    }
}
