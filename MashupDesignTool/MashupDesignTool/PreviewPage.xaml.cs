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
        LoadingWindow lw;

        private bool loaded = false;

        public PreviewPage()
        {
            InitializeComponent();
        }

        public void Preview(string xml)
        {
            DockCanvasSerializer dcs = new DockCanvasSerializer();
            dcs.DeserializeCompleted += new DockCanvasSerializer.DeserializeCompletedHandler(dcs_DeserializeCompleted);
            dcs.Deserialize(xml, dockCanvas1);
        }

        void dcs_DeserializeCompleted()
        {
            loaded = true;
            if (lw != null)
            {
                lw.Close();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Appear.Completed += new EventHandler(Appear_Completed);
            Appear.Begin();
        }

        void Appear_Completed(object sender, EventArgs e)
        {
            if (loaded == false)
            {
                lw = new LoadingWindow();
                lw.Closed += new EventHandler(lw_Closed);
                lw.Show();
            }
        }

        void lw_Closed(object sender, EventArgs e)
        {
            if (lw.DialogResult == false)
            {
                if (BackToEditor != null)
                    BackToEditor(this);
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (BackToEditor != null)
                BackToEditor(this);
        }
    }
}
