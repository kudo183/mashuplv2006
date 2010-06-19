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
using System.Reflection;

namespace MashupDesignTool
{
    public class EffectDownloader
    {
        public delegate void DownloadEffectCompletedHandler(EffectInfo ei, Assembly assembly);
        public event DownloadEffectCompletedHandler DownloadEffectCompleted;

        public delegate void DownloadCompletedHandler();
        public event DownloadCompletedHandler DownloadCompleted;

        private string clientRoot;
        Dictionary<string, Assembly> LoadedAssembly = new Dictionary<string, Assembly>();
        Dictionary<string, Assembly> LoadingAssembly = new Dictionary<string, Assembly>(); 
        private List<string> downloadedDllFilenames = new List<string>();
        private List<string> downloadedDllReferences = new List<string>();
        Dictionary<WebClient, string> downloadingDllFilenames = new Dictionary<WebClient, string>();
        Dictionary<WebClient, string> downloadingDllReferences = new Dictionary<WebClient, string>();
        private List<EffectInfo> downloadingEffectInfo = new List<EffectInfo>();
        private List<ControlInfo> downloadingControlInfo = new List<ControlInfo>();
        private List<string> dllFilenames, dllReferences;
        private int count;
        
        public EffectDownloader()
        {
            string absoluteUri = Application.Current.Host.Source.AbsoluteUri;
            int lastSlash = absoluteUri.LastIndexOf("/");
            clientRoot = absoluteUri.Substring(0, lastSlash + 1);
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
                    string assemblyPath = clientRoot + DownloadArgs.ControlReferenceDllFolder;
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
                count++;

                if (count == dllFilenames.Count)
                {
                    if (DownloadCompleted != null)
                        DownloadCompleted();
                }
            }
        }

        public Assembly GetAssembly(EffectInfo ei)
        {
            return GetAssembly(ei.DllFilename);
        }

        public Assembly GetAssembly(string dllFilename)
        {
            if (LoadedAssembly.ContainsKey(dllFilename))
                return LoadedAssembly[dllFilename];
            return null;
        }

        public void DownloadEffect(EffectInfo ei)
        {
            downloadingEffectInfo.Add(ei);

            String assemblyPath = clientRoot + DownloadArgs.EffectDllFolder + ei.DllFilename;
            if (!downloadedDllFilenames.Contains(ei.DllFilename))
            {
                if (!downloadingDllFilenames.ContainsValue(ei.DllFilename))
                {
                    Uri uri = new Uri(assemblyPath, UriKind.Absolute);
                    //Start an async download:
                    WebClient webClient = new WebClient();
                    webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadEffectCompleted);
                    webClient.OpenReadAsync(uri);
                    downloadingDllFilenames.Add(webClient, ei.DllFilename);
                }
            }
            else
                ei.IsDllFileDownloaded = true;

            assemblyPath = clientRoot + DownloadArgs.EffectReferenceDllFolder;
            for (int i = 0; i < ei.DllReferences.Count; i++)
            {
                if (!downloadedDllReferences.Contains(ei.DllReferences[i]))
                {
                    if (!downloadingDllReferences.ContainsValue(ei.DllReferences[i]))
                    {
                        Uri uri = new Uri(assemblyPath + ei.DllReferences[i], UriKind.Absolute);
                        //Start an async download:
                        WebClient webClient = new WebClient();
                        webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_DownloadDllDependenceCompleted);
                        webClient.OpenReadAsync(uri);
                        downloadingDllReferences.Add(webClient, ei.DllReferences[i]);
                    }
                }
                else
                    ei.IsDllReferencesDownloaded[i] = true;
            }

            if (ei.IsReady)
            {
                if (DownloadEffectCompleted != null)
                    DownloadEffectCompleted(ei, LoadedAssembly[ei.DllFilename]);
                return;
            }
        }

        private void webClient_DownloadEffectCompleted(object sender, OpenReadCompletedEventArgs e)
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

                    for (int i = downloadingEffectInfo.Count - 1; i >= 0; i--)
                    {
                        if (downloadingEffectInfo[i].DllFilename == dllFilename)
                        {
                            downloadingEffectInfo[i].IsDllFileDownloaded = true;
                            if (downloadingEffectInfo[i].IsReady)
                            {
                                if (!LoadedAssembly.ContainsKey(dllFilename)) 
                                    LoadedAssembly.Add(dllFilename, assembly);
                                if (DownloadEffectCompleted != null)
                                    DownloadEffectCompleted(downloadingEffectInfo[i], assembly);
                                downloadingEffectInfo.RemoveAt(i);
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

                    for (int i = downloadingEffectInfo.Count - 1; i >= 0; i--)
                    {
                        string dllFilename = downloadingEffectInfo[i].DllFilename;
                        downloadingEffectInfo[i].CheckDllReferences(dll);
                        if (downloadingEffectInfo[i].IsReady)
                        {
                            if (!LoadedAssembly.ContainsKey(dllFilename))
                                LoadedAssembly.Add(dllFilename, LoadingAssembly[dllFilename]);
                            LoadingAssembly.Remove(dllFilename);
                            if (DownloadEffectCompleted != null)
                                DownloadEffectCompleted(downloadingEffectInfo[i], LoadedAssembly[dllFilename]);
                            downloadingEffectInfo.RemoveAt(i);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
