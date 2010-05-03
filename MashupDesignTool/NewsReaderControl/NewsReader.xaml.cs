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
using DockCanvas;
using System.Windows.Threading;
using MoveAndScaleEffect;

namespace NewsReaderControl
{
    public partial class NewsReader : UserControl
    {
        const double SCALE_RATIO = 0.3;

        NewsPage currentPage = null;
        List<string> listURL = new List<string>();
        List<NewsPage> listMovePage = new List<NewsPage>();
        List<MoveAndScaleEffect.Effect> effect = new List<MoveAndScaleEffect.Effect>();
        DispatcherTimer timer;
        int selectedIndex;

        public NewsReader()
        {
            InitializeComponent();

            //nonePage.SetPageContent(@"<h1 align='center'>News Reader<h1>"); 

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 200);
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void ChangePage()
        {
            if (listPages.SelectedIndex == -1)
                return;

            double x = Canvas.GetLeft((UIElement)listPages.SelectedItem);
            double y = Canvas.GetTop((UIElement)listPages.SelectedItem);
            Canvas.SetLeft(listMovePage[listPages.SelectedIndex], x);
            Canvas.SetTop(listMovePage[listPages.SelectedIndex], y);
            ScaleSmall(listMovePage[listPages.SelectedIndex]);
            for (int i = 0; i < effect.Count; i++)
                listMovePage[i].Visibility = System.Windows.Visibility.Collapsed;
            listMovePage[listPages.SelectedIndex].Visibility = System.Windows.Visibility.Visible;
            effect[listPages.SelectedIndex].Start();
        }

        private void ScaleSmall(NewsPage page)
        {
            ScaleTransform transform = null;
            if (page.RenderTransform == null)
            {
                transform = new ScaleTransform();
                page.RenderTransform = transform;
            }
            else if (page.RenderTransform is TransformGroup)
            {
                TransformGroup tg = (TransformGroup)page.RenderTransform;
                foreach (Transform t in tg.Children)
                {
                    if (t is ScaleTransform)
                    {
                        transform = (ScaleTransform)t;
                        break;
                    }
                }
                if (transform == null)
                {
                    transform = new ScaleTransform();
                    tg.Children.Add(transform);
                }
            }
            else if (page.RenderTransform is ScaleTransform)
            {
                transform = (ScaleTransform)page.RenderTransform;
            }
            else
            {
                TransformGroup tg = new TransformGroup();
                Transform t = page.RenderTransform;
                tg.Children.Add(t);
                transform = new ScaleTransform();
                tg.Children.Add(transform);
                page.RenderTransform = tg;
            }
            transform.ScaleX = SCALE_RATIO;
            transform.ScaleY = SCALE_RATIO;
        }

        public void AddPages(string url)
        {
            if (listURL.Contains(url))
            {
                int i = 0;
                for (; i < listURL.Count; i++)
                {
                    if (listURL[i] == url)
                        break;
                }
                listPages.SelectedIndex = i;
                //ChangePage();
                return;
            }

            NewsPage page = CreateNewPage(url);
            listPages.Items.Add(page);
            listPages.SelectedIndex = listPages.Items.Count - 1;

            page = CreateNewPage(url);
            page.Visibility = System.Windows.Visibility.Collapsed;
            LayoutRoot.Children.Add(page);
            listMovePage.Add(page);
            listURL.Add(url);
            effect.Add(CreateEffect(page));

            //ChangePage();
        }

        private NewsPage CreateNewPage(string url)
        {
            NewsPage page = new NewsPage();
            page.PageURL = url;
            page.Width = nonePage.Width;
            page.Height = nonePage.Height; 
            TransformGroup group = new TransformGroup();
            ScaleTransform scale = new ScaleTransform();
            scale.ScaleX = SCALE_RATIO;
            scale.ScaleY = SCALE_RATIO;
            group.Children.Add(scale);
            page.RenderTransform = scale;
            return page;
        }

        private MoveAndScaleEffect.Effect CreateEffect(NewsPage page)
        {
            Point begin = new Point(Canvas.GetLeft(page), Canvas.GetTop(page));
            Point end = new Point(Canvas.GetLeft(nonePage), Canvas.GetTop(nonePage));
            Point scaleFrom = new Point(0.4, 0.4);
            Point scaleTo = new Point(1, 1);
            return new MoveAndScaleEffect.Effect(page, begin, end, scaleFrom, scaleTo, MoveAndScaleEffect.Effect.MoveAndScaleEffectSpeed.NORMAL);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width, height;
            width = height = 0;
            if (!double.IsNaN(this.ActualWidth) && this.ActualWidth != 0 && this.ActualHeight != 0)
            {
                width = this.ActualWidth;
                height = this.ActualHeight;
            }
            else if (double.IsNaN(this.Height))
            {
                width = this.Width;
                height = this.Height;
            }

            if (width != 0 && height != 0)
            {
                LayoutRoot.Width = width;
                LayoutRoot.Height = height;

                listPages.Width = width * SCALE_RATIO + 20;
                listPages.Height = height;

                Canvas.SetLeft(nonePage, listPages.Width);
                nonePage.Width = width - listPages.Width;
                nonePage.Height = height;

                if (currentPage == null)
                    return;

                Canvas.SetLeft(currentPage, listPages.Width);
                currentPage.Width = width - listPages.Width;
                currentPage.Height = height;

                effect.Clear();
                foreach (NewsPage page in listMovePage)
                    effect.Add(CreateEffect(page));
            }
        }

        private void listPages_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                if (selectedIndex == listPages.SelectedIndex && selectedIndex != -1)
                    ChangePage();
            }
            else
            {
                timer.Start();
                selectedIndex = listPages.SelectedIndex;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
        }
    }
}
