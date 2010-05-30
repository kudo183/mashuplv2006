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
using System.Collections.Generic;

namespace MashupDesignTool
{
    public class ControlGroupInfo
    {
        private string groupName;
        private List<ControlInfo> controlInfos;

        public ControlGroupInfo(string groupName)
        {
            this.groupName = groupName;
            controlInfos = new List<ControlInfo>();
        }

        public string GroupName
        {
            get { return groupName; }
            set { groupName = value; }
        }

        public List<ControlInfo> ControlInfos
        {
            get { return controlInfos; }
            set { controlInfos = value; }
        }
    }
}
