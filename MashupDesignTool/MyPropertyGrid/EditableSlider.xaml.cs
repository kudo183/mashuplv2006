using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EasyPainter.Imaging.Silverlight
{
    public partial class EditableSlider : UserControl
    {

        private bool _editOnClickSetting = false;
        public delegate void ValueChangedHandler(double newValue);

        public event ValueChangedHandler ValueChanged;

        bool _isInitialized = false;
        public EditableSlider()
        {
            InitializeComponent();
            _isInitialized = true;

            this.SizeChanged += new SizeChangedEventHandler(EditableSlider_SizeChanged);
        }

        void EditableSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isInitialized) return;
            UpdateView();
        }

        /// <summary>
        /// True if dragging via mouse, false otherwise
        /// </summary>
        private bool _isDragging = false;

        /// <summary>
        /// True if the textbox is enabled and being edited, false otherwise
        /// </summary>
        private bool _isInEditMode = false;

        /// <summary>
        /// True if the user started dragging, false otherwise
        /// </summary>
        private bool _hasBeenDragging = false;

        private double _minimum = 0;
        private double _maximum = 10;
        private double _value = 3;
        private string _displayFormat = "0";
        private bool _usingCustomDisplayFormat = false;
        private bool isFloatingPoint = false;

        public bool IsFloatingPoint
        {
            get { return isFloatingPoint; }
            set
            {
                isFloatingPoint = value;

                _displayFormat = (value == true) ? "0.00" : "0";
            }
        }
        public double Minimum
        {
            get { return _minimum; }
            set { _minimum = value; UpdateView(); }
        }

        public double Maximum
        {
            get { return _maximum; }
            set { _maximum = value; UpdateView(); }
        }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                UpdateView();
                UpdateText();
                if (ValueChanged != null) ValueChanged(_value);
            }
        }

        public string DisplayFormat
        {
            get { return _displayFormat; }
            set
            {
                _usingCustomDisplayFormat = true;
                if (_displayFormat == value) return;
                _displayFormat = value;
                UpdateText();
            }
        }

        private void rectBase_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rectBase.CaptureMouse();
            _isDragging = true;
            _hasBeenDragging = false;
            oldXPos = e.GetPosition(rectBase).X;
        }

        private void rectBase_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            if (_editOnClickSetting)
            {
                _isDragging = false;
                if (!_hasBeenDragging)
                {
                    EnterEditMode();
                }
            }
            else
            {
                if (_isDragging)
                {
                    UpdatePercent(e.GetPosition(rectBase).X);
                }
                _isDragging = false;
            }
        }

        double oldXPos = 0;
        int step = 1;
        private void rectBase_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                UpdatePercent(e.GetPosition(rectBase).X);

                _hasBeenDragging = true;
            }
        }

        private void UpdatePercent(double xPos)
        {
            UpdateRectPercent(xPos);

            //if (xPos < 0) xPos = 0;
            //if (xPos > rectBase.ActualWidth) xPos = rectBase.ActualWidth;
            //rectPercent.Width = xPos;
            //double valuePercent = xPos / rectBase.ActualWidth;
            //Value = (_maximum - _minimum) * valuePercent + _minimum;
        }

        private void UpdateRectPercent(double xPos)
        {
            if (oldXPos == xPos)
                return;

            //step = (int)Math.Round(xPos - oldXPos);

            if (oldXPos < xPos && Value < Maximum)
                Value = Value + step;
            else if (oldXPos > xPos && Value > Minimum)
                Value = Value - step;

            oldXPos = xPos;
        }

        private void ChangeDisplayFormatIfNeeded(string newFormat)
        {
            if (_usingCustomDisplayFormat) return;
            if (_displayFormat == newFormat) return;
            _displayFormat = newFormat;
            UpdateText();
        }

        private void UpdateView()
        {
            double valuePercent = (_value - _minimum) / (_maximum - _minimum);
            rectPercent.Width = rectBase.ActualWidth * valuePercent;

            double range = Math.Abs(_maximum - _minimum);
            //if (range >= 40) {
            //    ChangeDisplayFormatIfNeeded("0");
            //}
            //else 
            //if (range >= 10) {
            //    ChangeDisplayFormatIfNeeded("0.0");
            //}
            //if (range >= 0.5) {
            //    ChangeDisplayFormatIfNeeded("0.00");
            //}
            //else {
            //    ChangeDisplayFormatIfNeeded("0.0000");
            //}
        }

        private void UpdateText()
        {
            textValue.Text = _value.ToString(_displayFormat);
            textValueEdit.Text = textValue.Text;
        }

        private void EnterEditMode()
        {
            _isInEditMode = true;
            textValueEdit.Visibility = Visibility.Visible;
            textValue.Visibility = Visibility.Collapsed;
            textValueEdit.Focus();
            textValueEdit.SelectAll();
        }

        public void ExitEditMode()
        {
            _isInEditMode = false;
            textValueEdit.Visibility = Visibility.Collapsed;
            textValue.Visibility = Visibility.Visible;
            double result;
            if (double.TryParse(textValueEdit.Text, out result))
            {
                if (result < _minimum) result = _minimum;
                if (result > _maximum) result = _maximum;
                if (isFloatingPoint == true)
                    Value = result;
                else
                    Value = (int)result;
            }
            else
            {
                UpdateText();
            }
        }

        private void textValueEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!_isOverEditValueButton)
            {
                ExitEditMode();
            }
        }

        private void textValueEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExitEditMode();
            }
        }

        private void btnEditValue_Click(object sender, RoutedEventArgs e)
        {
            if (_isInEditMode) ExitEditMode();
            else EnterEditMode();
        }

        bool _isOverEditValueButton = false;
        private void btnEditValue_MouseEnter(object sender, MouseEventArgs e)
        {
            _isOverEditValueButton = true;
        }

        private void btnEditValue_MouseLeave(object sender, MouseEventArgs e)
        {
            _isOverEditValueButton = false;
        }
    }
}
