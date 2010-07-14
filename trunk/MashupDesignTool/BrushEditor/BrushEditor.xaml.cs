using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Editor
{
    public partial class BrushEditor : UserControl
    {
        public delegate void BrushChangeHandle(object sender, Brush newBrush);
        public event BrushChangeHandle BrushChanged;

        public event EventHandler Closed;

        FloatableWindow f = new FloatableWindow();
        string _strOrginBrush;
        Brush _brush;
        Stretch[] strectMode = new Stretch[] { Stretch.Fill, Stretch.None, Stretch.Uniform, Stretch.UniformToFill };
        ImageBrush img;
        
        public BrushEditor(Panel p)
        {
            InitializeComponent();

            f.ParentLayoutRoot = p;
            //f.HasCloseButton = false;
            f.OverlayBrush = new SolidColorBrush(Colors.Transparent);
            f.OverlayOpacity = 0;
            f.ResizeMode = ResizeMode.NoResize;
            f.Content = this;
            f.Title = "Brush editor";
            f.Closed += new EventHandler(f_Closed);
           
            comboBox1.ItemsSource = strectMode;
            comboBox1.SelectedIndex = 0;

            gradient = new ColorTools.GradientEditor();
            solid = new Controls.ColorPicker();

            gradient.BrushChanged += new EventHandler<ColorTools.GradientEditorEventArgs>(gradient_BrushChanged);
            solid.ColorSelected += new Controls.ColorPicker.ColorSelectedHandler(solid_ColorSelected);
            //solid.Margin = new Thickness((630 - 210) / 2, (450 - 220) / 2, 0, 0);
            solid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            solid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            solid.RenderTransformOrigin = new Point(0.5, 0.5);
            solid.RenderTransform = new ScaleTransform() { ScaleX = 1.4, ScaleY = 1.4};
            gridGradient.Children.Add(gradient);
            gridSolid.Children.Add(solid);
        }

        void f_Closed(object sender, EventArgs e)
        {
            if (isCancelled)
            {
                _brush = BasicLibrary.MyXmlSerializer.Deserialize(_strOrginBrush) as Brush;
                if (BrushChanged != null)
                    BrushChanged(this, _brush);
            }
            if (Closed != null)
                Closed(this, e);
        }

        public void ShowDialog(Brush brush)
        {
            init(brush);
            f.Width = 630;
            f.Height = 430;
            f.ShowDialog();           
        }

        bool isCancelled = true;
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //_brush = BasicLibrary.MyXmlSerializer.Deserialize(_strOrginBrush) as Brush;
            //if (BrushChanged != null)
            //    BrushChanged(this, _brush);
            f.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            isCancelled = false;
            f.Close();
        }

        ColorTools.GradientEditor gradient;
        Controls.ColorPicker solid;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void init(Brush brush)
        {
            _strOrginBrush = BasicLibrary.MyXmlSerializer.Serialize(brush);
            _brush = brush;

            rectImagePreview.Fill = null;
            textBox1.Text = "";
            comboBox1.SelectedIndex = 0;

            if (_brush.GetType() == typeof(SolidColorBrush))
            {
                solid.SelectedColor = (_brush as SolidColorBrush).Color;
            }
            else if (_brush.GetType() == typeof(GradientBrush))
            {
                gradient.CurrentBrush = _brush as GradientBrush;
            }
            else if (_brush.GetType() == typeof(ImageBrush))
            {
                ImageBrush imgBrush = _brush as ImageBrush;
                if (imgBrush != null)
                {
                    BitmapImage bm = imgBrush.ImageSource as BitmapImage;
                    comboBox1.SelectedItem = imgBrush.Stretch;
                    rectImagePreview.Fill = imgBrush;
                    if (bm != null)
                    {
                        textBox1.Text = bm.UriSource.ToString();
                    }
                    else
                        textBox1.Text = "";
                }                
            }
        }

        void solid_ColorSelected(Color c)
        {
            _brush = new SolidColorBrush(c);
            if (BrushChanged != null)
                BrushChanged(this, _brush);
        }

        void gradient_BrushChanged(object sender, ColorTools.GradientEditorEventArgs e)
        {
            _brush = e.SelectedBrush;
            if (BrushChanged != null)
                BrushChanged(this, _brush);
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    img = new ImageBrush();
                    img.ImageSource = new BitmapImage(new Uri(textBox1.Text, UriKind.RelativeOrAbsolute));
                    img.Stretch = strectMode[comboBox1.SelectedIndex];
                    _brush = img;
                    rectImagePreview.Fill = _brush;
                    if (BrushChanged != null)
                        BrushChanged(this, _brush);
                }
                catch { }
            }
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (img != null)
            {
                img.Stretch = strectMode[comboBox1.SelectedIndex];
            }
        }
    }
}
