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

namespace MenuEditor
{
    public partial class MenuEditor : UserControl
    {
        FloatableWindow f = new FloatableWindow();
        BasicLibrary.Menu.BasicMenu _menu;
        BasicLibrary.Menu.BasicMenu _tempMenu;
        public MenuEditor(Panel p, BasicLibrary.Menu.BasicMenu menu)
        {
            InitializeComponent();
            _menu = menu;
            _tempMenu = Activator.CreateInstance(menu.GetType()) as BasicLibrary.Menu.BasicMenu;
            _tempMenu.XmlString = menu.XmlString;
            _tempMenu.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _tempMenu.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            textXmlString.Text = menu.XmlString ?? "";
            border1.Child = _tempMenu;
            f.ParentLayoutRoot = p;
            f.Content = this;
            f.Title = "Menu editor";
        }
        public void ShowDialog()
        {
            f.Width = f.Height = 500;
            f.ShowDialog();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _menu.XmlString = textXmlString.Text;
            f.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            f.Close();
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            _tempMenu.XmlString = textXmlString.Text;
        }

        private void textXmlString_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.PlatformKeyCode == 190)//>
            {
                int begin = textXmlString.Text.LastIndexOf('<', textXmlString.SelectionStart - 1);
                int end = textXmlString.Text.IndexOfAny(new char[] { ' ', '>' }, begin);
                int temp = textXmlString.SelectionStart;
                textXmlString.SelectedText = "</" + textXmlString.Text.Substring(begin + 1, end - begin - 1) + ">";
                textXmlString.SelectionStart = temp;
                return;
            }
            if (e.PlatformKeyCode == 187)//=
            {
                textXmlString.SelectedText = "\"\"";
                textXmlString.SelectionStart = textXmlString.SelectionStart - 1;
                return;
            }
            if (e.PlatformKeyCode == 222)//"
            {
                
            }
        }
    }
}
