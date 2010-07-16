﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 4.0.50401.0
// 
namespace Present.DataService {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="DesignedApplicationData", Namespace="http://schemas.datacontract.org/2004/07/WebServer.Web")]
    public partial class DesignedApplicationData : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string ApplicationNameField;
        
        private System.Guid IdField;
        
        private System.Guid UserIdField;
        
        private string XmlStringField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ApplicationName {
            get {
                return this.ApplicationNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ApplicationNameField, value) != true)) {
                    this.ApplicationNameField = value;
                    this.RaisePropertyChanged("ApplicationName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid UserId {
            get {
                return this.UserIdField;
            }
            set {
                if ((this.UserIdField.Equals(value) != true)) {
                    this.UserIdField = value;
                    this.RaisePropertyChanged("UserId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string XmlString {
            get {
                return this.XmlStringField;
            }
            set {
                if ((object.ReferenceEquals(this.XmlStringField, value) != true)) {
                    this.XmlStringField = value;
                    this.RaisePropertyChanged("XmlString");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DataService.IDataService")]
    public interface IDataService {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IDataService/GetList", ReplyAction="http://tempuri.org/IDataService/GetListResponse")]
        System.IAsyncResult BeginGetList(string userName, System.AsyncCallback callback, object asyncState);
        
        System.Collections.ObjectModel.ObservableCollection<Present.DataService.DesignedApplicationData> EndGetList(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IDataService/Insert", ReplyAction="http://tempuri.org/IDataService/InsertResponse")]
        System.IAsyncResult BeginInsert(Present.DataService.DesignedApplicationData data, System.AsyncCallback callback, object asyncState);
        
        Present.DataService.DesignedApplicationData EndInsert(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IDataService/Update", ReplyAction="http://tempuri.org/IDataService/UpdateResponse")]
        System.IAsyncResult BeginUpdate(Present.DataService.DesignedApplicationData data, System.AsyncCallback callback, object asyncState);
        
        Present.DataService.DesignedApplicationData EndUpdate(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IDataService/Delete", ReplyAction="http://tempuri.org/IDataService/DeleteResponse")]
        System.IAsyncResult BeginDelete(System.Guid id, System.AsyncCallback callback, object asyncState);
        
        Present.DataService.DesignedApplicationData EndDelete(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IDataService/GetData", ReplyAction="http://tempuri.org/IDataService/GetDataResponse")]
        System.IAsyncResult BeginGetData(System.Guid id, System.AsyncCallback callback, object asyncState);
        
        Present.DataService.DesignedApplicationData EndGetData(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IDataService/GetUserIdFromName", ReplyAction="http://tempuri.org/IDataService/GetUserIdFromNameResponse")]
        System.IAsyncResult BeginGetUserIdFromName(string userName, System.AsyncCallback callback, object asyncState);
        
        System.Guid EndGetUserIdFromName(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataServiceChannel : Present.DataService.IDataService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.ObjectModel.ObservableCollection<Present.DataService.DesignedApplicationData> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.ObjectModel.ObservableCollection<Present.DataService.DesignedApplicationData>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class InsertCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public InsertCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public Present.DataService.DesignedApplicationData Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((Present.DataService.DesignedApplicationData)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UpdateCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public UpdateCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public Present.DataService.DesignedApplicationData Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((Present.DataService.DesignedApplicationData)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DeleteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public DeleteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public Present.DataService.DesignedApplicationData Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((Present.DataService.DesignedApplicationData)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public Present.DataService.DesignedApplicationData Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((Present.DataService.DesignedApplicationData)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetUserIdFromNameCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetUserIdFromNameCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Guid Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Guid)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataServiceClient : System.ServiceModel.ClientBase<Present.DataService.IDataService>, Present.DataService.IDataService {
        
        private BeginOperationDelegate onBeginGetListDelegate;
        
        private EndOperationDelegate onEndGetListDelegate;
        
        private System.Threading.SendOrPostCallback onGetListCompletedDelegate;
        
        private BeginOperationDelegate onBeginInsertDelegate;
        
        private EndOperationDelegate onEndInsertDelegate;
        
        private System.Threading.SendOrPostCallback onInsertCompletedDelegate;
        
        private BeginOperationDelegate onBeginUpdateDelegate;
        
        private EndOperationDelegate onEndUpdateDelegate;
        
        private System.Threading.SendOrPostCallback onUpdateCompletedDelegate;
        
        private BeginOperationDelegate onBeginDeleteDelegate;
        
        private EndOperationDelegate onEndDeleteDelegate;
        
        private System.Threading.SendOrPostCallback onDeleteCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetDataDelegate;
        
        private EndOperationDelegate onEndGetDataDelegate;
        
        private System.Threading.SendOrPostCallback onGetDataCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetUserIdFromNameDelegate;
        
        private EndOperationDelegate onEndGetUserIdFromNameDelegate;
        
        private System.Threading.SendOrPostCallback onGetUserIdFromNameCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public DataServiceClient() {
        }
        
        public DataServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DataServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<GetListCompletedEventArgs> GetListCompleted;
        
        public event System.EventHandler<InsertCompletedEventArgs> InsertCompleted;
        
        public event System.EventHandler<UpdateCompletedEventArgs> UpdateCompleted;
        
        public event System.EventHandler<DeleteCompletedEventArgs> DeleteCompleted;
        
        public event System.EventHandler<GetDataCompletedEventArgs> GetDataCompleted;
        
        public event System.EventHandler<GetUserIdFromNameCompletedEventArgs> GetUserIdFromNameCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Present.DataService.IDataService.BeginGetList(string userName, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetList(userName, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.ObjectModel.ObservableCollection<Present.DataService.DesignedApplicationData> Present.DataService.IDataService.EndGetList(System.IAsyncResult result) {
            return base.Channel.EndGetList(result);
        }
        
        private System.IAsyncResult OnBeginGetList(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string userName = ((string)(inValues[0]));
            return ((Present.DataService.IDataService)(this)).BeginGetList(userName, callback, asyncState);
        }
        
        private object[] OnEndGetList(System.IAsyncResult result) {
            System.Collections.ObjectModel.ObservableCollection<Present.DataService.DesignedApplicationData> retVal = ((Present.DataService.IDataService)(this)).EndGetList(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetListCompleted(object state) {
            if ((this.GetListCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetListCompleted(this, new GetListCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetListAsync(string userName) {
            this.GetListAsync(userName, null);
        }
        
        public void GetListAsync(string userName, object userState) {
            if ((this.onBeginGetListDelegate == null)) {
                this.onBeginGetListDelegate = new BeginOperationDelegate(this.OnBeginGetList);
            }
            if ((this.onEndGetListDelegate == null)) {
                this.onEndGetListDelegate = new EndOperationDelegate(this.OnEndGetList);
            }
            if ((this.onGetListCompletedDelegate == null)) {
                this.onGetListCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetListCompleted);
            }
            base.InvokeAsync(this.onBeginGetListDelegate, new object[] {
                        userName}, this.onEndGetListDelegate, this.onGetListCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Present.DataService.IDataService.BeginInsert(Present.DataService.DesignedApplicationData data, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginInsert(data, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Present.DataService.DesignedApplicationData Present.DataService.IDataService.EndInsert(System.IAsyncResult result) {
            return base.Channel.EndInsert(result);
        }
        
        private System.IAsyncResult OnBeginInsert(object[] inValues, System.AsyncCallback callback, object asyncState) {
            Present.DataService.DesignedApplicationData data = ((Present.DataService.DesignedApplicationData)(inValues[0]));
            return ((Present.DataService.IDataService)(this)).BeginInsert(data, callback, asyncState);
        }
        
        private object[] OnEndInsert(System.IAsyncResult result) {
            Present.DataService.DesignedApplicationData retVal = ((Present.DataService.IDataService)(this)).EndInsert(result);
            return new object[] {
                    retVal};
        }
        
        private void OnInsertCompleted(object state) {
            if ((this.InsertCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.InsertCompleted(this, new InsertCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void InsertAsync(Present.DataService.DesignedApplicationData data) {
            this.InsertAsync(data, null);
        }
        
        public void InsertAsync(Present.DataService.DesignedApplicationData data, object userState) {
            if ((this.onBeginInsertDelegate == null)) {
                this.onBeginInsertDelegate = new BeginOperationDelegate(this.OnBeginInsert);
            }
            if ((this.onEndInsertDelegate == null)) {
                this.onEndInsertDelegate = new EndOperationDelegate(this.OnEndInsert);
            }
            if ((this.onInsertCompletedDelegate == null)) {
                this.onInsertCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnInsertCompleted);
            }
            base.InvokeAsync(this.onBeginInsertDelegate, new object[] {
                        data}, this.onEndInsertDelegate, this.onInsertCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Present.DataService.IDataService.BeginUpdate(Present.DataService.DesignedApplicationData data, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginUpdate(data, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Present.DataService.DesignedApplicationData Present.DataService.IDataService.EndUpdate(System.IAsyncResult result) {
            return base.Channel.EndUpdate(result);
        }
        
        private System.IAsyncResult OnBeginUpdate(object[] inValues, System.AsyncCallback callback, object asyncState) {
            Present.DataService.DesignedApplicationData data = ((Present.DataService.DesignedApplicationData)(inValues[0]));
            return ((Present.DataService.IDataService)(this)).BeginUpdate(data, callback, asyncState);
        }
        
        private object[] OnEndUpdate(System.IAsyncResult result) {
            Present.DataService.DesignedApplicationData retVal = ((Present.DataService.IDataService)(this)).EndUpdate(result);
            return new object[] {
                    retVal};
        }
        
        private void OnUpdateCompleted(object state) {
            if ((this.UpdateCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.UpdateCompleted(this, new UpdateCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void UpdateAsync(Present.DataService.DesignedApplicationData data) {
            this.UpdateAsync(data, null);
        }
        
        public void UpdateAsync(Present.DataService.DesignedApplicationData data, object userState) {
            if ((this.onBeginUpdateDelegate == null)) {
                this.onBeginUpdateDelegate = new BeginOperationDelegate(this.OnBeginUpdate);
            }
            if ((this.onEndUpdateDelegate == null)) {
                this.onEndUpdateDelegate = new EndOperationDelegate(this.OnEndUpdate);
            }
            if ((this.onUpdateCompletedDelegate == null)) {
                this.onUpdateCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnUpdateCompleted);
            }
            base.InvokeAsync(this.onBeginUpdateDelegate, new object[] {
                        data}, this.onEndUpdateDelegate, this.onUpdateCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Present.DataService.IDataService.BeginDelete(System.Guid id, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginDelete(id, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Present.DataService.DesignedApplicationData Present.DataService.IDataService.EndDelete(System.IAsyncResult result) {
            return base.Channel.EndDelete(result);
        }
        
        private System.IAsyncResult OnBeginDelete(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid id = ((System.Guid)(inValues[0]));
            return ((Present.DataService.IDataService)(this)).BeginDelete(id, callback, asyncState);
        }
        
        private object[] OnEndDelete(System.IAsyncResult result) {
            Present.DataService.DesignedApplicationData retVal = ((Present.DataService.IDataService)(this)).EndDelete(result);
            return new object[] {
                    retVal};
        }
        
        private void OnDeleteCompleted(object state) {
            if ((this.DeleteCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.DeleteCompleted(this, new DeleteCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void DeleteAsync(System.Guid id) {
            this.DeleteAsync(id, null);
        }
        
        public void DeleteAsync(System.Guid id, object userState) {
            if ((this.onBeginDeleteDelegate == null)) {
                this.onBeginDeleteDelegate = new BeginOperationDelegate(this.OnBeginDelete);
            }
            if ((this.onEndDeleteDelegate == null)) {
                this.onEndDeleteDelegate = new EndOperationDelegate(this.OnEndDelete);
            }
            if ((this.onDeleteCompletedDelegate == null)) {
                this.onDeleteCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnDeleteCompleted);
            }
            base.InvokeAsync(this.onBeginDeleteDelegate, new object[] {
                        id}, this.onEndDeleteDelegate, this.onDeleteCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Present.DataService.IDataService.BeginGetData(System.Guid id, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetData(id, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Present.DataService.DesignedApplicationData Present.DataService.IDataService.EndGetData(System.IAsyncResult result) {
            return base.Channel.EndGetData(result);
        }
        
        private System.IAsyncResult OnBeginGetData(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid id = ((System.Guid)(inValues[0]));
            return ((Present.DataService.IDataService)(this)).BeginGetData(id, callback, asyncState);
        }
        
        private object[] OnEndGetData(System.IAsyncResult result) {
            Present.DataService.DesignedApplicationData retVal = ((Present.DataService.IDataService)(this)).EndGetData(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetDataCompleted(object state) {
            if ((this.GetDataCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetDataCompleted(this, new GetDataCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetDataAsync(System.Guid id) {
            this.GetDataAsync(id, null);
        }
        
        public void GetDataAsync(System.Guid id, object userState) {
            if ((this.onBeginGetDataDelegate == null)) {
                this.onBeginGetDataDelegate = new BeginOperationDelegate(this.OnBeginGetData);
            }
            if ((this.onEndGetDataDelegate == null)) {
                this.onEndGetDataDelegate = new EndOperationDelegate(this.OnEndGetData);
            }
            if ((this.onGetDataCompletedDelegate == null)) {
                this.onGetDataCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetDataCompleted);
            }
            base.InvokeAsync(this.onBeginGetDataDelegate, new object[] {
                        id}, this.onEndGetDataDelegate, this.onGetDataCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult Present.DataService.IDataService.BeginGetUserIdFromName(string userName, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetUserIdFromName(userName, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Guid Present.DataService.IDataService.EndGetUserIdFromName(System.IAsyncResult result) {
            return base.Channel.EndGetUserIdFromName(result);
        }
        
        private System.IAsyncResult OnBeginGetUserIdFromName(object[] inValues, System.AsyncCallback callback, object asyncState) {
            string userName = ((string)(inValues[0]));
            return ((Present.DataService.IDataService)(this)).BeginGetUserIdFromName(userName, callback, asyncState);
        }
        
        private object[] OnEndGetUserIdFromName(System.IAsyncResult result) {
            System.Guid retVal = ((Present.DataService.IDataService)(this)).EndGetUserIdFromName(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetUserIdFromNameCompleted(object state) {
            if ((this.GetUserIdFromNameCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetUserIdFromNameCompleted(this, new GetUserIdFromNameCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetUserIdFromNameAsync(string userName) {
            this.GetUserIdFromNameAsync(userName, null);
        }
        
        public void GetUserIdFromNameAsync(string userName, object userState) {
            if ((this.onBeginGetUserIdFromNameDelegate == null)) {
                this.onBeginGetUserIdFromNameDelegate = new BeginOperationDelegate(this.OnBeginGetUserIdFromName);
            }
            if ((this.onEndGetUserIdFromNameDelegate == null)) {
                this.onEndGetUserIdFromNameDelegate = new EndOperationDelegate(this.OnEndGetUserIdFromName);
            }
            if ((this.onGetUserIdFromNameCompletedDelegate == null)) {
                this.onGetUserIdFromNameCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetUserIdFromNameCompleted);
            }
            base.InvokeAsync(this.onBeginGetUserIdFromNameDelegate, new object[] {
                        userName}, this.onEndGetUserIdFromNameDelegate, this.onGetUserIdFromNameCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }
        
        protected override Present.DataService.IDataService CreateChannel() {
            return new DataServiceClientChannel(this);
        }
        
        private class DataServiceClientChannel : ChannelBase<Present.DataService.IDataService>, Present.DataService.IDataService {
            
            public DataServiceClientChannel(System.ServiceModel.ClientBase<Present.DataService.IDataService> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGetList(string userName, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = userName;
                System.IAsyncResult _result = base.BeginInvoke("GetList", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.ObjectModel.ObservableCollection<Present.DataService.DesignedApplicationData> EndGetList(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.ObjectModel.ObservableCollection<Present.DataService.DesignedApplicationData> _result = ((System.Collections.ObjectModel.ObservableCollection<Present.DataService.DesignedApplicationData>)(base.EndInvoke("GetList", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginInsert(Present.DataService.DesignedApplicationData data, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = data;
                System.IAsyncResult _result = base.BeginInvoke("Insert", _args, callback, asyncState);
                return _result;
            }
            
            public Present.DataService.DesignedApplicationData EndInsert(System.IAsyncResult result) {
                object[] _args = new object[0];
                Present.DataService.DesignedApplicationData _result = ((Present.DataService.DesignedApplicationData)(base.EndInvoke("Insert", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginUpdate(Present.DataService.DesignedApplicationData data, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = data;
                System.IAsyncResult _result = base.BeginInvoke("Update", _args, callback, asyncState);
                return _result;
            }
            
            public Present.DataService.DesignedApplicationData EndUpdate(System.IAsyncResult result) {
                object[] _args = new object[0];
                Present.DataService.DesignedApplicationData _result = ((Present.DataService.DesignedApplicationData)(base.EndInvoke("Update", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginDelete(System.Guid id, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = id;
                System.IAsyncResult _result = base.BeginInvoke("Delete", _args, callback, asyncState);
                return _result;
            }
            
            public Present.DataService.DesignedApplicationData EndDelete(System.IAsyncResult result) {
                object[] _args = new object[0];
                Present.DataService.DesignedApplicationData _result = ((Present.DataService.DesignedApplicationData)(base.EndInvoke("Delete", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginGetData(System.Guid id, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = id;
                System.IAsyncResult _result = base.BeginInvoke("GetData", _args, callback, asyncState);
                return _result;
            }
            
            public Present.DataService.DesignedApplicationData EndGetData(System.IAsyncResult result) {
                object[] _args = new object[0];
                Present.DataService.DesignedApplicationData _result = ((Present.DataService.DesignedApplicationData)(base.EndInvoke("GetData", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginGetUserIdFromName(string userName, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = userName;
                System.IAsyncResult _result = base.BeginInvoke("GetUserIdFromName", _args, callback, asyncState);
                return _result;
            }
            
            public System.Guid EndGetUserIdFromName(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Guid _result = ((System.Guid)(base.EndInvoke("GetUserIdFromName", _args, result)));
                return _result;
            }
        }
    }
}