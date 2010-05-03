// Author: Page Brooks
// Website: http://www.pagebrooks.com
// RSS Feed: http://feeds.pagebrooks.com/pagebrooks

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Controls
{
    public partial class ColorPicker : UserControl
    {
        ColorSpace m_colorSpace;
        bool m_sliderMouseDown;
        // bool m_isMouseCaptured;
        bool m_sampleMouseDown;
        bool m_ManualSet;
        float m_selectedHue;
        int m_sampleX;
        int m_sampleY;
        byte alpha;
        //private Color m_selectedColor;

        public static readonly DependencyProperty SelectedColorProperty =
          DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), null);
        public Color SelectedColor
        {
            get { return (Color)base.GetValue(SelectedColorProperty); }
            set
            {
                base.SetValue(SelectedColorProperty, value);
                HexValue.Text = m_colorSpace.GetHexCode(value);
                alpha = value.A;
                numericUpDownRed.Value = value.R;
                numericUpDownGreen.Value = value.G;
                numericUpDownBlue.Value = value.B;
                if (ColorSelected != null)
                    ColorSelected(value);
                if (m_ManualSet == true)
                {
                    return;
                }
                
                float h, s, v;
                h = s = v = 0;

                m_colorSpace.ConvertRgbToHSV(value, ref h, ref s, ref v);
                SelectedColorRect.Fill = new SolidColorBrush(value);
                m_selectedHue = h;

                int hue_yPos;
                hue_yPos = (int)(m_selectedHue * rectHueMonitor.Height) / 360;
                HueSelector.SetValue(Canvas.TopProperty, hue_yPos - HueSelector.Height / 2);
                int huePos = (int)(hue_yPos / rectHueMonitor.Height * 255);

                int gradientStops = 6;
                Color c = m_colorSpace.GetColorFromPosition(huePos * gradientStops);
                rectSample.Fill = new SolidColorBrush(c);

                m_sampleX = (int)(s / 100 * rectSample.Width);
                m_sampleY = (int)((1 - v / 100) * rectSample.Height);

                SampleSelector.SetValue(Canvas.LeftProperty, m_sampleX - (SampleSelector.Height / 2));
                SampleSelector.SetValue(Canvas.TopProperty, m_sampleY - (SampleSelector.Height / 2));

                HexValue.Text = m_colorSpace.GetHexCode(value);                                   
            }
        }

        public delegate void ColorSelectedHandler(Color c);
        public event ColorSelectedHandler ColorSelected;

        public ColorPicker()
        {
            InitializeComponent();
            rectHueMonitor.MouseLeftButtonDown += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonDown);
            rectHueMonitor.MouseLeftButtonUp += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonUp);
            rectHueMonitor.MouseLeave += new MouseEventHandler(rectHueMonitor_MouseLeave);
            rectHueMonitor.MouseMove += new MouseEventHandler(rectHueMonitor_MouseMove);

            rectSampleMonitor.MouseLeftButtonDown += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonDown);
            rectSampleMonitor.MouseLeftButtonUp += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonUp);
            rectSampleMonitor.MouseLeave += new MouseEventHandler(rectSampleMonitor_MouseLeave);
            rectSampleMonitor.MouseMove += new MouseEventHandler(rectSampleMonitor_MouseMove);
            
            m_colorSpace = new ColorSpace();
            m_ManualSet = false;
            SelectedColor = Colors.Black;
            numericUpDownRed.Focus();
        }

        void rectHueMonitor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            m_sliderMouseDown = true;
            int yPos = (int)e.GetPosition((UIElement)sender).Y;
            UpdateSelection(yPos);
            //CaptureMouse();
        }

        void rectHueMonitor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Mouse Up");
            e.Handled = true;
            m_sliderMouseDown = false;
            //ReleaseMouseCapture();
        }

        void rectHueMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_sliderMouseDown)
            {
                int yPos = (int)e.GetPosition((UIElement)sender).Y;
                UpdateSelection(yPos);
            }
        }

        void rectHueMonitor_MouseLeave(object sender, EventArgs e)
        {
            m_sliderMouseDown = false;
        }

        void rectSampleMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            m_sampleMouseDown = true;
            Point pos = e.GetPosition((UIElement)sender);
            m_sampleX = (int)pos.X;
            m_sampleY = (int)pos.Y;
            UpdateSample(m_sampleX, m_sampleY);
        }

        void rectSampleMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            m_sampleMouseDown = false;
        }

        void rectSampleMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_sampleMouseDown)
            {
                Point pos = e.GetPosition((UIElement)sender);
                m_sampleX = (int)pos.X;
                m_sampleY = (int)pos.Y;
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

            m_ManualSet = true;
            Color c = m_colorSpace.ConvertHsvToRgb((float)m_selectedHue, xComponent, yComponent);
            c.A = alpha;
            SelectedColor = c;
            m_ManualSet = false;
            SelectedColorRect.Fill = new SolidColorBrush(SelectedColor);            
            
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
        
        private void numericUpDownRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SelectedColor = Color.FromArgb(alpha, (byte)numericUpDownRed.Value, (byte)numericUpDownGreen.Value, (byte)numericUpDownBlue.Value);
        }

        private void numericUpDownGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SelectedColor = Color.FromArgb(alpha, (byte)numericUpDownRed.Value, (byte)numericUpDownGreen.Value, (byte)numericUpDownBlue.Value);
        }

        private void numericUpDownBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SelectedColor = Color.FromArgb(alpha, (byte)numericUpDownRed.Value, (byte)numericUpDownGreen.Value, (byte)numericUpDownBlue.Value);
        }

        private void HexValue_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void HexValue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                bool error;
                Color c = m_colorSpace.FromString(HexValue.Text, out error);
                if (error)
                    MessageBox.Show("Invalid color: " + HexValue.Text);
                SelectedColor = c;
            }            
        }
       
    }
}
