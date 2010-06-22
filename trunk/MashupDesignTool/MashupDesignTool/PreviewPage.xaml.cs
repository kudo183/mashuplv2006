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

namespace MashupDesignTool
{
    public partial class PreviewPage : UserControl
    {
        public delegate void BackToEditorHandler(object sender);
        public event BackToEditorHandler BackToEditor;

        public PreviewPage()
        {
            InitializeComponent();
        }

        public void Preview(string xml)
        {
            DockCanvasSerializer.Deserialize(xml, dockCanvas1);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Appear.Begin();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (BackToEditor != null)
                BackToEditor(this);
        }
    }
}
