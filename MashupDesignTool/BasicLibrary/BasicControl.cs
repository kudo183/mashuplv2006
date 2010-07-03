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
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace BasicLibrary
{
    public class BasicControl : UserControl, IBasic
    {
        #region IBasic Members
        protected List<string> parameterNameList = new List<string>();
        public string GetParameterNames()
        {
            //if (!parameterNameList.Contains("Width"))
            //    parameterNameList.Add("Width");
            //if (!parameterNameList.Contains("Height"))
            //    parameterNameList.Add("Height"); 
            
            StringBuilder sb = new StringBuilder();
            XmlWriter xw = XmlWriter.Create(sb);
            xw.WriteStartDocument();
            xw.WriteStartElement("ParameterNames");

            foreach (string name in parameterNameList)
            {
                xw.WriteStartElement("ParameterName");
                xw.WriteValue(name);
                xw.WriteEndElement();
            }

            xw.WriteEndElement();
            xw.WriteEndDocument();
            xw.Flush();
            xw.Close();
            return sb.ToString();
        }

        public List<string> GetParameterNameList()
        {
            //if (!parameterNameList.Contains("Width"))
            //    parameterNameList.Add("Width");
            //if (!parameterNameList.Contains("Height"))
            //    parameterNameList.Add("Height");
            return parameterNameList;
        }

        public Type GetParameterType(string parameterName)
        {
            if (parameterNameList.Contains(parameterName))
            {
                PropertyInfo pi = this.GetType().GetProperty(parameterName);
                if (pi == null)
                    return null;
                return pi.PropertyType;
            }
            return null;
        }

        public object GetParameterValue(string parameterName)
        {
            if (parameterNameList.Contains(parameterName))
            {
                PropertyInfo pi = this.GetType().GetProperty(parameterName);
                if (pi == null)
                    return null;
                return pi.GetValue(this, null);
            }
            return null;
        }

        public bool SetParameterValue(string parameterName, object value)
        {
            if (parameterNameList.Contains(parameterName))
            {
                PropertyInfo pi = this.GetType().GetProperty(parameterName);
                if (pi == null)
                    return false;
                Type t = GetParameterType(parameterName);
                if (t == null)
                    return false;
                pi.SetValue(this, Convert.ChangeType(value, t, null), null);
                return true;
            }
            return false;
        }
        #endregion

        public delegate void MDTEventHandler(object sender, string xmlString);

        public delegate void BCVisibilityChangedHandler(object sender, System.Windows.Visibility newValue);
        public event BCVisibilityChangedHandler BCVisibilityChanged;

        protected List<string> effectPropertyNameList = new List<string>();
        private List<string> eventNameList = new List<string>();
        private List<string> operationNameList = new List<string>();

        #region Effect Property Name List
        public List<string> GetListEffectPropertyName()
        {
            return effectPropertyNameList;
        }

        public Type GetEffectType(string effectName)
        {
            if (!effectPropertyNameList.Contains(effectName))
                return null;
            return this.GetType().GetProperty(effectName).PropertyType;
        }

        public IBasic GetEffect(string effectName)
        {
            if (!effectPropertyNameList.Contains(effectName))
                return null;
            return (IBasic)this.GetType().GetProperty(effectName).GetValue(this, null);
        }
        #endregion Effect Property Name List

        #region Event Name List
        public List<string> GetListEventName()
        {
            return eventNameList;
        }

        public EventInfo GetEventInfoByName(string eventName)
        {
            if (!eventNameList.Contains(eventName))
                return null;
            return this.GetType().GetEvent(eventName);
        }

        protected void AddEventNameToList(string eventName)
        {
            EventInfo ei = this.GetType().GetEvent(eventName);
            if (ei != null)
                if (ei.EventHandlerType == typeof(MDTEventHandler))
                    eventNameList.Add(eventName);
        }
        #endregion Event Name List

        #region Operation Name List
        public List<string> GetListOperationName()
        {
            return operationNameList;
        }

        public MethodInfo GetOperationInfoByName(string operationName)
        {
            if (!operationNameList.Contains(operationName))
                return null;
            return this.GetType().GetMethod(operationName);
        }

        protected void AddOperationNameToList(string operationName)
        {
            MethodInfo mi = this.GetType().GetMethod(operationName);
            if (mi != null)
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length != 1)
                    return;
                if (pis[0].ParameterType != typeof(string))
                    return;
                if (mi.IsPublic && !mi.IsStatic)
                    operationNameList.Add(operationName);
            }
        }
        #endregion Operation Name List

        public BasicControl()
            : base()
        {
            parameterNameList.Add("Left");
            parameterNameList.Add("Top");
            parameterNameList.Add("Width");
            parameterNameList.Add("Height");
            parameterNameList.Add("DockType");
            parameterNameList.Add("ZIndex");
            parameterNameList.Add("ControlName");
            parameterNameList.Add("Visible");

            effectPropertyNameList.Add("MainEffect");
            effectPropertyNameList.Add("AppearEffect");
            effectPropertyNameList.Add("DisappearEffect");

            AddOperationNameToList("Show");
            AddOperationNameToList("Hide");
        }

        protected BasicEffect mainEffect;
        protected BasicAppearEffect appearEffect;
        protected BasicDisappearEffect disappearEffect;
        protected string controlName;

        public BasicEffect MainEffect
        {
            get { return mainEffect; }
        }

        public BasicAppearEffect AppearEffect
        {
            get { return appearEffect; }
        }

        public BasicDisappearEffect DisappearEffect
        {
            get { return disappearEffect; }
        }

        public string ControlName
        {
            get { return controlName; }
            set { controlName = value; }
        }

        public bool Visible
        {
            get
            {
                if (this.Visibility == System.Windows.Visibility.Collapsed) 
                    return false;
                else
                    return true;
            }
            set
            {
                System.Windows.Visibility vis = System.Windows.Visibility.Collapsed;
                if (value == true)
                    vis = System.Windows.Visibility.Visible;
                if (this.Visibility != vis)
                {
                    this.Visibility = vis;
                    if (BCVisibilityChanged != null)
                        BCVisibilityChanged(this, vis);
                }
            }
        }

        public virtual void ChangeEffect(string propertyName, Type effectType, EffectableControl owner)
        {
            if (propertyName == "MainEffect")
            {
                if (mainEffect != null)
                    mainEffect.DetachEffect();
                ConstructorInfo ci = effectType.GetConstructor(new Type[] { typeof(EffectableControl) });
                mainEffect = (BasicEffect)ci.Invoke(new object[] { owner });
            }
            else if (propertyName == "AppearEffect")
            {
                if (appearEffect != null)
                    appearEffect.DetachEffect();
                ConstructorInfo ci = effectType.GetConstructor(new Type[] { typeof(EffectableControl) });
                appearEffect = (BasicAppearEffect)ci.Invoke(new object[] { owner });
            }
            else if (propertyName == "DisappearEffect")
            {
                if (disappearEffect != null)
                    disappearEffect.DetachEffect();
                ConstructorInfo ci = effectType.GetConstructor(new Type[] { typeof(EffectableControl) });
                disappearEffect = (BasicDisappearEffect)ci.Invoke(new object[] { owner });
            }
        }

        protected void StartMainEffect()
        {
            if (mainEffect != null)
                mainEffect.Start();
        }

        public virtual void Dispose()
        {
        }

        public void Show(string xml)
        {
            if (this.Visibility == System.Windows.Visibility.Visible)
                return;
            this.Visible = true;
            //this.Visibility = System.Windows.Visibility.Visible;
            if (appearEffect != null)
                appearEffect.Start();
        }

        public void Hide(string xml)
        {
            if (this.Visibility == System.Windows.Visibility.Collapsed)
                return;
            if (disappearEffect != null)
            {
                disappearEffect.MDTEffectCompleted += new BasicEffect.MDTEffectCompleteHandler(disappearEffect_MDTEffectCompleted);
                disappearEffect.Start();
            }
            else
                //this.Visibility = System.Windows.Visibility.Collapsed;
                this.Visible = false;
        }

        void disappearEffect_MDTEffectCompleted(object sender)
        {
            disappearEffect.MDTEffectCompleted -= new BasicEffect.MDTEffectCompleteHandler(disappearEffect_MDTEffectCompleted);
            //this.Visibility = System.Windows.Visibility.Collapsed;
            this.Visible = false;
        }
    }   
}
