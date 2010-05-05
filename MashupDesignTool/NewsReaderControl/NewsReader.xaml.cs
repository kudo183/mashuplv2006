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
using System.Windows.Threading;
using Effect;

namespace NewsReaderControl
{
    public partial class NewsReader : UserControl
    {
        NewsPage frontPage, backPage;

        public NewsReader()
        {
            InitializeComponent();

            frontPage = page1;
            backPage = page2;
        }

        public void NavigatePage(string url)
        {
            backPage.PageURL = url;
            backPage.Visibility = System.Windows.Visibility.Collapsed;
            TurnEffect.AttachEffect(frontPage, 0, 90, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(400), 0.5, TurnEffect.TurnEffectOrientation.VERTICAL);
            Storyboard sb = TurnEffect.GetStoryboard(frontPage);
            if (sb != null)
                sb.Completed += new EventHandler(sb_Completed1);
            TurnEffect.Start(frontPage);
        }

        public void LoadHTML(string html)
        {
            backPage.LoadHTML(html);
            backPage.Visibility = System.Windows.Visibility.Collapsed;
            TurnEffect.AttachEffect(frontPage, 0, 90, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(400), 0.5, TurnEffect.TurnEffectOrientation.VERTICAL);
            Storyboard sb = TurnEffect.GetStoryboard(frontPage);
            if (sb != null)
                sb.Completed += new EventHandler(sb_Completed1);
            TurnEffect.Start(frontPage);
        }

        void sb_Completed1(object sender, EventArgs e)
        {
            Storyboard sb = TurnEffect.GetStoryboard(frontPage);
            if (sb != null)
                sb.Completed -= new EventHandler(sb_Completed1);
            backPage.Visibility = System.Windows.Visibility.Visible;
            frontPage.Visibility = System.Windows.Visibility.Collapsed;
            TurnEffect.AttachEffect(backPage, -90, 0, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(400), 0.5, TurnEffect.TurnEffectOrientation.VERTICAL);
            TurnEffect.Start(backPage);
            sb = TurnEffect.GetStoryboard(backPage);
            if (sb != null)
                sb.Completed += new EventHandler(sb_Completed2);
        }

        void sb_Completed2(object sender, EventArgs e)
        {
            Storyboard sb = TurnEffect.GetStoryboard(backPage);
            if (sb != null)
                sb.Completed -= new EventHandler(sb_Completed2); 
            Switch();
        }

        private void Switch()
        {
            NewsPage temp = frontPage;
            frontPage = backPage;
            backPage = temp;
            Canvas.SetZIndex(frontPage, 2);
            Canvas.SetZIndex(backPage, 1);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void listPages_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            page1.Width = e.NewSize.Width;
            page1.Height = e.NewSize.Height;
            page2.Width = e.NewSize.Width;
            page2.Height = e.NewSize.Height;
        }

        private void sdMagnification_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (page1 != null)
                MagnifierOverEffect.ChangeMagnification(page1, sdMagnification.Value);
            if (page2 != null)
                MagnifierOverEffect.ChangeMagnification(page2, sdMagnification.Value);
        }

        private void ckbMagnifier_Checked(object sender, RoutedEventArgs e)
        {
            MagnifierOverEffect.AttachEffect(page1, sdMagnification.Value);
            MagnifierOverEffect.AttachEffect(page2, sdMagnification.Value);
        }

        private void ckbMagnifier_Unchecked(object sender, RoutedEventArgs e)
        {
            MagnifierOverEffect.DetachEffect(page1);
            MagnifierOverEffect.DetachEffect(page2);
        }

        public Brush MagnifierChoiseBackground
        {
            get { return spMagnifierChoise.Background; }
            set { spMagnifierChoise.Background = value; }
        }
    }
}
