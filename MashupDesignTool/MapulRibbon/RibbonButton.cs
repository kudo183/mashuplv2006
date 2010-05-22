using System.Windows;
using System.Windows.Controls;

namespace MapulRibbon
{
    public class RibbonButton : RibbonButtonBase
    {
        public RibbonButton()
		{			
            // Attach an events
            this.Loaded += new RoutedEventHandler(RibbonButton_Loaded);            
        }              

        void RibbonButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (Button == null)
            {
                Button = new Button();
                if (Button.Style == null)
                    Button.Style = buttonStyle;
                //
                ButtonLoaded();
            }
        }        
    }
}
