using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SampleMauiApp;

public partial class MainPage : ContentPage
{
	MainViewModel viewModel = new();

	public MainPage()
	{
		BindingContext = viewModel;

		InitializeComponent();
	}

}
