using System.IO.Compression;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Claunia.PropertyList;
using SharpCompress.Readers;

namespace Microsoft.Maui.Automation.Driver
{
	public class RegexParser
	{
		public RegexParser(string input, string pattern, RegexOptions rxOptions = RegexOptions.Singleline | RegexOptions.IgnoreCase)
		{
			Match = Regex.Match(input, pattern, rxOptions);
		}

		public readonly Match Match;

		public int GetGroup(string groupName, int defaultValue)
		{
			if (Match?.Groups.TryGetValue(groupName, out var group) ?? false)
			{
				if (int.TryParse(group.Value, out var intValue))
					return intValue;
			}
			return defaultValue;
		}
	}

	public static class AppUtil
	{
		internal static Platform InferDevicePlatformFromFilename(string? appFile)
		{
			if (string.IsNullOrEmpty(appFile))
				throw new ArgumentNullException("AppFilename");


			if (!File.Exists(appFile) && !Directory.Exists(appFile))
				throw new ArgumentNullException("AppFilename");

			var ext = Path.GetExtension(appFile.TrimEnd('/'));

			return ext.ToLowerInvariant() switch
			{
				".apk" => Platform.Android,
				".app" => InferApplePlatform(appFile),
				".msix" => Platform.Winappsdk,
				_ => throw new ArgumentOutOfRangeException("Unable to infer Device platform, try specifying explicitly in the config.")
			};
		}


		public static Platform InferApplePlatform(string? appFile)
		{
			if (string.IsNullOrEmpty(appFile))
				throw new ArgumentNullException("AppFilename");

            if (!File.Exists(appFile) && !Directory.Exists(appFile))
                throw new ArgumentNullException("AppFilename");

			if (appFile.EndsWith(".app", StringComparison.InvariantCultureIgnoreCase))
			{
				var plistFile = Path.Combine(appFile.TrimEnd('/') + "/", "Contents", "Info.plist");
				if (File.Exists(plistFile))
				{
					var rootDict = (NSDictionary)PropertyListParser.Parse(plistFile);
					var dtplatformName = rootDict.ObjectForKey("DTPlatformName")?.ToString();

					if (!string.IsNullOrEmpty(dtplatformName))
					{
						if (dtplatformName.Equals("maccatalyst", StringComparison.OrdinalIgnoreCase))
							return Platform.Maccatalyst;

						return Platform.Macos;
					}
				}

				// iOS and ipad apps have the info.plist in the bundle root
				plistFile = Path.Combine(appFile.TrimEnd('/') + "/", "Info.plist");
				if (File.Exists(plistFile))
					return Platform.Ios;
			}
			throw new ArgumentOutOfRangeException("Unknown Apple Platform");
		}

		public static string? GetBundleIdentifier(string? appFile)
		{
			if (string.IsNullOrEmpty(appFile))
				return null;

            if (!File.Exists(appFile) && !Directory.Exists(appFile))
                throw new ArgumentNullException("AppFilename");

            if (appFile.EndsWith(".app", StringComparison.InvariantCultureIgnoreCase))
			{
				// iOS is in root of .app
				var plistFile = Path.Combine(appFile.TrimEnd('/') + "/", "Info.plist");

				// If not there, check for mac pattern inside Contents/Info.plist
				if (!File.Exists(plistFile))
					plistFile = Path.Combine(appFile.TrimEnd('/') + "/", "Contents", "Info.plist");

				if (!File.Exists(plistFile))
					return null;

				var rootDict = (NSDictionary)PropertyListParser.Parse(plistFile);
				return rootDict.ObjectForKey("CFBundleIdentifier")?.ToString();
			}

			return null;
		}

		public static string? GetPackageId(string? apkFile)
		{
			if (string.IsNullOrEmpty(apkFile) || !File.Exists(apkFile))
				return null;

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

		public static string? GetAppxId(string msixFilename)
		{
			using var zip = ZipFile.OpenRead(msixFilename);

			foreach (var entry in zip.Entries)
			{
				if (entry.FullName.Equals("AppxManifest.xml", StringComparison.OrdinalIgnoreCase))
				{
					using var s = entry.Open();
					var doc = XDocument.Load(s);
					var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;
					
					var rootElem = doc.Root;

					var identityName = rootElem?.Element(ns + "Identity")?.Attribute("Name")?.Value;
					var appId = rootElem?.Element(ns + "Applications")?.Element(ns + "Application")?.Attribute("Id")?.Value;
					
					if (!string.IsNullOrEmpty(identityName) && !string.IsNullOrEmpty(appId))
						return $"{identityName}!{appId}";
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

