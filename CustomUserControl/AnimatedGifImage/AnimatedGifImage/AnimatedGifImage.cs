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

namespace ControlLibrary
{
    public class AnimatedGifImage : ImageTools.Controls.AnimatedImage
    {
        public string ImageURL
        {
            get { return img.UriSource.OriginalString; }
            set 
            {
                img = new ImageTools.Image();
                img.UriSource = new Uri(value, UriKind.RelativeOrAbsolute);

                img.LoadingCompleted += new EventHandler(img_LoadingCompleted);
                img.LoadingFailed += new EventHandler<UnhandledExceptionEventArgs>(img_LoadingFailed);
                Source = img;                
            }
        }

        ImageTools.Image img;
        public AnimatedGifImage()
        {
            ImageTools.IO.Decoders.AddDecoder<ImageTools.IO.Gif.GifDecoder>();
        }

        void img_LoadingFailed(object sender, UnhandledExceptionEventArgs e)
        {
            img.LoadingCompleted -= new EventHandler(img_LoadingCompleted);
            img.LoadingFailed -= new EventHandler<UnhandledExceptionEventArgs>(img_LoadingFailed);
        }

        void img_LoadingCompleted(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(UpdateLayout);
            img.LoadingCompleted -= new EventHandler(img_LoadingCompleted);
            img.LoadingFailed -= new EventHandler<UnhandledExceptionEventArgs>(img_LoadingFailed);
        }
    }
}
