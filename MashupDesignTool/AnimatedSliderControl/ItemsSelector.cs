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
using System.Collections.ObjectModel;

namespace AnimatedSliderControl
{
    public class ItemsSelector: ItemsControl
    {

        #region private members

        protected ObservableCollection<ISelectable> _selectableItems;
        private SelectionManager _selectionManager;
        public event EventHandler<SelectionChangedEventArgs> SelectionChange;

        #endregion

        public ItemsSelector()
        {
            _selectableItems = new ObservableCollection<ISelectable>();
            _selectionManager = new SelectionManager();

            _selectionManager.SetCollectionToManage( _selectableItems );
            _selectionManager.SelectionChange += new EventHandler<SelectionChangedEventArgs>( _selectManager_SelectionChange );
        }

        void _selectManager_SelectionChange( object sender, SelectionChangedEventArgs e )
        {
            if( this.SelectionChange != null )
                this.SelectionChange( this, e );
        }

        #region overridden methods

        protected override void PrepareContainerForItemOverride( DependencyObject element, object item )
        {
            base.PrepareContainerForItemOverride( element, item );
            ISelectable item2 = element as ISelectable;
            if ( item2 != null )
            {
                this._selectableItems.Add( item2 );
            }
        }

        protected override void ClearContainerForItemOverride( DependencyObject element, object item )
        {
            base.ClearContainerForItemOverride( element, item );
            ISelectable item2 = element as ISelectable;
            this._selectableItems.Remove(  item2  );
        }

        #endregion
    }
    
}
