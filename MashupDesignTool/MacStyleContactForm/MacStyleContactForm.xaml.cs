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
using System.Text.RegularExpressions;
using System.ServiceModel;

namespace MacStyleContactFormControl
{
    public partial class MacStyleContactForm : BasicControl
    {
        private string receiveEmail = "tranphuonghai@gmail.com";
        private string proxy = "http://localhost:1728/Service1.svc";

        public MacStyleContactForm()
        {
            InitializeComponent();
        }

        public string ReceiveEmail
        {
            get { return receiveEmail; }
            set
            {
                Regex re = new Regex(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
                if (re.IsMatch(value))
                    receiveEmail = value;
            }
        }

        public Color LabelColor
        {
            get { return ((SolidColorBrush)lblEmail.Foreground).Color; }
            set
            {
                ((SolidColorBrush)lblEmail.Foreground).Color = value;
                ((SolidColorBrush)lblMessage.Foreground).Color = value;
                ((SolidColorBrush)lblName.Foreground).Color = value;
                ((SolidColorBrush)lblSubject.Foreground).Color = value;
                ((SolidColorBrush)lblError.Foreground).Color = value;
            }
        }

        public Color ButtonColor
        {
            get { return ((LinearGradientBrush)button1.Background).GradientStops[1].Color; }
            set { ((LinearGradientBrush)button1.Background).GradientStops[1].Color = value; }
        }

        public Color ContentColor
        {
            get { return ((SolidColorBrush)txtEmail.Foreground).Color; }
            set
            {
                ((SolidColorBrush)txtEmail.Foreground).Color = value;
                ((SolidColorBrush)txtSubject.Foreground).Color = value;
                ((SolidColorBrush)txtName.Foreground).Color = value;
                ((SolidColorBrush)txtMessage.Foreground).Color = value;
            }
        }

        public Color ContentBackgroundColor
        {
            get { return ((LinearGradientBrush)txtEmail.Background).GradientStops[1].Color; }
            set
            {
                ((LinearGradientBrush)txtEmail.Background).GradientStops[0].Color = value;
                ((LinearGradientBrush)txtSubject.Background).GradientStops[0].Color = value;
                ((LinearGradientBrush)txtName.Background).GradientStops[0].Color = value;
                ((LinearGradientBrush)txtMessage.Background).GradientStops[0].Color = value;
            }
        }

        private void root_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            txtEmail.Width = lblEmail.ActualWidth;
            txtMessage.Width = lblEmail.ActualWidth;
            txtName.Width = lblEmail.ActualWidth;
            txtSubject.Width = lblEmail.ActualWidth;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (receiveEmail.Length == 0)
            {
                lblError.Content = "Receive email empty!";
                return;
            }

            if (txtName.Text.Length == 0)
            {
                lblError.Content = "Please enter your name!";
                return;
            }

            Regex re = new Regex(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");
            if (!re.IsMatch(txtEmail.Text))
            {
                lblError.Content = "Please enter valid email!";
                return;
            }

            if (txtSubject.Text.Length == 0)
            {
                lblError.Content = "Please enter your subject!";
                return;
            }

            if (txtMessage.Text.Length == 0)
            {
                lblError.Content = "Please enter your message!";
                return;
            }

            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress(proxy);
            ServiceReference1.Service1Client client = new ServiceReference1.Service1Client(basicHttpBinding, endpointAddress);
            client.SendMailCompleted += new EventHandler<ServiceReference1.SendMailCompletedEventArgs>(client_SendMailCompleted);
            client.SendMailAsync(txtName.Text, txtEmail.Text, receiveEmail, txtSubject.Text, txtMessage.Text);
            lblError.Content = "Sending ...";
            button1.Visibility = System.Windows.Visibility.Collapsed;
        }

        void client_SendMailCompleted(object sender, ServiceReference1.SendMailCompletedEventArgs e)
        {
            if (e.Error == null && e.Result == true)
                lblError.Content = "Your message was sent!";
            else
                lblError.Content = "An error occurred while sending!";

            button1.Visibility = System.Windows.Visibility.Visible;
            txtEmail.Text = "";
            txtMessage.Text = "";
            txtName.Text = "";
            txtSubject.Text = "";

            if (mainEffect != null)
                mainEffect.Start();
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            if (mainEffect != null)
                mainEffect.Start();
        }
    }
}
