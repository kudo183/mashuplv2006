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
using System.Collections.Generic;

namespace Controls
{
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
            return string.Format("#{0}{1}{2}{3}",
                c.A.ToString("X2"),
                c.R.ToString("X2"),
                c.G.ToString("X2"),
                c.B.ToString("X2"));
        }

        public Color FromString(string s, out bool error)
        {
            error = false;
            if (s.Length != 9)
            {
                error = true;
                return Colors.Black;
            }
            try
            {
                byte a = byte.Parse(s.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                byte r = byte.Parse(s.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                byte g = byte.Parse(s.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                byte b = byte.Parse(s.Substring(7, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                return Color.FromArgb(a, r, g, b);
            }
            catch (Exception ex)
            {
                error = true;
                return Colors.Black;
            }
        }
        // Algorithm ported from: http://nofunc.org/Color_Conversion_Library/
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
        // Algorithm ported from: http://nofunc.org/Color_Conversion_Library/
        public void ConvertRgbToHSV(Color c, ref float h, ref float s, ref float v)
        {
            float min, max, delta;

            min = Math.Min(c.R, Math.Min(c.G, c.B));
            max = Math.Max(c.R, Math.Max(c.G, c.B));

            delta = max - min;
            if (delta == 0)
                return;
            if (max != 0)
                s = (float)Math.Round(delta / max * 100);		// s

            if (c.R == max)
                h = (c.G - c.B) / delta;		// between yellow & magenta
            else if (c.G == max)
                h = 2 + (c.B - c.R) / delta;	// between cyan & yellow
            else
                h = 4 + (c.R - c.G) / delta;	// between magenta & cyan

            h = Math.Min((int)Math.Round(h * 60), 360);
            if (h < 0)
                h += 360;

            v = (float)Math.Round(max / 255 * 100);
        }
    }
}

