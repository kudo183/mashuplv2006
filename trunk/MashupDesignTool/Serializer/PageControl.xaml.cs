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
using BasicLibrary;

namespace MashupDesignTool
{
    public partial class PageControl : BasicControl
    {
        public delegate void LoadControlCompletedHandler(PageControl pc);
        public event LoadControlCompletedHandler LoadControlCompleted;

        private bool isLoading = false;
        
        private string xml;
        public PageControl()
        {
            InitializeComponent();
        }

        public string Xml
        {
            get { return xml; }
        }

        public void LoadControl(string xml)
        {
            isLoading = true;
            this.xml = xml;
            PageSerializer ps = new PageSerializer();
            ps.DeserializeCompleted += new PageSerializer.DeserializeCompletedHandler(ps_DeserializeCompleted);
            ps.Deserialize(xml, dockCanvas1);
        }

        void ps_DeserializeCompleted()
        {
            isLoading = false;
            this.Width = dockCanvas1.Width;
            this.Height = dockCanvas1.Height;

            if (LoadControlCompleted != null)
                LoadControlCompleted(this);
        }

        private void BasicControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (isLoading)
                return;
            dockCanvas1.Width = e.NewSize.Width;
            dockCanvas1.Height = e.NewSize.Height;
            dockCanvas1.UpdateChildrenPosition();
        }
    }
}
