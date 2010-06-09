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

namespace BasicLibrary
{
    public class ListDataSource
    {
        public enum DataSourceType
        {
            XML,
            MYSQL
        }

        DataSourceType _SourceType;

        public DataSourceType SourceType
        {
            get { return _SourceType; }
            set { _SourceType = value; }
        }

        string _XmlURL;

        public string XmlURL
        {
            get { return _XmlURL; }
            set { _XmlURL = value; }
        }

        string elementName;

        public string ElementName
        {
            get { return elementName; }
            set { elementName = value; }
        }

        string _server;

        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }
        string _username;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        string _db;

        public string Db
        {
            get { return _db; }
            set { _db = value; }
        }
        string _table;

        public string Table
        {
            get { return _table; }
            set { _table = value; }
        }
    }
}
