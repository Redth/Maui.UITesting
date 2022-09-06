using Microsoft.Maui.Automation;

namespace SampleMauiApp
{
    public partial class App : Microsoft.Maui.Controls.Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

//#if ANDROID
           this.StartAutomationServiceListener("http://127.0.0.1:5000");
//#else
            //this.StartAutomationServiceListener("https://localhost:5001");
//#endif
		}
	}
}