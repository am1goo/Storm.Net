using System;
using System.IO;
using System.Threading.Tasks;

namespace Ston.Serializers.Loaders
{
    public class FileUrlStonLoader : IUrlStonLoader
    {
        public static readonly FileUrlStonLoader instance = new FileUrlStonLoader();

        public async Task<StonObject> DeserializeAsync(Uri uri, StonContext ctx)
        {
            var path = $"{uri.Host}{uri.AbsolutePath}";
            if (!Path.IsPathRooted(path))
            {
                var cwd = ctx.cwd;
                if (!string.IsNullOrWhiteSpace(cwd))
                    path = Path.Combine(cwd, path);
            }

            var fi = new FileInfo(path);
            if (!fi.Exists)
                throw new Exception($"file {fi.FullName} not found");

            ctx.cwd = fi.Directory.FullName;
            using (var fs = fi.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    var ston = await sr.ReadToEndAsync();
                    return await ctx.serializer.DeserializeAsync(ston, ctx);
                }
            }
        }
    }
}
