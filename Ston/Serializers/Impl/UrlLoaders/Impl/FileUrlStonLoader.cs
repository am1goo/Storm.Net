using System;
using System.IO;

namespace Ston.Serializers.Loaders
{
    public class FileUrlStonLoader : IUrlStonLoader
    {
        public static readonly FileUrlStonLoader instance = new FileUrlStonLoader();

        public StonObject Deserialize(Uri uri, StonContext ctx)
        {
            var path = $"{uri.Host}{uri.AbsolutePath}";
            if (!Path.IsPathRooted(path))
            {
                var cwd = ctx.settings.cwd;
                if (!string.IsNullOrWhiteSpace(cwd))
                    path = Path.Combine(cwd, path);
            }

            var fi = new FileInfo(path);
            if (!fi.Exists)
                throw new Exception($"file {fi.FullName} not found");

            using (var fs = fi.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    var ston = sr.ReadToEnd();
                    return ctx.serializer.Deserialize(ston, ctx);
                }
            }
        }
    }
}
