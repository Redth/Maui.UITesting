using System;
namespace SampleMauiApp
{
	public class TodoItem
	{
		public TodoItem(string title, bool done = false)
		{
			Title = title;
			Done = done;
		}

		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string Title { get; set; }
		public bool Done { get; set; }
	}
}

