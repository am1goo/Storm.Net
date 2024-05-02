using System;
using System.IO;
using System.Threading.Tasks;

namespace Storm.Converters.Loaders
{
    public class FileUrlStormLoader : IUrlStormLoader
    {
        public static readonly FileUrlStormLoader instance = new FileUrlStormLoader();

        public async Task<StormObject> DeserializeAsync(Uri uri, StormContext ctx)
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
                    var storm = await sr.ReadToEndAsync();
                    return await ctx.serializer.DeserializeAsync(storm, ctx);
                }
            }
        }
    }
}
