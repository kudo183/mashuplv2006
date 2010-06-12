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

        public static bool RegisterEvent(BasicControl raiseControl, string eventName, BasicControl handleControl, string handleOperation)
        {
            List<MDTEventInfo> list = GetListEventInfoRaiseBy(raiseControl);
            foreach (MDTEventInfo mdtei in list)
                if (mdtei.EventName == eventName)
                {
                    EventInfo ei = raiseControl.GetEventInfoByName(mdtei.EventName);
                    ei.RemoveEventHandler(mdtei.RaiseControl, Delegate.CreateDelegate(ei.EventHandlerType, mdtei, "HandleFunction"));
                    listEventInfo.Remove(mdtei);
                    break;
                }

            MDTEventInfo mei = MDTEventInfo.RegisterEvent(raiseControl, eventName, handleControl, handleOperation);
            if (mei == null)
                return false;
            listEventInfo.Add(mei);
            return true;
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

        public static List<MDTEventInfo> GetListEventInfoHandleBy(BasicControl handleControl)
        {
            List<MDTEventInfo> list = new List<MDTEventInfo>();
            foreach (MDTEventInfo mei in listEventInfo)
            {
                if (mei.HandleControl.Name == handleControl.Name)
                    list.Add(mei);
            }
            return list;
        }
    }
}
