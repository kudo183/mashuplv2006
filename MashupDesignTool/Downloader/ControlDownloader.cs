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
using System.Linq;
using System.Reflection;

namespace MashupDesignTool
{
    public class ControlDownloader
    {
        public delegate void DownloadControlCompletedHandler(ControlInfo ci, Assembly assembly);
        public event DownloadControlCompletedHandler DownloadControlCompleted;

        public delegate void DownloadCompletedHandler();
        public event DownloadCompletedHandler DownloadCompleted;

        private string clientRoot;
        Dictionary<string, Assembly> LoadedAssembly = new Dictionary<string, Assembly>();
        Dictionary<string, Assembly> LoadingAssembly = new Dictionary<string, Assembly>(); 
        private List<string> downloadedDllFilenames = new List<string>();
        private List<string> downloadedDllReferences = new List<string>();
        Dictionary<WebClient, string> downloadingDllFilenames = new Dictionary<WebClient, string>();
        Dictionary<WebClient, string> downloadingDllReferences = new Dictionary<WebClient, string>();
        private List<ControlInfo> downloadingControlInfo = new List<ControlInfo>();
        private List<string> dllFilenames, dllReferences;
        private int count;
        
        public ControlDownloader()
        {
            string absoluteUri = Application.Current.Host.Source.AbsoluteUri;
            int lastSlash = absoluteUri.LastIndexOf("/");
            clientRoot = absoluteUri.Substring(0, lastSlash + 1);
            MessageBox.Show(clientRoot);
        }

        public void Download(List<string> dllFilenames, List<string> dllReferences)
        {
            this.dllFilenames = dllFilenames;
            this.dllReferences = dllReferences;
            count = 0;

            string assemblyPath = clientRoot + DownloadArgs.ControlReferenceDllFolder;
            foreach (string str in dllReferences)
            {
                if (!downloadedDllReferences.Contains(str))
                {
                    Uri uri = new Uri(assemblyPath + str, UriKind.Absolute);
                    //Start an async download:
                    WebClient webClient = new WebClient();
                    webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
                    webClient.OpenReadAsync(uri);
                    downloadingDllReferences.Add(webClient, str);
                }
                else
                    count++;
            }

            if (count == dllReferences.Count)
            {
                count = 0;
                assemblyPath = clientRoot + DownloadArgs.ControlDllFolder;
                foreach (string str in dllFilenames)
                {
                    if (!downloadedDllFilenames.Contains(str))
                    {
                        Uri uri = new Uri(assemblyPath + str, UriKind.Absolute);
                        //Start an async download:
                        WebClient webClient = new WebClient();
                        webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted1);
                        webClient.OpenReadAsync(uri);
                        downloadingDllFilenames.Add(webClient, str);
                    }
                    else
                        count++;
                }

                if (count == dllFilenames.Count)
                {
                    if (DownloadCompleted != null)
                        DownloadCompleted();
                }
            }
        }

        void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string dllReference = downloadingDllReferences[(WebClient)sender];
                downloadedDllReferences.Add(dllReference);
                downloadingDllReferences.Remove((WebClient)sender);
                AssemblyPart assemblyPart = new AssemblyPart();
                Assembly assembly = assemblyPart.Load(e.Result);
                count++;

