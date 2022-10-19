using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SampleMauiApp;

public partial class MainViewModel : ObservableObject
{
	public MainViewModel()
	{
		Items = new List<TodoItem>
		{
			new TodoItem("Walk the dog"),
			new TodoItem("Clean the pool"),
			new TodoItem("Water the flowers"),
			new TodoItem("Take out the garbage"),
			new TodoItem("Buy groceries"),
			new TodoItem("Finish my connect"),
			new TodoItem("Check my emails"),
			new TodoItem("Call my mom"),
			new TodoItem("Read a book"),
			new TodoItem("Listen to a podcast"),
			new TodoItem("Take a vacation"),
			new TodoItem("Add more items"),
			new TodoItem("Watch a movie"),
			new TodoItem("Play a game"),
			new TodoItem("Cook dinner"),
		};
	}

	[ObservableProperty]
	List<TodoItem> items = new();


	[ObservableProperty]
	string? addText;

	[RelayCommand]
	void Add(string title)
	{
		Items.Add(new TodoItem(title));
		AddText = string.Empty;
		OnPropertyChanged(nameof(Items));
		OnPropertyChanged(nameof(AddText));
	}
}
