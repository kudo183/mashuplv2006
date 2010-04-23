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
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;

namespace AnimatedSliderControl
{
    public interface ISelectable
    {
        event EventHandler Selected;
        void Select();
        void Deselect();
    }

    public class SelectionChangedEventArgs : EventArgs
    {
        public SelectionChangedEventArgs( int indexOfSelectedItem )
        {
            this.selectedItemIndex = indexOfSelectedItem;
        }
        public int selectedItemIndex;
    }

    public class SelectionManager
    {
        private ObservableCollection<ISelectable> items;
        public event EventHandler<SelectionChangedEventArgs> SelectionChange;
        private int _lastSelected = 0;
        private int _next = 0;

        public void SetCollectionToManage( ObservableCollection<ISelectable> items )
        {
            if ( this.items != null )
                if ( this.items != items )
                    foreach ( ISelectable s in this.items )
                        s.Selected -= new EventHandler( OnItemSelected );
            this.items = items;
            items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler( CollectionChanged );
            foreach ( ISelectable current in this.items )
            {
                current.Selected += new EventHandler( OnItemSelected );
                current.Deselect();
            }
        }

        void CollectionChanged( object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e )
        {
            if ( e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace )
            {
                foreach ( ISelectable newItem in e.OldItems )
                {
                    newItem.Selected -= new EventHandler( OnItemSelected );
                }
            }
            if ( e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace )
                foreach ( ISelectable newItem in e.NewItems )
                {
                    newItem.Selected += new EventHandler( OnItemSelected );
                    newItem.Deselect();
                }
        }

        public void OnItemSelected( object sender, EventArgs e )
        {
            _next = this.items.IndexOf( sender as ISelectable );
            if ( _lastSelected != _next )
                this.items[ _lastSelected ].Deselect();
            _lastSelected = _next;
            if ( this.SelectionChange != null )
                this.SelectionChange( sender, new SelectionChangedEventArgs( _lastSelected ) );
        }
    }
}
