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
using System.Windows.Media.Imaging;

namespace EffectLibrary
{
    public class Checkerboard : BasicEffect
    {
        private const int MIN = 20;
        #region attributes
        private TimeSpan cellDuration;
        private Storyboard sb;
        double width, height, cellWidth, cellHeight;
        Rectangle[][] cells = new Rectangle[0][];
        Rectangle[][] blackCells = new Rectangle[0][];
        #endregion attributes

        #region properties
        #endregion properties

        public Checkerboard(EffectableControl control)
            : base(control)
        {
            width = control.Width;
            height = control.Height;
            if (((double.IsNaN(width) && double.IsNaN(height)) || (width == 0 && height == 0)) && !double.IsNaN(control.ActualWidth) && !double.IsNaN(control.ActualHeight))
            {
                width = control.ActualWidth;
                height = control.ActualHeight;
            }

            cellDuration = TimeSpan.FromMilliseconds(600);
            InitStoryboard();

            control.SizeChanged += new SizeChangedEventHandler(control_SizeChanged);
        }

        void control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;
            InitStoryboard();
        }

        private void InitStoryboard()
        {
            for (int i = 0; i < cells.Length; i++)
                for (int j = 0; j < cells[i].Length; j++)
                    control.CanvasRoot.Children.Remove(cells[i][j]);

            int col = CalculateNum(width);
            int row = CalculateNum(height);
            cellWidth = width / col;
            cellHeight = height / row;

            Random random = new Random();
            int max = (int)(cellDuration.TotalMilliseconds * 3);
            sb = new Storyboard();
            double x, y;
            x = y = 0;

            cells = new Rectangle[col][];
            blackCells = new Rectangle[col][];
            for (int i = 0; i < col; i++)
            {
                cells[i] = new Rectangle[row];
                blackCells[i] = new Rectangle[row];
                y = 0;
                for (int j = 0; j < row; j++)
                {
                    blackCells[i][j] = new Rectangle();
                    blackCells[i][j].Width = cellWidth;
                    blackCells[i][j].Height = cellHeight;
                    blackCells[i][j].Fill = new SolidColorBrush(Colors.Black);
                    blackCells[i][j].Visibility = Visibility.Visible;
                    Canvas.SetLeft(blackCells[i][j], x);
                    Canvas.SetTop(blackCells[i][j], y);
                    control.CanvasRoot.Children.Add(blackCells[i][j]);

                    cells[i][j] = new Rectangle();
                    cells[i][j].Width = cellWidth;
                    cells[i][j].Height = cellHeight;
                    PlaneProjection pp = new PlaneProjection() { CenterOfRotationY = 0.5, RotationY = 90 };
                    cells[i][j].Projection = pp;
                    Canvas.SetLeft(cells[i][j], x);
                    Canvas.SetTop(cells[i][j], y);
                    control.CanvasRoot.Children.Add(cells[i][j]);

                    TimeSpan ts = TimeSpan.FromMilliseconds(random.Next(10, max));
                    DoubleAnimation doubleAnimation1 = new DoubleAnimation() { BeginTime = ts, Duration = cellDuration, From = 90, To = 0 };
                    Storyboard.SetTarget(doubleAnimation1, pp);
                    Storyboard.SetTargetProperty(doubleAnimation1, new PropertyPath("RotationY"));
                    sb.Children.Add(doubleAnimation1);

                    TimeSpan ts1 = ts.Add(cellDuration);
                    ObjectAnimationUsingKeyFrames oaufk1 = new ObjectAnimationUsingKeyFrames() { BeginTime = ts1 };
                    DiscreteObjectKeyFrame dokf1 = new DiscreteObjectKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)), Value = Visibility.Collapsed };
                    oaufk1.KeyFrames.Add(dokf1);
                    Storyboard.SetTarget(oaufk1, cells[i][j]);
                    Storyboard.SetTargetProperty(oaufk1, new PropertyPath("(Rectangle.Visibility)"));
                    sb.Children.Add(oaufk1);

                    ObjectAnimationUsingKeyFrames oaufk2 = new ObjectAnimationUsingKeyFrames() { BeginTime = ts1 };
                    DiscreteObjectKeyFrame dokf2 = new DiscreteObjectKeyFrame() { KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)), Value = Visibility.Collapsed };
                    oaufk2.KeyFrames.Add(dokf2);
                    Storyboard.SetTarget(oaufk2, blackCells[i][j]);
                    Storyboard.SetTargetProperty(oaufk2, new PropertyPath("(Rectangle.Visibility)"));
                    sb.Children.Add(oaufk2);

                    y += cellHeight;
                }
                x += cellWidth;
            }
        }

        private int CalculateNum(double value)
        {
            if (value < MIN)
                return 1;

            double size = MIN;
            int temp = (int)(value / size);

            while (temp > 10)
            {
                size += MIN;
                temp = (int)(value / size);
            }

            return temp;
        }

        void sb_Completed(object sender, EventArgs e)
        {
        }

        #region override methods
        public override void Start()
        {
            double x, y;
            x = y = 0;
            for (int i = 0; i < cells.Length; i++)
            {
                y = 0;
                for (int j = 0; j < cells[i].Length; j++)
                {
                    WriteableBitmap bitmap = new WriteableBitmap(control.Control, null);
                    Canvas.SetLeft(cells[i][j], x);
                    Canvas.SetTop(cells[i][j], y);
                    cells[i][j].Width = cellWidth;
                    cells[i][j].Height = cellHeight;

                    TranslateTransform rt = new TranslateTransform();
                    rt.X = -x;
                    rt.Y = -y;
                    cells[i][j].Fill = new ImageBrush() { ImageSource = bitmap, AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top, Transform = rt, Stretch = Stretch.None };

                    blackCells[i][j].Visibility = Visibility.Visible;
                    cells[i][j].Visibility = Visibility.Visible;

                    ((PlaneProjection)cells[i][j].Projection).RotationY = 90;
                    y += cellHeight;
                }
                x += cellWidth;
            }
            sb.Begin();
        }

        public override void Stop()
        {
            sb.Stop();
        }

        public override void DetachEffect()
        {
            for (int i = 0; i < cells.Length; i++)
                for (int j = 0; j < cells[i].Length; j++)
                {
                    control.CanvasRoot.Children.Remove(cells[i][j]);
                    control.CanvasRoot.Children.Remove(blackCells[i][j]);
                }
        }

        protected override void SetSelfHandle()
        {
        }
        #endregion override methods
    }
}
