using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BasicLibrary;

namespace ItemCollectionEditor
{
    public partial class ImageListEditor : UserControl
    {
        FloatableWindow f;

        Panel root;

        public Panel Root
        {
            get { return root; }
            set
            {
                root = value;
                f.ParentLayoutRoot = root;
            }
        }

        public ImageListEditor()
        {
            InitializeComponent();
            f = new FloatableWindow();
            f.Height = 300;
        }

        BasicImageListControl listControl;
        List<ImageListControlItems> listControlItems = new List<ImageListControlItems>();
        public ImageListEditor(Panel p, BasicImageListControl list)
            : this()
        {
            listControl = list;
            root = p;
            f.ParentLayoutRoot = root;
            f.Content = this;
            f.HasCloseButton = false;
            f.Title = "Image list editor";
            f.Width = 600;
            f.Height = 400;
            foreach (ImageListControlItems ec in list.Items)
            {
                Image img = new Image();
                Image temp = ec.Content as Image;
                img.Source = temp.Source;
                listBox.Items.Add(img);
                listControlItems.Add(ec);
            }
            if (listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;
        }

        public void Close()
        {
            f.Close();
        }

        public void Show()
        {
            f.Show(150, 150);
        }
        public void ShowDialog()
        {
            f.ShowDialog();
        }
        public void Show(double horizontalOffset, double verticalOffset)
        {
            f.Show(horizontalOffset, verticalOffset);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            Image img = e.AddedItems[0] as Image;
            imgPreview.Source = img.Source;
            txtURL.Text = listControlItems[listBox.SelectedIndex].ImageUrl;
        }
       
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            listControl.RemoveAllItem();
            foreach (EffectableControl ec in listControlItems)
                listControl.AddItem(ec);

            f.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            f.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            listControl.RemoveItemAt(listBox.SelectedIndex);
            listBox.Items.RemoveAt(listBox.SelectedIndex);            
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            listControl.RemoveAllItem();
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            object temp = listBox.SelectedItem;
            int selectedIndex = listBox.SelectedIndex;
            listBox.Items.RemoveAt(selectedIndex);
            selectedIndex--;
            listBox.Items.Insert(selectedIndex, temp);
            listBox.SelectedIndex = selectedIndex;

            listControl.SwapItem(selectedIndex, selectedIndex + 1);
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            object temp = listBox.SelectedItem;
            int selectedIndex = listBox.SelectedIndex;
            listBox.Items.RemoveAt(selectedIndex);
            selectedIndex++;
            listBox.Items.Insert(selectedIndex, temp);
            listBox.SelectedIndex = selectedIndex;

            listControl.SwapItem(selectedIndex - 1, selectedIndex);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ImageListControlItems item = new ImageListControlItems();
            listControl.AddItem(item);
            Image img = new Image();
            Image temp = item.Content as Image;
            img.Source = temp.Source;
            listBox.Items.Add(img);
        }

        private void txtURL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ImageListControlItems item = listControl.GetAt(listBox.SelectedIndex) as ImageListControlItems;
                item.ImageUrl = txtURL.Text;
                Image temp = item.Content as Image;
                Image temp1 = listBox.SelectedItem as Image;
                temp1.Source = temp.Source;                
            }
        }

        private void txtTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ImageListControlItems item = listControl.GetAt(listBox.SelectedIndex) as ImageListControlItems;
                item.Title = txtTitle.Text;             
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ImageListControlItems item = listControl.GetAt(listBox.SelectedIndex) as ImageListControlItems;
                item.Description = txtDescription.Text;
            }
        }

    }
}
