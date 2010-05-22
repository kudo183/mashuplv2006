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
using System.Xml.Linq;

namespace MashupDesignTool
{
    public class EffectInfo
    {
        private string effectName;
        private string displayName;
        private string dllFilename;
        private string description;
        private string iconName;
        private string group;
        private List<string> dllReferences;
        private List<bool> isDllReferencesDownloaded;
        private bool isDllFileDownloaded;

        public string EffectName
        {
            get { return effectName; }
            set { effectName = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public string DllFilename
        {
            get { return dllFilename; }
            set { dllFilename = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string IconName
        {
            get { return iconName; }
            set { iconName = value; }
        }

        public string Group
        {
            get { return group; }
            set { group = value; }
        }

        public List<string> DllReferences
        {
            get { return dllReferences; }
            set { dllReferences = value; }
        }

        public List<bool> IsDllReferencesDownloaded
        {
            get { return isDllReferencesDownloaded; }
            set { isDllReferencesDownloaded = value; }
        }

        public bool IsDllFileDownloaded
        {
            get { return isDllFileDownloaded; }
            set { isDllFileDownloaded = value; }
        }

        public bool IsReady
        {
            get
            {
                if (!isDllFileDownloaded)
                    return false;
                for (int i = 0; i < isDllReferencesDownloaded.Count; i++)
                    if (!isDllReferencesDownloaded[i])
                        return false;
                return true;
            }
        }

        public EffectInfo(string xml) : this(XElement.Parse(xml))
        {
        }

        public EffectInfo(XElement element)
        {
            effectName = element.Element("Name").Value;
            dllFilename = element.Element("DllFilename").Value;
            displayName = element.Element("DisplayName").Value;
            description = element.Element("Description").Value;
            group = element.Element("Group").Value;
            iconName = element.Element("IconName").Value;
            isDllFileDownloaded = false;

            dllReferences = new List<string>();
            isDllReferencesDownloaded = new List<bool>();
            XElement temp = element.Element("DllReferences");
            if (temp != null)
            {
                foreach (XElement child in temp.Descendants("Dll"))
                {
                    dllReferences.Add(child.Value);
                    isDllReferencesDownloaded.Add(false);
                }
            }
        }

        public void CheckDllReferences(string dll)
        {
            for (int i = 0; i < dllReferences.Count;i++)
                if (dllReferences[i] == dll)
                {
                    isDllReferencesDownloaded[i] = true;
                }
        }

        public void CheckDllFilename(string dll)
        {
            if (dllFilename == dll)
                IsDllFileDownloaded = true;
        }
    }
}
