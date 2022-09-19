using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using Claunia.PropertyList;
using Newtonsoft.Json.Linq;
using SharpCompress.Readers;

namespace Microsoft.Maui.Automation.Driver
{
	public static class AppUtil
	{
		public static string? GetBundleIdentifier(string appFile)
		{
			var file = new FileInfo(appFile);

			if (appFile.EndsWith(".app", StringComparison.InvariantCultureIgnoreCase))
			{
				// iOS is in root of .app
				var plistFile = Path.Combine(file.FullName + "/", "Info.plist");

				// If not there, check for mac pattern inside Contents/Info.plist
				if (!File.Exists(plistFile))
					plistFile = Path.Combine(file.FullName + "/", "Contents", "Info.plist");

				if (!File.Exists(plistFile))
					return null;

				var rootDict = (NSDictionary)PropertyListParser.Parse(file);
				return rootDict.ObjectForKey("CFBundleIdentifier")?.ToString();
			}

			return null;
		}

		public static string? GetPackageId(string apkFile)
		{
			using var zip = ZipFile.OpenRead(apkFile);

			foreach (var entry in zip.Entries)
			{
				if (entry.FullName.Equals("AndroidManifest.xml", StringComparison.OrdinalIgnoreCase))
				{
					using var s = entry.Open();
					using var ms = new MemoryStream();

					s.CopyTo(ms);

					var data = ms.ToArray();

					var manifestReader = new AndroidManifestReader(data);
					var manifestElement = manifestReader.Manifest.Element("root")?.Element("manifest");
					return manifestElement?.Attribute("package")?.Value;
				}
			}

			return null;
		}
	}

	public static class EmbeddedToolUtil
	{
		public static Stream OpenEmbeddedResourceRead(string filename)
		{
			var thisAssembly = Assembly.GetExecutingAssembly();
			foreach (var name in thisAssembly.GetManifestResourceNames())
			{
				if (name.EndsWith(filename, StringComparison.Ordinal))
				{
					var stream = thisAssembly.GetManifestResourceStream(filename);
					if (stream is not null)
						return stream;
				}
			}

			throw new FileNotFoundException($"Embedded Resource File Not Found: {filename}");
		}

		public static void ExtractTarGz(Stream stream, string outputDirectory)
		{
			using var reader = ReaderFactory.Open(stream);

			reader.WriteAllToDirectory(outputDirectory, new SharpCompress.Common.ExtractionOptions
			{
				PreserveAttributes = true,
				ExtractFullPath = true,
				Overwrite = true,
			});
		}

		public static void ExtractEmbeddedResourceTarGz(string filename, string outputDirectory)
		{
			using var stream = OpenEmbeddedResourceRead(filename);
			using var reader = ReaderFactory.Open(stream);

			reader.WriteAllToDirectory(outputDirectory, new SharpCompress.Common.ExtractionOptions
			{
				ExtractFullPath = true,
				Overwrite = true,
			});
		}
	}
}

