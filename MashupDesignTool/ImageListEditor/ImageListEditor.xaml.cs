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
        bool isCancelled = true;
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
        List<EffectableControl> listControlItems = new List<EffectableControl>();
        //List<ImageListControlItems> listControlItems = new List<ImageListControlItems>();
        public ImageListEditor(Panel p, BasicImageListControl list)
            : this()
        {
            listControl = list;
            root = p;
            f.ParentLayoutRoot = root;
            f.Content = this;
            //f.HasCloseButton = false;
            f.Title = "Image list editor";
            f.Width = 600;
            f.Height = 400;
            f.Closed += new EventHandler(f_Closed);
            foreach (EffectableControl ec in list.Items)
            {
                ImageListControlItems item = ec.Control as ImageListControlItems;
                listBox.Items.Add(MakeListBoxItem(item));
                //listControlItems.Add(ec.Control as ImageListControlItems);
                listControlItems.Add(ec);
            }
            if (listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;
        }

        void f_Closed(object sender, EventArgs e)
        {
            if (isCancelled == false)
                return;
            listControl.RemoveAllItem();
            foreach (EffectableControl ec in listControlItems)
                listControl.AddItem(ec);
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
            Border b = e.AddedItems[0] as Border;
            ImageListControlItems item = b.Child as ImageListControlItems;
            
            previewImg.Source = item.Img.Source;
            previewTitle.Text = item.Title;

            txtURL.Text = item.ImageUrl;            
            txtDescription.Text = item.Description;
            txtTitle.Text = item.Title;
            txtLink.Text = item.Link;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            f.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            isCancelled = false;
            f.Close();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int temp = listBox.SelectedIndex;
            listControl.RemoveItemAt(listBox.SelectedIndex);
            listBox.Items.RemoveAt(listBox.SelectedIndex);
            if (temp == listBox.Items.Count && temp > 0)
                temp--;
            listBox.SelectedIndex = temp;
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            listControl.RemoveAllItem();
        }

        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex == 0)
                return;
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
            if (listBox.SelectedIndex == listBox.Items.Count - 1)
                return;
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
            item.TitleColor = listControl.TitleColor;
            item.TitleFontFamily = listControl.TitleFontFamily;
            item.TitleSize = listControl.TitleSize;
            item.IsShowTitle = listControl.IsShowTitle;
            listControl.InsertItem(listBox.SelectedIndex + 1, new EffectableControl(item));

            listBox.Items.Insert(listBox.SelectedIndex + 1, MakeListBoxItem(item));
        }

        private void txtURL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EffectableControl ec = listControl.GetAt(listBox.SelectedIndex) as EffectableControl;
                ImageListControlItems item = ec.Control as ImageListControlItems;
                item.ImageUrl = txtURL.Text;
                Image temp = item.Img as Image;
                previewImg.Source = temp.Source;

                ImageListControlItems temp1 = ((listBox.SelectedItem as Border).Child as ImageListControlItems);
                temp1.ImageUrl = txtURL.Text;
            }
        }

        private void txtTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EffectableControl ec = listControl.GetAt(listBox.SelectedIndex) as EffectableControl;
                ImageListControlItems item = ec.Control as ImageListControlItems;
                ((listBox.SelectedItem as Border).Child as ImageListControlItems).Title = txtTitle.Text;
                item.Title = txtTitle.Text;
                previewTitle.Text = txtTitle.Text;
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EffectableControl ec = listControl.GetAt(listBox.SelectedIndex) as EffectableControl;
                ImageListControlItems item = ec.Control as ImageListControlItems;
                ((listBox.SelectedItem as Border).Child as ImageListControlItems).Description = txtDescription.Text;
                item.Description = txtDescription.Text;
                ToolTipService.SetToolTip(previewImg, txtDescription.Text);
            }
        }

        private Border MakeListBoxItem(ImageListControlItems item)
        {
            ImageListControlItems temp = new ImageListControlItems();
            temp.Title = item.Title;
            temp.Description = item.Description;
            temp.Img.Source = item.Img.Source;
            temp.Link = item.Link;
            temp.Width = temp.Height = 130;
            temp.Margin = new Thickness(20, 0, 0, 0);
            temp.IsShowTitle = item.IsShowTitle;            
            return new Border() { Child = temp };
        }

        private void txtLink_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EffectableControl ec = listControl.GetAt(listBox.SelectedIndex) as EffectableControl;
                ImageListControlItems item = ec.Control as ImageListControlItems;
                ((listBox.SelectedItem as Border).Child as ImageListControlItems).Link = txtLink.Text;
                item.Link = txtLink.Text;
            }
        }
    }
}
