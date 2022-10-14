using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Formatting;
using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Driver;

public static class DriverExtensions
{
	public static async Task RenderScreenshot(this IDriver driver)
	{
		var file = Path.GetTempFileName();
		await driver.Screenshot(file);

		if (KernelInvocationContext.Current is { } context)
		{
			var ext = Path.GetExtension(file)?.ToLowerInvariant()?.TrimStart('.')?.Trim();

			var mime = ext switch
			{
				"jpg" => "image/jpeg",
				"jpeg" => "image/jpeg",
				"png" => "image/png",
				_ => "image/png"
			};

			var data = Convert.ToBase64String(File.ReadAllBytes(file));

			PocketView img = PocketViewTags.img[src: $"data:{mime};base64,{data}", style: "max-width: 1000px; max-height: 1000px;"];

			context.Display(img, "text/html");
		}
		

	}
}

