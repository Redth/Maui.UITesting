using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Automation.Driver;
using Newtonsoft.Json;

namespace Microsoft.Maui.Automation.Util;

public class Xcode
{
    public static List<DeviceData> GetDevices(params string[] targetPlatformIdentifiers)
    {
        var xcode = GetBestXcode();

        var xcdevice = new FileInfo(Path.Combine(xcode, "Contents/Developer/usr/bin/xcdevice"));

        if (!xcdevice.Exists)
            throw new FileNotFoundException(xcdevice.FullName);
        var pr = new ProcessRunner(NullLogger.Instance, xcdevice.FullName, "list");
        
        var json = string.Join(Environment.NewLine, pr.Output);


        var xcdevices = JsonConvert.DeserializeObject<List<XcDevice>>(json);

        var tpidevices = xcdevices
            .Where(d => (targetPlatformIdentifiers == null || targetPlatformIdentifiers.Length <= 0)
                || (targetPlatformIdentifiers.Intersect(d.DotNetPlatforms)?.Any() ?? false));

        var filteredDevices = tpidevices
            .Select(d => new DeviceData
            {
                IsEmulator = d.Simulator,
                IsRunning = false,
                Name = d.Name,
                Details = d.ModelName + " (" + d.Architecture + ")",
                Platforms = d.DotNetPlatforms,
                Serial = d.Identifier,
                Version = d.OperatingSystemVersion
            });

        return filteredDevices.ToList();
    }

    internal static string GetBestXcode()
    {
        var selected = GetSelectedXCodePath();

        if (!string.IsNullOrEmpty(selected))
            return selected;

        return FindXCodeInstalls()?.FirstOrDefault();
    }

    static string? GetSelectedXCodePath()
    {
        var r = new ProcessRunner(NullLogger.Instance, "/usr/bin/xcode-select", "-p");
        var xcodeSelectedPath = string.Join(Environment.NewLine, r.Output)?.Trim();

        if (!string.IsNullOrEmpty(xcodeSelectedPath))
        {
            var infoPlist = Path.Combine(xcodeSelectedPath, "..", "Info.plist");
            if (File.Exists(infoPlist))
            {
                var info = GetXcodeInfo(
                    Path.GetFullPath(
                        Path.Combine(xcodeSelectedPath, "..", "..")), true);

                if (info != null)
                    return info?.Path;
            }
        }

        return null;
    }

    static readonly string[] LikelyPaths = new[]
    {
        "/Applications/Xcode.app",
        "/Applications/Xcode-beta.app",
    };

    static IEnumerable<string> FindXCodeInstalls()
    {
        foreach (var p in LikelyPaths)
        {
            var i = GetXcodeInfo(p, false)?.Path;
            if (i != null)
                yield return i;
        }
    }

    static (string Path, bool Selected)? GetXcodeInfo(string path, bool selected)
    {
        var versionPlist = Path.Combine(path, "Contents", "version.plist");

        if (File.Exists(versionPlist))
        {
            return (path, selected);
        }
        else
        {
            var infoPlist = Path.Combine(path, "Contents", "Info.plist");

            if (File.Exists(infoPlist))
            {
                return (path, selected);
            }
        }
        return null;
    }
}

public class XcDevice
{

    public const string PlatformMacOsx = "com.apple.platform.macosx";
    public const string PlatformiPhoneSimulator = "com.apple.platform.iphonesimulator";
    public const string PlatformAppleTvSimulator = "com.apple.platform.appletvsimulator";
    public const string PlatformAppleTv = "com.apple.platform.appletvos";
    public const string PlatformWatchSimulator = "com.apple.platform.watchsimulator";
    public const string PlatformiPhone = "com.apple.platform.iphoneos";
    public const string PlatformWatch = "com.apple.platform.watchos";

    [JsonProperty("simulator")]
    public bool Simulator { get; set; }

    [JsonProperty("operatingSystemVersion")]
    public string OperatingSystemVersion { get; set; }

    [JsonProperty("available")]
    public bool Available { get; set; }

    [JsonProperty("platform")]
    public string Platform { get; set; }

    public bool IsiOS
        => !string.IsNullOrEmpty(Platform) && (Platform.Equals(PlatformiPhone) || Platform.Equals(PlatformiPhoneSimulator));
    public bool IsTvOS
        => !string.IsNullOrEmpty(Platform) && (Platform.Equals(PlatformAppleTv) || Platform.Equals(PlatformAppleTvSimulator));
    public bool IsWatchOS
        => !string.IsNullOrEmpty(Platform) && (Platform.Equals(PlatformWatch) || Platform.Equals(PlatformWatchSimulator));
    public bool IsOsx
        => !string.IsNullOrEmpty(Platform) && Platform.Equals(PlatformMacOsx);

    public string[] DotNetPlatforms
        => Platform switch
        {
            PlatformiPhone => new[] { "ios" },
            PlatformiPhoneSimulator => new[] { "ios" },
            PlatformAppleTv => new[] { "tvos" },
            PlatformAppleTvSimulator => new[] { "tvos" },
            PlatformWatch => new[] { "watchos" },
            PlatformWatchSimulator => new[] { "watchos" },
            PlatformMacOsx => new[] { "macos", "maccatalyst" },
            _ => new string[0]
        };

    [JsonProperty("modelCode")]
    public string ModelCode { get; set; }

    [JsonProperty("identifier")]
    public string Identifier { get; set; }

    [JsonProperty("architecture")]
    public string Architecture { get; set; }

    public string RuntimeIdentifier
        => Architecture switch
        {
            _ => "iossimulator-x64"
        };

    [JsonProperty("modelUTI")]
    public string ModelUTI { get; set; }

    [JsonProperty("modelName")]
    public string ModelName { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("interface")]
    public string Interface { get; set; }
}

public class DeviceData
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("details")]
    public string Details { get; set; }

    [JsonProperty("serial")]
    public string Serial { get; set; }

    [JsonProperty("platforms")]
    public string[] Platforms { get; set; }

    [JsonProperty("version")]
    public string Version { get; set; }

    [JsonProperty("isEmulator")]
    public bool IsEmulator { get; set; }

    [JsonProperty("isRunning")]
    public bool IsRunning { get; set; }

    [JsonProperty("rid")]
    public string RuntimeIdentifier { get; set; }
}