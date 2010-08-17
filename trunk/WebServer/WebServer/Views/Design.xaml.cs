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
using System.Windows.Navigation;
using WebServer.Web;
using WebServer.DataService;
using System.Windows.Browser;

namespace WebServer
{
    public partial class Designed : Page
    {
        private int rowHeight = 20;
        List<DesignedApplicationData> list = new List<DesignedApplicationData>();

        public Designed()
        {
            InitializeComponent();

            this.Title = ApplicationStrings.DesignApplicationPageTitle;
        }

        void Authentication_LoggedIn(object sender, System.ServiceModel.DomainServices.Client.ApplicationServices.AuthenticationEventArgs e)
        {
            GetDesignedApplicationList();
        }

        void Authentication_LoggedOut(object sender, System.ServiceModel.DomainServices.Client.ApplicationServices.AuthenticationEventArgs e)
        {
            grid.Children.Clear();
        }

        void GetDesignedApplicationList()
        {
            string uri = HtmlPage.Document.DocumentUri.AbsoluteUri;
            uri = uri.Substring(0,uri.IndexOf("index.aspx"));
            DataServiceClient client = new DataServiceClient("BasicHttpBinding_IDataService", uri + "DataService.svc");
            
            client.GetDesignedApplicationListCompleted += new EventHandler<GetDesignedApplicationListCompletedEventArgs>(client_GetDesignedApplicationListCompleted);
            client.GetDesignedApplicationListAsync(WebContext.Current.User.Name);
            //busyIndicator.IsBusy = true;
        }

        void client_GetDesignedApplicationListCompleted(object sender, GetDesignedApplicationListCompletedEventArgs e)
        {
            //busyIndicator.IsBusy = false;
            list.Clear();
            foreach (DesignedApplicationData da in e.Result)
                list.Add(da);
            ReloadGridList();
        }

        void ReloadGridList()
        {
            grid.Children.Clear();
            for (int i = 0; i < list.Count; i++)
                AddRow(list[i], i);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void AddRow(DataService.DesignedApplicationData da, int i)
        {
            Border bd1 = new Border();
            bd1.Child = new TextBlock() { Text = da.ApplicationName, VerticalAlignment = System.Windows.VerticalAlignment.Center };
            bd1.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 148, 173, 235));
            bd1.BorderThickness = new Thickness(0, 0, 0, 1);
            bd1.Margin = new Thickness(0, 3, 0, 0);

            Border bd2 = new Border();
            bd2.Child = new TextBlock() { Text = da.LastUpdate.ToString("HH:mm dd/MM/yyyy"), VerticalAlignment = System.Windows.VerticalAlignment.Center, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            bd2.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 148, 173, 235));
            bd2.BorderThickness = new Thickness(0, 0, 0, 1);
            bd2.Margin = new Thickness(0, 3, 0, 0);

            HyperlinkButton hb1 = new HyperlinkButton() { Content = "View", DataContext = da.Id, VerticalAlignment = System.Windows.VerticalAlignment.Center, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            hb1.Click += new RoutedEventHandler(hb1_Click);
            Border bd3 = new Border();
            bd3.Child = hb1;
            bd3.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 148, 173, 235));
            bd3.BorderThickness = new Thickness(0, 0, 0, 1);
            bd3.Margin = new Thickness(0, 3, 0, 0);

            HyperlinkButton hb2 = new HyperlinkButton() { Content = "Edit", DataContext = da.Id, VerticalAlignment = System.Windows.VerticalAlignment.Center, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            hb2.Click += new RoutedEventHandler(hb2_Click);
            Border bd4 = new Border();
            bd4.Child = hb2;
            bd4.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 148, 173, 235));
            bd4.BorderThickness = new Thickness(0, 0, 0, 1);
            bd4.Margin = new Thickness(0, 3, 0, 0);

            HyperlinkButton hb3 = new HyperlinkButton() { Content = "Delete", DataContext = da.Id, VerticalAlignment = System.Windows.VerticalAlignment.Center, HorizontalAlignment = System.Windows.HorizontalAlignment.Center };
            hb3.Click += new RoutedEventHandler(hb3_Click);
            Border bd5 = new Border();
            bd5.Child = hb3;
            bd5.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 148, 173, 235));
            bd5.BorderThickness = new Thickness(0, 0, 0, 1);
            bd5.Margin = new Thickness(0, 3, 0, 0);

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowHeight, GridUnitType.Pixel) });
            Grid.SetRow(bd1, i);
            Grid.SetRow(bd2, i);
            Grid.SetRow(bd3, i);
            Grid.SetRow(bd4, i);
            Grid.SetRow(bd5, i);
            Grid.SetColumn(bd2, 1);
            Grid.SetColumn(bd3, 2);
            Grid.SetColumn(bd4, 3);
            Grid.SetColumn(bd5, 4);
            grid.Children.Add(bd1);
            grid.Children.Add(bd2);
            grid.Children.Add(bd3);
            grid.Children.Add(bd4);
            grid.Children.Add(bd5);
        }

       void hb1_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hb = (HyperlinkButton)sender;
            string id = ((Guid)hb.DataContext).ToString();

            //navigate toi trang aspx preview
            string para = "?app=" + id;
            Uri documentUri = HtmlPage.Document.DocumentUri;
            HtmlPage.Window.Navigate(new Uri(documentUri, "Present/present.aspx" + para), "", "fullscreen=yes");
        }

        void hb2_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hb = (HyperlinkButton)sender;
            string id = ((Guid)hb.DataContext).ToString();

            //navvigate toi trang aspx design
            string para = "?app=" + id;
            Uri documentUri = HtmlPage.Document.DocumentUri;
            HtmlPage.Window.Navigate(new Uri(documentUri, "Design/design.aspx" + para),"","fullscreen=yes");
        }

        void hb3_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hb = (HyperlinkButton)sender;

            DataServiceClient client = new DataServiceClient();
            client.DeleteCompleted += new EventHandler<DeleteCompletedEventArgs>(client_DeleteCompleted);
            client.DeleteAsync((Guid)hb.DataContext);
        }

        void client_DeleteCompleted(object sender, DeleteCompletedEventArgs e)
        {
            foreach (DesignedApplicationData data in list)
            {
                if (data.Id == e.Result.Id)
                {
                    list.Remove(data);
                    break;
                }
            }
            ReloadGridList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            WebContext.Current.Authentication.LoggedIn += new EventHandler<System.ServiceModel.DomainServices.Client.ApplicationServices.AuthenticationEventArgs>(Authentication_LoggedIn);
            WebContext.Current.Authentication.LoggedOut += new EventHandler<System.ServiceModel.DomainServices.Client.ApplicationServices.AuthenticationEventArgs>(Authentication_LoggedOut);
            if (WebContext.Current.Authentication.User.Identity.IsAuthenticated)
                GetDesignedApplicationList();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Uri documentUri = HtmlPage.Document.DocumentUri;
            HtmlPage.Window.Navigate(new Uri(documentUri, "Design/design.aspx"), "", "fullscreen=yes");
        }
    }
}
