using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using SharpCompress.Readers;

namespace Microsoft.Maui.Automation.Driver
{
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

