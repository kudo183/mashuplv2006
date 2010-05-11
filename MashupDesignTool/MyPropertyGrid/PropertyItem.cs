
namespace SL30PropertyGrid
{
    #region Using Directives
    using System;
    using System.Windows;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using SL30PropertyGrid.Converters;
    #endregion

    #region PropertyItem
    /// <summary>
    /// PropertyItem hold a reference to an individual property in the propertygrid
    /// </summary>
    public sealed class PropertyItem : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        /// Event raised when an error is encountered attempting to set the Value
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ValueError;
        /// <summary>
        /// Raises the ValueError event
        /// </summary>
        /// <param name="ex">The exception</param>
        private void OnValueError(Exception ex)
        {
            if (null != ValueError)
                ValueError(this, new ExceptionEventArgs(ex));
        }
        #endregion

        #region Fields
        private PropertyInfo _propertyInfo;
        private object _instance;
        private bool _readOnly = false;
        private bool _attached = false;
        private string _name;
        private MethodInfo _get, _set;
        private Type _propertyType;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        public PropertyItem(object instance, object parent, object value, PropertyInfo property, bool readOnly, bool attached, string name, MethodInfo g, MethodInfo s)
        {
            _instance = instance;
            _parent = parent;
            _propertyInfo = property;
            _value = value;
            _readOnly = readOnly;

            _attached = attached;
            _name = name;
            _get = g;
            _set = s;
            _propertyType = (value == null) ? null : value.GetType();

            if (instance is INotifyPropertyChanged)
                ((INotifyPropertyChanged)instance).PropertyChanged += new PropertyChangedEventHandler(PropertyItem_PropertyChanged);
        }

        void PropertyItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.Name)
                Value = _propertyInfo.GetValue(_instance, null);
        }
        #endregion

        #region Properties
        public object Instant
        {
            get { return _instance; }
        }

        private object _parent;

        public string Name
        {
            get
            {
                if (_attached == true)
                {
                    return _name;
                }
                return _propertyInfo.Name;
            }
        }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    DisplayNameAttribute attr = GetAttribute<DisplayNameAttribute>(_propertyInfo);
                    _displayName = (attr != null) ? attr.DisplayName : Name;
                }

                return _displayName;
            }
        } private string _displayName;

        public string Category
        {
            get
            {
                if (string.IsNullOrEmpty(_category))
                {
                    if (_attached == true)
                        _category = "Attached property";
                    else
                    {
                        CategoryAttribute attr = GetAttribute<CategoryAttribute>(_propertyInfo);
                        if (attr != null && !string.IsNullOrEmpty(attr.Category))
                            _category = attr.Category;
                        else
                            _category = "Misc";
                    }
                }
                return this._category;
            }
        } private string _category;

        public object Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;
                object originalValue = _value;
                _value = value;
                Type propertyType;

                if (_attached == true)
                {
                    propertyType = _propertyType;
                    if (propertyType == null)
                    {
                        OnValueError(new Exception("attached property type null. Cannot dedect type."));
                        return;
                    }
                    if (propertyType.IsEnum)
                    {
                        object val = Enum.Parse(propertyType, value.ToString(), false);
                        _set.Invoke(null, new object[] { _instance, val });
                        OnPropertyChanged("Value");
                    }
                    else
                    {
                        try
                        {
                            TypeConverter tc = TypeConverterHelper.GetConverter(propertyType);
                            if (tc != null)
                            {
                                object val = tc.ConvertFrom(value);
                                _set.Invoke(null, new object[] { _instance, val });
                                OnPropertyChanged("Value");
                            }
                            else
                            {
                                // try direct setting as a string...
                                _set.Invoke(null, new object[] { _instance, value.ToString() });
                                OnPropertyChanged("Value");
                            }
                        }
                        catch (Exception ex)
                        {
                            _value = originalValue;
                            OnPropertyChanged("Value");
                            OnValueError(ex);
                        }
                    }
                    return;
                }

                try
                {
                    propertyType = this._propertyInfo.PropertyType;
                    if (((propertyType == typeof(object)) || ((value == null) && propertyType.IsClass)) || ((value != null) && propertyType.IsAssignableFrom(value.GetType())))
                    {
                        _propertyInfo.SetValue(_instance, value, (BindingFlags.NonPublic | BindingFlags.Public), null, null, null);
                        OnPropertyChanged("Value");
                    }
                    else
                    {
                        try
                        {
                            if (propertyType.IsEnum)
                            {
                                object val = Enum.Parse(_propertyInfo.PropertyType, value.ToString(), false);
                                _propertyInfo.SetValue(_instance, val, (BindingFlags.NonPublic | BindingFlags.Public), null, null, null);
                                OnPropertyChanged("Value");
                            }
                            else
                            {
                                TypeConverter tc = TypeConverterHelper.GetConverter(propertyType);
                                if (tc != null)
                                {
                                    object convertedValue = tc.ConvertFrom(value);
                                    _propertyInfo.SetValue(_instance, convertedValue, null);
                                    OnPropertyChanged("Value");
                                }
                                else
                                {
                                    // try direct setting as a string...
                                    _propertyInfo.SetValue(_instance, value.ToString(), (BindingFlags.NonPublic | BindingFlags.Public), null, null, null);
                                    OnPropertyChanged("Value");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _value = originalValue;
                            OnPropertyChanged("Value");
                            OnValueError(ex);
                        }
                    }
                }
                catch (MethodAccessException mex)
                {
                    _value = originalValue;
                    _readOnly = true;
                    OnPropertyChanged("Value");
                    OnPropertyChanged("CanWrite");
                    OnValueError(mex);
                }
            }
        } private object _value;

        public Type PropertyType
        {
            //get { return _propertyInfo.PropertyType; }
            get
            {
                //object val = _propertyInfo.GetValue(_instance, null);
                //if (val == null)
                //    return _propertyInfo.PropertyType;
                //return val.GetType();

                if (_value == null)
                {
                    if (_propertyInfo == null)
                        return null;
                    return _propertyInfo.PropertyType;
                }
                return _value.GetType();
            }
        }

        public bool CanWrite
        {
            get
            {
                if (_propertyInfo == null)
                    return !_readOnly;
                return _propertyInfo.CanWrite && !_readOnly;
            }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
            internal set { _readOnly = value; }
        }

        #endregion

        #region Helpers
        public static T GetAttribute<T>(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return default(T);
            var attributes = propertyInfo.GetCustomAttributes(typeof(T), true);
            return (attributes.Length > 0) ? attributes.OfType<T>().First() : default(T);
        }
        public T GetAttribute<T>()
        {
            return GetAttribute<T>(_propertyInfo);
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException("propertyName");
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void updateValue()
        {
            if (_attached == true)
            {
                FrameworkElement fe = _instance as FrameworkElement;
                while (fe != null && fe.Parent != _parent)
                {
                    fe = fe.Parent as FrameworkElement;
                }
                _value = _get.Invoke(null, new object[] {fe });
                return;
            }
            _value = _propertyInfo.GetValue(_instance, null);
        }
    }
    #endregion
}
