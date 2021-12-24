using Microsoft.Maui.Automation;

namespace SampleMauiApp
{
    public partial class App : Microsoft.Maui.Controls.Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            

            this.StartAutomationServiceConnection("192.168.2.50");
        }
    }
}