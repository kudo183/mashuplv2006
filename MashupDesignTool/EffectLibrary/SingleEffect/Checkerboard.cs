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
    public class Checkerboard : BasicAppearEffect
    {
        public enum CheckDirection
        {
            FROM_LEFT,
            FROM_TOP
        }

        private const int MIN = 20;
        private const int MAX_NUM_CELLS = 8;
        #region attributes
        private CheckDirection direction;
        private TimeSpan cellDuration = TimeSpan.FromMilliseconds(600);
        private TimeSpan beginTime = TimeSpan.FromMilliseconds(0);
        private Color cellColor = Colors.Black;
        private Storyboard sb;
        double width, height, cellWidth, cellHeight;
        Rectangle[][] cells = new Rectangle[0][];
        Rectangle[][] blackCells = new Rectangle[0][];
        double centerOfRotationX, centerOfRotationY, rotationX, rotationY;
        string rotationPath;
        #endregion attributes

        #region properties
        public CheckDirection Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                if (direction == CheckDirection.FROM_LEFT)
                {
                    centerOfRotationX = 0.5;
                    centerOfRotationY = 0;
                    rotationX = 0;
                    rotationY = 90;
                    rotationPath = "RotationY";
                }
                else
                {
                    centerOfRotationX = 0;
                    centerOfRotationY = 0.5;
                    rotationX = 90;
                    rotationY = 0;
                    rotationPath = "RotationX";
                }
                InitStoryboard();
            }
        }

        public Color CellColor
        {
            get { return cellColor; }
            set
            {
                cellColor = value;
                for (int i = 0; i < blackCells.Length; i++)
                    for (int j = 0; j < blackCells[i].Length; j++)
                        blackCells[i][j].Fill = new SolidColorBrush(cellColor);
            }
        }

        public double CellDuration
        {
            get { return cellDuration.TotalMilliseconds; }
            set
            {
                cellDuration = TimeSpan.FromMilliseconds(value);
                InitStoryboard();
            }
        }

        public double BeginTime
        {
            get { return beginTime.TotalMilliseconds; }
            set
            {
                beginTime = TimeSpan.FromMilliseconds(value);
                InitStoryboard();
            }
        }
        #endregion properties

        public Checkerboard(EffectableControl control)
            : base(control)
        {
            parameterNameList.Add("Direction");
            parameterNameList.Add("CellColor");
            parameterNameList.Add("CellDuration");
            parameterNameList.Add("BeginTime");

            width = control.Width;
            height = control.Height;
            if (((double.IsNaN(width) && double.IsNaN(height)) || (width == 0 && height == 0)) && !double.IsNaN(control.ActualWidth) && !double.IsNaN(control.ActualHeight))
            {
                width = control.ActualWidth;
                height = control.ActualHeight;
            }

            direction = CheckDirection.FROM_LEFT;
            centerOfRotationX = 0.5;
            centerOfRotationY = 0;
            rotationX = 0;
            rotationY = 90;
            rotationPath = "RotationY";
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
                    blackCells[i][j].Width = cellWidth + 1;
                    blackCells[i][j].Height = cellHeight + 1;
                    blackCells[i][j].Fill = new SolidColorBrush(cellColor);
                    blackCells[i][j].Visibility = Visibility.Collapsed;
                    Canvas.SetLeft(blackCells[i][j], x);
                    Canvas.SetTop(blackCells[i][j], y);
                    control.CanvasRoot.Children.Add(blackCells[i][j]);

                    cells[i][j] = new Rectangle();
                    cells[i][j].Width = cellWidth + 1;
                    cells[i][j].Height = cellHeight + 1;
                    PlaneProjection pp = new PlaneProjection() { CenterOfRotationX = centerOfRotationX, CenterOfRotationY = centerOfRotationY, RotationX = rotationX, RotationY = rotationY };
                    cells[i][j].Projection = pp;
                    Canvas.SetLeft(cells[i][j], x);
                    Canvas.SetTop(cells[i][j], y);
                    control.CanvasRoot.Children.Add(cells[i][j]);

                    y += cellHeight;
                }
                x += cellWidth;
            }
        }

        #region calculate paramter
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

        private double[][] CalculateBeginTime()
        {
            double[][] begins = new double[cells.Length][];
            for (int i = 0; i < cells.Length; i++)
                begins[i] = new double[cells[i].Length];
            
            int cd = (int)cellDuration.TotalMilliseconds;
            int min1 = 10;
            int max1 = min1 + cd;
            int min2 = 4 * cd / 5;
            int max2 = min2 + cd;
            Random random = new Random();

            if (direction == CheckDirection.FROM_LEFT)
            {
                if (begins.Length > 1)
                {
                    for (int j = 0; j < begins[0].Length; j++)
                    {
                        begins[0][j] = random.Next(min1, max1);
                        for (int i = 2; i < begins.Length; i += 2)
                            begins[i][j] = begins[i - 2][j] + cd;
                    }

                }
                if (begins[1].Length > 2)
                {
                    for (int j = 0; j < begins[1].Length; j++)
                    {
                        begins[1][j] = random.Next(min2, max2);
                        for (int i = 3; i < begins.Length; i += 2)
                            begins[i][j] = begins[i - 2][j] + cd;
                    }
                }
            }
            else
            {
                if (begins.Length > 0)
                {
                    if (begins[0].Length > 1)
                    {
                        for (int i = 0; i < begins.Length; i++)
                        {
                            begins[i][0] = random.Next(min1, max1);
                            for (int j = 2; j < begins[0].Length; j += 2)
                                begins[i][j] = begins[i][j - 2] + cd;
                        }

                    }
                    if (begins[0].Length > 2)
                    {
                        for (int i = 0; i < begins.Length; i++)
                        {
                            begins[i][1] = random.Next(min2, max2);
                            for (int j = 3; j < begins[0].Length; j += 2)
                                begins[i][j] = begins[i][j - 2] + cd;
                        }
                    }
                }
            }

            for (int i = 0; i < begins.Length; i++)
                for (int j = 0; j < begins[i].Length; j++)
                    begins[i][j] += beginTime.TotalMilliseconds;

            return begins;
        }
        #endregion calculate paramter

        #region override methods
        public override void Start()
        {
            sb = new Storyboard();
            double x, y;
            x = y = 0;
            double[][] begins = CalculateBeginTime();

            WriteableBitmap bitmap = new WriteableBitmap(control.Control, null);
            for (int i = 0; i < cells.Length; i++)
            {
                y = 0;
                for (int j = 0; j < cells[i].Length; j++)
                {
                    cells[i][j].Fill = new ImageBrush()
                                        {
                                            ImageSource = bitmap,
                                            AlignmentX = AlignmentX.Left,
                                            AlignmentY = AlignmentY.Top,
                                            Transform = new TranslateTransform() { X = -x, Y = -y },
                                            Stretch = Stretch.None
                                        };

                    blackCells[i][j].Visibility = Visibility.Visible;
                    cells[i][j].Visibility = Visibility.Visible;

                    PlaneProjection pp = (PlaneProjection)cells[i][j].Projection;
                    pp.RotationX = rotationX;
                    pp.RotationY = rotationY;

                    TimeSpan ts = TimeSpan.FromMilliseconds(begins[i][j]);
                    DoubleAnimation doubleAnimation1 = new DoubleAnimation() { BeginTime = ts, Duration = cellDuration, From = 90, To = 0 };
                    Storyboard.SetTarget(doubleAnimation1, pp);
                    Storyboard.SetTargetProperty(doubleAnimation1, new PropertyPath(rotationPath));
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
            sb.Completed += new EventHandler(sb_Completed);
            sb.Begin();
        }

        void sb_Completed(object sender, EventArgs e)
        {
            Storyboard sb = (Storyboard)sender;
            sb.Completed -= new EventHandler(sb_Completed);
            base.RaiseEffectCompleteEvent(this);
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
