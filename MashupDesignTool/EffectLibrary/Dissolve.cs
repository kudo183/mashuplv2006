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

namespace EffectLibrary
{
    public class Dissolve : BasicEffect
    {
        private const int MIN = 20;
        private const int MAX_NUM_CELLS = 10;
        #region attributes
        private TimeSpan cellDuration = TimeSpan.FromMilliseconds(600);
        private Color cellColor = Colors.Black;
        private Storyboard sb;
        double width, height, cellWidth, cellHeight;
        Rectangle[][] cells = new Rectangle[0][];
        #endregion attributes

        #region properties
        public Color CellColor
        {
            get { return cellColor; }
            set
            {
                cellColor = value;
                for (int i = 0; i < cells.Length; i++)
                    for (int j = 0; j < cells[i].Length; j++)
                        cells[i][j].Fill = new SolidColorBrush(cellColor);
            }
        }

        public TimeSpan CellDuration
        {
            get { return cellDuration; }
            set
            {
                cellDuration = value;
                InitStoryboard();
            }
        }
        #endregion properties

        public Dissolve(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("CellColor");
            parameterNameList.Add("CellDuration");

            width = control.Width;
            height = control.Height;
            if (((double.IsNaN(width) && double.IsNaN(height)) || (width == 0 && height == 0)) && !double.IsNaN(control.ActualWidth) && !double.IsNaN(control.ActualHeight))
            {
                width = control.ActualWidth;
                height = control.ActualHeight;
            }

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
            int max = (int)(cellDuration.TotalMilliseconds * 0.9);
            sb = new Storyboard();
            double x, y;
            x = y = 0;
            cells = new Rectangle[col][];
            for (int i = 0; i < col; i++)
            {
                cells[i] = new Rectangle[row];
                y = 0;
                for (int j = 0; j < row; j++)
                {
                    cells[i][j] = new Rectangle();
                    cells[i][j].Fill = new SolidColorBrush(cellColor);
                    cells[i][j].Width = 0;
                    cells[i][j].Height = 0;
                    Canvas.SetLeft(cells[i][j], x);
                    Canvas.SetTop(cells[i][j], y);
                    control.CanvasRoot.Children.Add(cells[i][j]);

                    TimeSpan ts = TimeSpan.FromMilliseconds(random.Next(0, max));
                    DoubleAnimation doubleAnimation1 = new DoubleAnimation() { BeginTime = ts, Duration = cellDuration, From = cellWidth, To = 0 };
                    Storyboard.SetTarget(doubleAnimation1, cells[i][j]);
                    Storyboard.SetTargetProperty(doubleAnimation1, new PropertyPath("Width"));
                    sb.Children.Add(doubleAnimation1);

                    DoubleAnimation doubleAnimation2 = new DoubleAnimation() { BeginTime = ts, Duration = cellDuration, From = cellHeight, To = 0 };
                    Storyboard.SetTarget(doubleAnimation2, cells[i][j]);
                    Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath("Height"));
                    sb.Children.Add(doubleAnimation2);

                    DoubleAnimation doubleAnimation3 = new DoubleAnimation() { BeginTime = ts, Duration = cellDuration, From = x, To = x + cellWidth / 2 };
                    Storyboard.SetTarget(doubleAnimation3, cells[i][j]);
                    Storyboard.SetTargetProperty(doubleAnimation3, new PropertyPath("(Canvas.Left)"));
                    sb.Children.Add(doubleAnimation3);

                    DoubleAnimation doubleAnimation4 = new DoubleAnimation() { BeginTime = ts, Duration = cellDuration, From = y, To = y + cellHeight / 2 };
                    Storyboard.SetTarget(doubleAnimation4, cells[i][j]);
                    Storyboard.SetTargetProperty(doubleAnimation4, new PropertyPath("(Canvas.Top)"));
                    sb.Children.Add(doubleAnimation4);

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

            while (temp > MAX_NUM_CELLS)
            {
                size += MIN;
                temp = (int)(value / size);
            }

            return temp;
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
                    Canvas.SetLeft(cells[i][j], x);
                    Canvas.SetTop(cells[i][j], y);
                    cells[i][j].Width = cellWidth + 1;
                    cells[i][j].Height = cellHeight + 1;
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
                    control.CanvasRoot.Children.Remove(cells[i][j]);
        }

        protected override void SetSelfHandle()
        {
        }
        #endregion override methods
    }
}
