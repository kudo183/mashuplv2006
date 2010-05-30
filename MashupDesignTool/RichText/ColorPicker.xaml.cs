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

using Liquid;
using System.Windows.Markup;

namespace Liquid
{
    public partial class ColorPicker : UserControl
    {
        #region Private Properties

        private Color _selected = new Color();
        private ColorSelector _selector = null;

        #endregion

        #region Public Properties

        public Dialog ParentDialog { get { return (Dialog)Parent; } }

        public Color Selected
        {
            get { return _selected; }
        }

        public ColorSelector Selector
        {
            get { return _selector; }
        }

        #endregion

        #region Constructor

        public ColorPicker()
        {
            InitializeComponent();

            SetupWebPalette();
            InitcolorPicker();
        }

        #endregion

        #region Public Methods

        public void Build(ColorSelector selector)
        {
            _selector = selector;
            ParentDialog.GetButton("ok").IsEnabled = false;
        }

        public void SetupWebPalette()
        {
            uint[] colors = new uint[] {
                0xFFFF00FF, 0xFFFF33FF, 0xFFFF66FF, 0xFFFF99FF, 0xFFFFCCFF, 0xFFFFFFFF, 0xFFFFFF66, 0xFFFFCC66, 0xFFFF9966, 0xFFFF6666, 0xFFFF3366, 0xFFFF0066,
                0xFFCC00FF, 0xFFCC33FF, 0xFFCC66FF, 0xFFCC99FF, 0xFFCCCCFF, 0xFFCCFFFF, 0xFFCCFF66, 0xFFCCCC66, 0xFFCC9966, 0xFFCC6666, 0xFFCC3366, 0xFFCC0066,
                0xFF9900FF, 0xFF9933FF, 0xFF9966FF, 0xFF9999FF, 0xFF99CCFF, 0xFF99FFFF, 0xFF99FF66, 0xFF99CC66, 0xFF999966, 0xFF996666, 0xFF993366, 0xFF990066,
                0xFF6600FF, 0xFF6633FF, 0xFF6666FF, 0xFF6699FF, 0xFF66CCFF, 0xFF66FFFF, 0xFF66FF66, 0xFF66CC66, 0xFF669966, 0xFF666666, 0xFF663366, 0xFF660066,
                0xFF3300FF, 0xFF3333FF, 0xFF3366FF, 0xFF3399FF, 0xFF33CCFF, 0xFF33FFFF, 0xFF33FF66, 0xFF33CC66, 0xFF339966, 0xFF336666, 0xFF333366, 0xFF330066,
                0xFF0000FF, 0xFF0033FF, 0xFF0066FF, 0xFF0099FF, 0xFF00CCFF, 0xFF00FFFF, 0xFF00FF66, 0xFF00CC66, 0xFF009966, 0xFF006666, 0xFF003366, 0xFF000066,
                0xFF0000CC, 0xFF0033CC, 0xFF0066CC, 0xFF0099CC, 0xFF00CCCC, 0xFF00FFCC, 0xFF00FF33, 0xFF00CC33, 0xFF009933, 0xFF006633, 0xFF003333, 0xFF000033,
                0xFF3300CC, 0xFF3333CC, 0xFF3366CC, 0xFF3399CC, 0xFF33CCCC, 0xFF33FFCC, 0xFF33FF33, 0xFF33CC33, 0xFF339933, 0xFF336633, 0xFF333333, 0xFF330033,
                0xFF6600CC, 0xFF6633CC, 0xFF6666CC, 0xFF6699CC, 0xFF66CCCC, 0xFF66FFCC, 0xFF66FF33, 0xFF66CC33, 0xFF669933, 0xFF666633, 0xFF663333, 0xFF660033,
                0xFF9900CC, 0xFF9933CC, 0xFF9966CC, 0xFF9999CC, 0xFF99CCCC, 0xFF99FFCC, 0xFF99FF33, 0xFF99CC33, 0xFF999933, 0xFF996633, 0xFF993333, 0xFF990033,
                0xFFCC00CC, 0xFFCC33CC, 0xFFCC66CC, 0xFFCC99CC, 0xFFCCCCCC, 0xFFCCFFCC, 0xFFCCFF33, 0xFFCCCC33, 0xFFCC9933, 0xFFCC6633, 0xFFCC3333, 0xFFCC0033,
                0xFFFF00CC, 0xFFFF33CC, 0xFFFF66CC, 0xFFFF99CC, 0xFFFFCCCC, 0xFFFFFFCC, 0xFFFFFF33, 0xFFFFCC33, 0xFFFF9933, 0xFFFF6633, 0xFFFF3333, 0xFFFF0033,
                0xFFFF0099, 0xFFFF3399, 0xFFFF6699, 0xFFFF9999, 0xFFFFCC99, 0xFFFFFF99, 0xFFFFFF00, 0xFFFFCC00, 0xFFFF9900, 0xFFFF6600, 0xFFFF3300, 0xFFFF0000,
                0xFFCC0099, 0xFFCC3399, 0xFFCC6699, 0xFFCC9999, 0xFFCCCC99, 0xFFCCFF99, 0xFFCCFF00, 0xFFCCCC00, 0xFFCC9900, 0xFFCC6600, 0xFFCC3300, 0xFFCC0000,
                0xFF990099, 0xFF993399, 0xFF996699, 0xFF999999, 0xFF99CC99, 0xFF99FF99, 0xFF99FF00, 0xFF99CC00, 0xFF999900, 0xFF996600, 0xFF993300, 0xFF990000,
                0xFF660099, 0xFF663399, 0xFF666699, 0xFF669999, 0xFF66CC99, 0xFF66FF99, 0xFF66FF00, 0xFF66CC00, 0xFF669900, 0xFF666600, 0xFF663300, 0xFF660000,
                0xFF330099, 0xFF333399, 0xFF336699, 0xFF339999, 0xFF33CC99, 0xFF33FF99, 0xFF33FF00, 0xFF33CC00, 0xFF339900, 0xFF336600, 0xFF333300, 0xFF330000,
                0xFF000099, 0xFF003399, 0xFF006699, 0xFF009999, 0xFF00CC99, 0xFF00FF99, 0xFF00FF00, 0xFF00CC00, 0xFF009900, 0xFF006600, 0xFF003300, 0xFF000000
            };

            Set(colors);

            ElementWebPalette.ItemSelected += new ItemViewerEventHandler(ElementWebPalette_ItemSelected);
        }

