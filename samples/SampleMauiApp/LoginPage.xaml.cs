namespace SampleMauiApp;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		var success = false;

		loginActivity.IsEnabled = loginActivity.IsRunning = loginActivity.IsVisible = true;
		entryUsername.IsEnabled = entryPassword.IsEnabled = false;

		var username = entryUsername.Text;
		var password = entryPassword.Text;

		if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
		{
			await this.DisplayAlert("Fields Missing", "Please enter your username and password", "OK");
		}
		else
		{
			success = username.Equals("xamarin", StringComparison.InvariantCultureIgnoreCase)
				&& password == "1234";
		}

        entryUsername.IsEnabled = entryPassword.IsEnabled = true;
        loginActivity.IsVisible = loginActivity.IsRunning = loginActivity.IsEnabled= false;

        if (!success)
        {
            await this.DisplayAlert("Login Failed", "Incorrect username or password!", "OK");
        }
		else
		{
			await Navigation.PushAsync(new MainPage(), true);
		}

    }
}