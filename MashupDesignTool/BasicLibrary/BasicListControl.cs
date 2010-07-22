using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Xml;
using System.Reflection;
namespace BasicLibrary
{
    public class BasicListControl : BasicControl
    {
        public enum ListItemsAction
        {
            ADD,
            INSERT,
            SWAP,
            REMOVE,
            REMOVEAT,
            REMOVEALL,
        }
        public delegate void ListChangeHandler(ListItemsAction action, int index1, EffectableControl control, int index2);
        public event ListChangeHandler OnListChange;

        private List<EffectableControl> _items = new List<EffectableControl>();
        public virtual ReadOnlyCollection<EffectableControl> Items
        {
            get { return new ReadOnlyCollection<EffectableControl>(_items); }
        }
        public virtual int ItemCount
        {
            get { return _items.Count; }
        }
        public virtual void AddItem(EffectableControl control)
        {
            if (listItemEffect != null)
                control.ChangeEffect("MainEffect", listItemEffect.GetType());
            if (OnListChange != null)
                OnListChange(ListItemsAction.ADD, -1, control, -1);
            _items.Add(control);

            BasicControl bc = control.Control as BasicControl;
            if (bc != null)
                bc.LinkClicked += new MDTEventHandler(bc_LinkClicked);
        }

        void bc_LinkClicked(object sender, string xmlString)
        {
            OnLinkClicked(xmlString);
        }

        public virtual void InsertItem(int index, EffectableControl control)
        {
            if (OnListChange != null)
                OnListChange(ListItemsAction.INSERT, index, control, -1);
            _items.Insert(index, control);
        }

        public virtual void SwapItem(int index1, int index2)
        {
            if (OnListChange != null)
                OnListChange(ListItemsAction.SWAP, index1, null, index2);
            EffectableControl temp = _items[index1];
            _items[index1] = _items[index2];
            _items[index2] = temp;
        }
        public virtual void RemoveItemAt(int index)
        {
            if (index < 0 || index >= _items.Count)
                return;

            if (OnListChange != null)
                OnListChange(ListItemsAction.REMOVEAT, index, null, -1);
            _items.RemoveAt(index);

            BasicControl bc = _items[index].Control as BasicControl;
            if (bc != null)
                bc.LinkClicked -= new MDTEventHandler(bc_LinkClicked);
        }
        public virtual void RemoveItem(EffectableControl control)
        {
            if (OnListChange != null)
                OnListChange(ListItemsAction.REMOVE, -1, control, -1);
            _items.Remove(control);

            BasicControl bc = control.Control as BasicControl;
            if (bc != null)
                bc.LinkClicked -= new MDTEventHandler(bc_LinkClicked);
        }
        public virtual void RemoveAllItem()
        {
            if (OnListChange != null)
                OnListChange(ListItemsAction.REMOVEALL, -1, null, -1);

            BasicControl bc;
            foreach (EffectableControl ec in _items)
            {
                bc = ec.Control as BasicControl;
                if (bc != null)
                    bc.LinkClicked -= new MDTEventHandler(bc_LinkClicked);
            }
            _items.Clear();
        }
        public virtual EffectableControl GetAt(int index)
        {
            return _items[index];
        }

        protected BasicListEffect listEffect;

        public BasicListEffect ListEffect
        {
            get { return listEffect; }
        }

        protected BasicEffect listItemEffect;

        public BasicEffect ListItemEffect
        {
            get { return listItemEffect; }
        }

        public override void ChangeEffect(string propertyName, Type effectType, EffectableControl owner)
        {
            base.ChangeEffect(propertyName, effectType, owner);
            if (propertyName == "ListEffect")
            {
                if (listEffect != null)
                    listEffect.DetachEffect();
                ConstructorInfo ci = effectType.GetConstructor(new Type[] { typeof(BasicListControl) });
                listEffect = (BasicListEffect)ci.Invoke(new object[] { this });
            }
            else if (propertyName == "ListItemEffect")
            {
                ConstructorInfo ci = effectType.GetConstructor(new Type[] { typeof(EffectableControl) });
                listItemEffect = (BasicEffect)ci.Invoke(new object[] { new EffectableControl(new TextBlock()) });
                foreach (EffectableControl ec in _items)
                {
                    ec.ChangeEffect("MainEffect", effectType);
                }
            }
        }
        
        public BusyIndicator Container = new BusyIndicator();

        public BasicListControl()
            : base()
        {
            effectPropertyNameList.Add("ListEffect");
            effectPropertyNameList.Add("ListItemEffect");
            
            Content = Container;
        }
    }
}
