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

namespace MashupDesignTool
{
    class CursorManager
    {
        public enum CursorType
        {
            SizeNS,
            SizeWE,
            SizeENWS,
            SizeWNES,
            Arrow
        }

        private static CursorType currCursor = CursorType.Arrow;
        private static CustomCursor cursorSizeENWS = new CustomCursor(@"Images/SizeENWS.png");
        private static CustomCursor cursorSizeWNES = new CustomCursor(@"Images/SizeWNES.png");

        public static void InitCursor(Canvas canvas)
        {
            canvas.Children.Add(cursorSizeENWS);
            canvas.Children.Add(cursorSizeWNES);

            cursorSizeENWS.Visibility = Visibility.Collapsed;
            cursorSizeWNES.Visibility = Visibility.Collapsed;

            Canvas.SetZIndex(cursorSizeENWS, 10000);
            Canvas.SetZIndex(cursorSizeWNES, 10000);
        }

        public static void ChangeCursor(FrameworkElement page, CursorType cursor)
        {
            cursorSizeENWS.Visibility = Visibility.Collapsed;
            cursorSizeWNES.Visibility = Visibility.Collapsed;

            currCursor = cursor;
            switch (currCursor)
            {
                case CursorType.SizeNS:
                    page.Cursor = Cursors.SizeNS;
                    break;
                case CursorType.SizeWE:
                    page.Cursor = Cursors.SizeWE;
                    break;
                case CursorType.SizeENWS:
                    page.Cursor = Cursors.None;
                    cursorSizeENWS.Visibility = Visibility.Visible;
                    break;
                case CursorType.SizeWNES:
                    page.Cursor = Cursors.None;
                    cursorSizeWNES.Visibility = Visibility.Visible;
                    break;
                case CursorType.Arrow:
                    page.Cursor = Cursors.Arrow;
                    break;
            }
        }

        public static void UpdateCursorPosition(Point pt)
        {
            if (currCursor == CursorType.SizeENWS)
            {
                cursorSizeENWS.MoveTo(pt);
            }
            else if (currCursor == CursorType.SizeWNES)
            {
                cursorSizeWNES.MoveTo(pt);
            }
        }
    }
}
