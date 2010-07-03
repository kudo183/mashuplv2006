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
using BasicLibrary;
using System.Reflection;
using System.Collections.Generic;

namespace MashupDesignTool
{
    public class MDTEventInfo
    {
        BasicControl raiseControl;
        List<BasicControl> handleControls;
        string eventName;
        List<string> handleOperations;

        #region Property
        public BasicControl RaiseControl
        {
            get { return raiseControl; }
        }

        public List<BasicControl> HandleControls
        {
            get { return handleControls; }
        }

        public string EventName
        {
            get { return eventName; }
        }

        public List<string> HandleOperations
        {
            get { return handleOperations; }
        }
        #endregion Property

        internal MDTEventInfo(BasicControl raiseControl, string eventName, List<BasicControl> handleControls, List<string> handleOperations)
        {
            this.raiseControl = raiseControl;
            this.handleControls = handleControls;
            this.eventName = eventName;
            this.handleOperations = handleOperations;
        }

        public static MDTEventInfo RegisterEvent(BasicControl raiseControl, string eventName, List<BasicControl> handleControls, List<string> handleOperations)
        {
            if (raiseControl == null || handleControls == null)
                return null;
            if (handleOperations.Count != handleControls.Count)
                return null;
            if (handleOperations.Count == 0)
                return null;
            foreach (BasicControl bs in handleControls)
                if (bs == null)
                    return null;

            EventInfo ei = raiseControl.GetEventInfoByName(eventName);
            if (ei == null)
                return null;
            for (int i = 0; i < handleControls.Count; i++)
            {
                MethodInfo mi = handleControls[i].GetOperationInfoByName(handleOperations[i]);
                if (mi == null)
                    return null;
            }

            MDTEventInfo mei = new MDTEventInfo(raiseControl, eventName, handleControls, handleOperations);
            Type delegateType = ei.EventHandlerType;
            try { ei.AddEventHandler(raiseControl, Delegate.CreateDelegate(delegateType, mei, "HandleFunction")); }
            catch { return null; }
            return mei;
        }

        public void HandleFunction(object sender, string xmlString)
        {
            for (int i = 0; i < handleControls.Count; i++)
            {
                handleControls[i].GetOperationInfoByName(handleOperations[i]).Invoke(handleControls[i], new object[] { xmlString });
            }
        }

        public void DetachEvent()
        {
            EventInfo ei = raiseControl.GetEventInfoByName(eventName);
            if (ei == null)
                return;

            try 
            { 
                ei.AddEventHandler(raiseControl, Delegate.CreateDelegate(ei.EventHandlerType, this, "HandleFunction")); 
            }
            catch {}
        }
    }
}