                if (count == dllReferences.Count)
                {
                    count = 0;
                    string assemblyPath = clientRoot + DownloadArgs.ControlDllFolder;
                    foreach (string str in dllFilenames)
                    {
                        if (!downloadedDllFilenames.Contains(str))
                        {
                            Uri uri = new Uri(assemblyPath + str, UriKind.Absolute);
                            //Start an async download:
                            WebClient webClient = new WebClient();
                            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted1);
                            webClient.OpenReadAsync(uri);
                            downloadingDllFilenames.Add(webClient, str);
                        }
                        else
                            count++;
                    }
                }
            }
        }

        void webClient_OpenReadCompleted1(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string dllFilename = downloadingDllFilenames[(WebClient)sender];
                downloadedDllFilenames.Add(dllFilename);
                downloadingDllFilenames.Remove((WebClient)sender);
                AssemblyPart assemblyPart = new AssemblyPart();
                Assembly assembly = assemblyPart.Load(e.Result);
                LoadedAssembly.Add(dllFilename, assembly);
                count++;

                if (count == dllFilenames.Count)
                {
                    if (DownloadCompleted != null)
                        DownloadCompleted();
                }
            }
        }

        public Assembly GetAssembly(ControlInfo ci)
        {
            return GetAssembly(ci.DllFilename);
        }

        public Assembly GetAssembly(string dllFilename)
        {
            if (LoadedAssembly.ContainsKey(dllFilename))
                return LoadedAssembly[dllFilename];
            return null;
        }

        public void DownloadControl(ControlInfo ci)
        {
            downloadingControlInfo.Add(ci);

            String assemblyPath = clientRoot + DownloadArgs.ControlDllFolder + ci.DllFilename;
            if (!downloadedDllFilenames.Contains(ci.DllFilename))
            {
                if (!downloadingDllFilenames.ContainsValue(ci.DllFilename))
                {
                    Uri uri = new Uri(assemblyPath, UriKind.Absolute);
                    //Start an async download:
                    WebClient webClient = new WebClient();
                    webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadControlCompleted);
                    webClient.OpenReadAsync(uri);
                    downloadingDllFilenames.Add(webClient, ci.DllFilename);
                }
            }
            else
                ci.IsDllFileDownloaded = true;

            assemblyPath = clientRoot + DownloadArgs.ControlReferenceDllFolder;
            for (int i = 0; i < ci.DllReferences.Count; i++)
            {
                if (!downloadedDllReferences.Contains(ci.DllReferences[i]))
                {
                    if (!downloadingDllReferences.ContainsValue(ci.DllReferences[i]))
                    {
                        Uri uri = new Uri(assemblyPath + ci.DllReferences[i], UriKind.Absolute);
                        //Start an async download:
                        WebClient webClient = new WebClient();
                        webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadDllDependenceCompleted);
                        webClient.OpenReadAsync(uri);
                        downloadingDllReferences.Add(webClient, ci.DllReferences[i]);
                    }
                }
                else
                    ci.IsDllReferencesDownloaded[i] = true;
            }

            if (ci.IsReady)
            {
                if (DownloadControlCompleted != null)
                    DownloadControlCompleted(ci, LoadedAssembly[ci.DllFilename]);
                return;
            }
        }

        private void webClient_DownloadControlCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    string dllFilename = downloadingDllFilenames[(WebClient)sender];
                    downloadedDllFilenames.Add(dllFilename);
                    downloadingDllFilenames.Remove((WebClient)sender);
                    AssemblyPart assemblyPart = new AssemblyPart();
                    Assembly assembly = assemblyPart.Load(e.Result);

                    for (int i = downloadingControlInfo.Count - 1; i >= 0; i--)
                    {
                        if (downloadingControlInfo[i].DllFilename == dllFilename)
                        {
                            downloadingControlInfo[i].IsDllFileDownloaded = true;
                            if (downloadingControlInfo[i].IsReady)
                            {
                                if (!LoadedAssembly.ContainsKey(dllFilename)) 
                                    LoadedAssembly.Add(dllFilename, assembly);
                                if (DownloadControlCompleted != null)
                                    DownloadControlCompleted(downloadingControlInfo[i], assembly);
                                downloadingControlInfo.RemoveAt(i);
                            }
                            else
                                LoadingAssembly.Add(dllFilename, assembly);
                        }
                    }
                }
            }
            catch { }
        }

        private void webClient_DownloadDllDependenceCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    string dll = downloadingDllReferences[(WebClient)sender];
                    downloadedDllReferences.Add(dll);
                    downloadingDllReferences.Remove((WebClient)sender);
                    AssemblyPart assemblyPart = new AssemblyPart();
                    Assembly assembly = assemblyPart.Load(e.Result);

                    for (int i = downloadingControlInfo.Count - 1; i >= 0; i--)
                    {
                        string dllFilename = downloadingControlInfo[i].DllFilename;
                        downloadingControlInfo[i].CheckDllReferences(dll);
                        if (downloadingControlInfo[i].IsReady)
                        {
                            if (!LoadedAssembly.ContainsKey(dllFilename)) 
                                LoadedAssembly.Add(dllFilename, LoadingAssembly[dllFilename]);
                            LoadingAssembly.Remove(dllFilename);
                            if (DownloadControlCompleted != null)
                                DownloadControlCompleted(downloadingControlInfo[i], LoadedAssembly[dllFilename]);
                            downloadingControlInfo.RemoveAt(i);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
