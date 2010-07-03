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
using BasicLibrary;
using System.Reflection;

namespace MashupDesignTool
{
    public class MDTEventManager
    {
        private static List<MDTEventInfo> listEventInfo = new List<MDTEventInfo>();

        public static MDTEventInfo RegisterEvent(BasicControl raiseControl, string eventName, List<BasicControl> handleControls, List<string> handleOperations)
        {
            RemoveEvent(raiseControl, eventName);

            MDTEventInfo mei = MDTEventInfo.RegisterEvent(raiseControl, eventName, handleControls, handleOperations);
            if (mei == null)
                return null;
            listEventInfo.Add(mei);
            return mei;
        }

        public static List<MDTEventInfo> GetListEventInfoRaiseBy(BasicControl raiseControl)
        {
            List<MDTEventInfo> list = new List<MDTEventInfo>();
            foreach (MDTEventInfo mei in listEventInfo)
            {
                if (mei.RaiseControl.Name == raiseControl.Name)
                    list.Add(mei);
            }
            return list;
        }

        public static MDTEventInfo GetEventInfo(BasicControl raiseControl, string eventName)
        {
            List<MDTEventInfo> list = GetListEventInfoRaiseBy(raiseControl);
            foreach (MDTEventInfo mdtei in list)
                if (mdtei.EventName == eventName)
                    return mdtei;

            return null;
        }

        private static List<MDTEventInfo> GetListEventInfoHandleBy(BasicControl handleControl)
        {
            List<MDTEventInfo> list = new List<MDTEventInfo>();
            foreach (MDTEventInfo mei in listEventInfo)
            {
                foreach (BasicControl bc in mei.HandleControls)
                {
                    if (bc.Name == handleControl.Name)
                    {
                        list.Add(mei);
                        break;
                    }
                }
            }
            return list;
        }

        public static void RemoveEventInfoRelateTo(BasicControl control)
        {
            List<MDTEventInfo> list = GetListEventInfoRaiseBy(control);
            foreach (MDTEventInfo mei in list)
                RemoveEvent(mei);

            list = GetListEventInfoHandleBy(control);
            foreach (MDTEventInfo mei in list)
                RemoveEvent(mei);
        }

        public static void RemoveEvent(BasicControl raiseControl, string eventName)
        {
            List<MDTEventInfo> list = GetListEventInfoRaiseBy(raiseControl);
            foreach (MDTEventInfo mdtei in list)
                if (mdtei.EventName == eventName)
                {
                    RemoveEvent(mdtei);
                    return;
                }
        }

        private static void RemoveEvent(MDTEventInfo mei)
        {
            if (mei == null)
                return;
            mei.DetachEvent();
            listEventInfo.Remove(mei);
        }
    }
}