        #endregion

        #region Private Methods

        public void Set(uint[] colors)
        {
            uint a;
            uint r;
            uint g;
            uint b;

            ElementWebPalette.DisableUpdates = true;

            foreach (uint i in colors)
            {
                a = (i >> 24) & 255;
                r = (i >> 16) & 255;
                g = (i >> 8) & 255;
                b = (i >> 0) & 255;

                Add(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
            }

            ElementWebPalette.DisableUpdates = false;
        }

        public void Add(Color color)
        {
            ColorItem item = new ColorItem();

            item.Width = 26;
            item.Background = new SolidColorBrush(color);

            ToolTipService.SetToolTip(item, "#" + color.ToString().Substring(3));

            AddColor(item);
        }

        private void AddColor(ColorItem item)
        {
            ElementWebPalette.Add(item);
        }

        #endregion

        #region Event Handling

        private void ElementWebPalette_ItemSelected(object sender, ItemViewerEventArgs e)
        {
            ParentDialog.GetButton("ok").IsEnabled = true;
            _selected = ((ColorItem)ElementWebPalette.Selected).Color;
        }

        #endregion

        #region Color Picker

        private ColorSpace m_colorSpace;
        private bool m_sliderMouseDown;
        private bool m_sampleMouseDown;
        private float m_selectedHue;
        private int m_sampleX;
        private int m_sampleY;
        private Color m_selectedColor;

        private void InitcolorPicker()
        {
            rectHueMonitor.MouseLeftButtonDown += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonDown);
            rectHueMonitor.MouseLeftButtonUp += new MouseButtonEventHandler(rectHueMonitor_MouseLeftButtonUp);
            rectHueMonitor.MouseLeave += new MouseEventHandler(rectHueMonitor_MouseLeave);
            rectHueMonitor.MouseMove += new MouseEventHandler(rectHueMonitor_MouseMove);

            rectSampleMonitor.MouseLeftButtonDown += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonDown);
            rectSampleMonitor.MouseLeftButtonUp += new MouseButtonEventHandler(rectSampleMonitor_MouseLeftButtonUp);
            rectSampleMonitor.MouseLeave += new MouseEventHandler(rectSampleMonitor_MouseLeave);
            rectSampleMonitor.MouseMove += new MouseEventHandler(rectSampleMonitor_MouseMove);

            HexValue.TextChanged += new TextChangedEventHandler(HexValue_TextChanged);

            m_colorSpace = new ColorSpace();
            m_selectedHue = 0;
            m_sampleX = (int)rectSampleMonitor.Width;
            m_sampleY = 0;
            UpdateSample(m_sampleX, m_sampleY);
        }

        private void HexValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool success = false;

            try
            {
                if (HexValue.Text.StartsWith("#") && HexValue.Text.Length == 7)
                {
                    SelectedColor.Fill = GetBrush(HexValue.Text);
                    _selected = ((SolidColorBrush)SelectedColor.Fill).Color;
                    success = true;
                }
            }
            catch (Exception ex)
            {
            }

