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

namespace ControlLibrary
{
    public class AnimatedGifImage : BasicControl
    {
        private string _ImageURL;
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
        public bool IsAnimated
        {
            get { return _IsAnimated; }
        }

        public int AnimationSpeed
        {
            get
            {
                if (_IsAnimated == true)
                    return img.AnimationSpeed;
                return 0;
            }
            set
            {
                if (_IsAnimated == true)
                    img.AnimationSpeed = value;
            }
        }

        void client_GetDataFromURLCompleted(object sender, ServiceReference1.GetDataFromURLCompletedEventArgs e)
        {
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

        public AnimatedGifImage()
        {
            parameterNameList.Add("ImageURL");
            parameterNameList.Add("AnimationSpeed");
            parameterNameList.Add("IsAnimated");

            Content = ai;
            ai.Width = ai.Height = 100;
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
