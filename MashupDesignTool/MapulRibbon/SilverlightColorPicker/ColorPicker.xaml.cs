using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SilverlightColorPicker
{
    public partial class ColorPicker : UserControl
    {
        ColorSpace m_colorSpace;
        bool m_sliderMouseDown;
       // bool m_isMouseCaptured;
        bool m_sampleMouseDown;
        float m_selectedHue;
        int m_sampleX;
        int m_sampleY;
        private Color m_selectedColor;
        public delegate void ColorSelectedHandler(Color c);
        public event ColorSelectedHandler ColorSelected;

        private static int INTERVAL = 300;          // Double Click Detect Interval
        DispatcherTimer _timer;                     // Detect Double Click

        public ColorPicker()
        {
            InitializeComponent();

            // create a timer to detect double click
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, INTERVAL);
            _timer.Tick += new EventHandler(_timer_Tick);

            rectHueMonitor.MouseLeftButtonDown += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonDown);
            rectHueMonitor.MouseLeftButtonUp += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonUp);
            rectHueMonitor.MouseLeave += new MouseEventHandler(rectHueMonitor_MouseLeave);
            rectHueMonitor.MouseMove += new MouseEventHandler(rectHueMonitor_MouseMove);

            rectSampleMonitor.MouseLeftButtonDown += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonDown);
            rectSampleMonitor.MouseLeftButtonUp += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonUp);
            rectSampleMonitor.MouseLeave += new MouseEventHandler(rectSampleMonitor_MouseLeave);
            rectSampleMonitor.MouseMove += new MouseEventHandler(rectSampleMonitor_MouseMove);

            m_colorSpace = new ColorSpace();
            m_selectedHue = 0;
            m_sampleX = (int)rectSampleMonitor.Width;
            m_sampleY = 0;
            UpdateSample(m_sampleX, m_sampleY);   
        }

        void rectHueMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            //e.Handled = true;            
            m_sliderMouseDown = true;
            //int yPos = (int)e.GetPosition((UIElement)sender).Y;                        
            //int yPos = (int)e.GetPosition(((this.Parent as FrameworkElement).Parent as StackPanel).Parent as FrameworkElement).Y - 60;            
            FrameworkElement obj = ((((this.Parent as FrameworkElement).Parent as Border).Parent as FrameworkElement).Parent as StackPanel).Parent as FrameworkElement;
            int yPos = (int)e.GetPosition(obj).Y - 60;

            UpdateSelection(yPos);
           // m_isMouseCaptured = CaptureMouse();
        }

        void rectHueMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Mouse Up");
            //e.Handled = true;
            m_sliderMouseDown = false;
         //   ReleaseMouseCapture();
        }

        void rectHueMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_sliderMouseDown)
            {
                //int yPos = (int)e.GetPosition((UIElement)sender).Y;
                //int yPos = (int)e.GetPosition(((this.Parent as FrameworkElement).Parent as StackPanel).Parent as FrameworkElement).Y - 60;
                FrameworkElement obj = ((((this.Parent as FrameworkElement).Parent as Border).Parent as FrameworkElement).Parent as StackPanel).Parent as FrameworkElement;
                int yPos = (int)e.GetPosition(obj).Y - 60;
                UpdateSelection(yPos);
            }
        }

        void rectHueMonitor_MouseLeave(object sender, EventArgs e)
        {
            m_sliderMouseDown = false;
        }

        // stop the timer after an interval
        void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
        }


        void rectSampleMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            //
            m_sampleMouseDown = true;
            //Point pos = e.GetPosition((UIElement)sender);
            //Point pos = e.GetPosition(((this.Parent as FrameworkElement).Parent as StackPanel).Parent as FrameworkElement);
            FrameworkElement obj = ((((this.Parent as FrameworkElement).Parent as Border).Parent as FrameworkElement).Parent as StackPanel).Parent as FrameworkElement;            
            Point pos = e.GetPosition(obj);
            m_sampleX = (int)pos.X - 18;
            m_sampleY = (int)pos.Y + 3 - 60;
            UpdateSample(m_sampleX, m_sampleY);
            //

            // if second click comes before the timer end, it's an double click
            if (_timer.IsEnabled)
            {
                // stop the timer
                _timer.Stop();                
                //
                m_sampleMouseDown = false;
                //
                if (ColorSelected != null)
                    ColorSelected(m_selectedColor);
            }
            else
            {
                // if not double click, start the timer
                _timer.Start();
            }            
        }

        void rectSampleMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            m_sampleMouseDown = false;
        }

        void rectSampleMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_sampleMouseDown)
            {
                //Point pos = e.GetPosition((UIElement)sender);
                //Point pos = e.GetPosition(((this.Parent as FrameworkElement).Parent as StackPanel).Parent as FrameworkElement);
                FrameworkElement obj = ((((this.Parent as FrameworkElement).Parent as Border).Parent as FrameworkElement).Parent as StackPanel).Parent as FrameworkElement;
                Point pos = e.GetPosition(obj);
                m_sampleX = (int)pos.X - 18;
                m_sampleY = (int)pos.Y + 3 - 60;
                UpdateSample(m_sampleX, m_sampleY);
            }
        }

        void rectSampleMonitor_MouseLeave(object sender, EventArgs e)
        {
            m_sampleMouseDown = false;
        }

        private void UpdateSample(int xPos, int yPos)
        {

            SampleSelector.SetValue(Canvas.LeftProperty, xPos - (SampleSelector.Height / 2));
            SampleSelector.SetValue(Canvas.TopProperty, yPos - (SampleSelector.Height / 2));

            float yComponent = 1 - (float)(yPos / rectSample.Height);
            float xComponent = (float)(xPos / rectSample.Width);

            m_selectedColor = m_colorSpace.ConvertHsvToRgb((float)m_selectedHue, xComponent, yComponent);
            SelectedColor.Fill = new SolidColorBrush(m_selectedColor);
            HexValue.Text = m_colorSpace.GetHexCode(m_selectedColor);

            //if (ColorSelected != null)
            //    ColorSelected(m_selectedColor);
        }

        private void UpdateSelection(int yPos)
        {
            int huePos = (int)(yPos / rectHueMonitor.Height * 255);
            int gradientStops = 6;
            Color c = m_colorSpace.GetColorFromPosition(huePos * gradientStops);
            rectSample.Fill = new SolidColorBrush(c);
            HueSelector.SetValue(Canvas.TopProperty, yPos - (HueSelector.Height / 2));
            m_selectedHue = (float)(yPos / rectHueMonitor.Height) * 360;
            UpdateSample(m_sampleX, m_sampleY);
        }

        private void SelectedColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ColorSelected != null)
                ColorSelected(m_selectedColor);
        }
    }
}