            if (ParentDialog != null)
            {
                ParentDialog.GetButton("ok").IsEnabled = success;
            }
        }

        private void rectHueMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            m_sliderMouseDown = true;
            int yPos = (int)e.GetPosition((UIElement)sender).Y;
            UpdateSelection(yPos);
            rectHueMonitor.CaptureMouse();
        }

        private void rectHueMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            m_sliderMouseDown = false;
            rectHueMonitor.ReleaseMouseCapture();
        }

        private void rectHueMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_sliderMouseDown)
            {
                int yPos = (int)e.GetPosition((UIElement)sender).Y;
                UpdateSelection(yPos);
            }
        }

        private void rectHueMonitor_MouseLeave(object sender, EventArgs e)
        {
            m_sliderMouseDown = false;
        }

        private void rectSampleMonitor_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            rectSampleMonitor.CaptureMouse();
            m_sampleMouseDown = true;
            Point pos = e.GetPosition((UIElement)sender);
            m_sampleX = (int)pos.X;
            m_sampleY = (int)pos.Y;
            UpdateSample(m_sampleX, m_sampleY);
        }

        private void rectSampleMonitor_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            rectSampleMonitor.ReleaseMouseCapture();
            m_sampleMouseDown = false;
        }

        private void rectSampleMonitor_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_sampleMouseDown)
            {
                Point pos = e.GetPosition((UIElement)sender);
                m_sampleX = (int)pos.X;
                m_sampleY = (int)pos.Y;
                UpdateSample(m_sampleX, m_sampleY);
            }
        }

        private void rectSampleMonitor_MouseLeave(object sender, EventArgs e)
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
            HexValue.Text = m_colorSpace.GetHexCode(m_selectedColor);
            SelectedColor.Fill = new SolidColorBrush(m_selectedColor);

            if (ParentDialog != null)
            {
                ParentDialog.GetButton("ok").IsEnabled = true;
            }
            _selected = m_selectedColor;
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

        #endregion

        #region Utilities

        public static Brush GetBrush(string brush)
        {
            Brush result;
            Color tempColor;

            if (brush.StartsWith("#"))
            {
                if (brush.Length == 7)
                {
                    brush = brush.Replace("#", "#ff");
                }
                tempColor = RichTextBoxStyle.StringToColor(brush.TrimStart('#'));
                result = new SolidColorBrush(tempColor);
            }
            else
            {
                result = (Brush)CreateFromXaml(brush);
            }

            return result;
        }

        public static object CreateFromXaml(string xaml)
        {
            string ns = "http://schemas.microsoft.com/client/2007";

            if (!xaml.Contains(ns))
            {
                xaml = xaml.Insert(xaml.IndexOf('>'), " xmlns=\"" + ns + "\"");
            }

            return XamlReader.Load(xaml);
        }

        #endregion
    }

    internal class ColorSpace
    {
        private const byte MIN = 0;
        private const byte MAX = 255;

        public Color GetColorFromPosition(int position)
        {
            byte mod = (byte)(position % MAX);
            byte diff = (byte)(MAX - mod);
            byte alpha = 255;

            switch (position / MAX)
            {
                case 0: return Color.FromArgb(alpha, MAX, mod, MIN);
                case 1: return Color.FromArgb(alpha, diff, MAX, MIN);
                case 2: return Color.FromArgb(alpha, MIN, MAX, mod);
                case 3: return Color.FromArgb(alpha, MIN, diff, MAX);
                case 4: return Color.FromArgb(alpha, mod, MIN, MAX);
                case 5: return Color.FromArgb(alpha, MAX, MIN, diff);
                default: return Colors.Black;
            }
        }

        public string GetHexCode(Color c)
        {
            return string.Format("#{0}{1}{2}",
                c.R.ToString("X2"),
                c.G.ToString("X2"),
                c.B.ToString("X2"));
        }

        // Algorithm ported from: http://www.colorjack.com/software/dhtml+color+picker.html
        public Color ConvertHsvToRgb(float h, float s, float v)
        {
            h = h / 360;
            if (s > 0)
            {
                if (h >= 1)
                    h = 0;
                h = 6 * h;
                int hueFloor = (int)Math.Floor(h);
                byte a = (byte)Math.Round(MAX * v * (1.0 - s));
                byte b = (byte)Math.Round(MAX * v * (1.0 - (s * (h - hueFloor))));
                byte c = (byte)Math.Round(MAX * v * (1.0 - (s * (1.0 - (h - hueFloor)))));
                byte d = (byte)Math.Round(MAX * v);

                switch (hueFloor)
                {
                    case 0: return Color.FromArgb(MAX, d, c, a);
                    case 1: return Color.FromArgb(MAX, b, d, a);
                    case 2: return Color.FromArgb(MAX, a, d, c);
                    case 3: return Color.FromArgb(MAX, a, b, d);
                    case 4: return Color.FromArgb(MAX, c, a, d);
                    case 5: return Color.FromArgb(MAX, d, a, b);
                    default: return Color.FromArgb(0, 0, 0, 0);
                }
            }
            else
            {
                byte d = (byte)(v * MAX);
                return Color.FromArgb(255, d, d, d);
            }
        }
    }
}
