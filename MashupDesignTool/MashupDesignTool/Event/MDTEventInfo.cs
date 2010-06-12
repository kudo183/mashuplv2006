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

namespace MashupDesignTool
{
    public class MDTEventInfo
    {
        BasicControl raiseControl, handleControl;
        string eventName, handleOperation;

        #region Property
        public BasicControl RaiseControl
        {
            get { return raiseControl; }
        }

        public BasicControl HandleControl
        {
            get { return handleControl; }
        }

        public string EventName
        {
            get { return eventName; }
        }

        public string HandleOperation
        {
            get { return handleOperation; }
        }
        #endregion Property

        internal MDTEventInfo(BasicControl raiseControl, string eventName, BasicControl handleControl, string handleOperation)
        {
            this.raiseControl = raiseControl;
            this.handleControl = handleControl;
            this.eventName = eventName;
            this.handleOperation = handleOperation;
        }

        public static MDTEventInfo RegisterEvent(BasicControl raiseControl, string eventName, BasicControl handleControl, string handleOperation)
        {
            if (raiseControl == null || handleControl == null)
                return null;
            EventInfo ei = raiseControl.GetEventInfoByName(eventName);
            MethodInfo mi = handleControl.GetOperationInfoByName(handleOperation);
            if (ei == null || mi == null)
                return null;

            MDTEventInfo mei = new MDTEventInfo(raiseControl, eventName, handleControl, handleOperation);
            Type delegateType = ei.EventHandlerType;
            try { ei.AddEventHandler(raiseControl, Delegate.CreateDelegate(delegateType, mei, "HandleFunction")); }
            catch { return null; }
            return mei;
        }

        public void HandleFunction(object sender, string xmlString)
        {
            handleControl.GetOperationInfoByName(handleOperation).Invoke(handleControl, new object[] { xmlString });
        }
    }
}
