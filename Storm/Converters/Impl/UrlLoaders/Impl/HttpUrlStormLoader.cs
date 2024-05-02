using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Reflection;

namespace Storm.Converters.Loaders
{
    public class HttpUrlStormLoader : IUrlStormLoader
    {
        public static HttpUrlStormLoader instance = new HttpUrlStormLoader();

        private static readonly AssemblyName _assemblyName = typeof(StormSerializer).Assembly.GetName();

        public async Task<StormObject> DeserializeAsync(Uri uri, StormContext ctx)
        {
            using (var client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }))
            {
                var name = _assemblyName.Name;
                var version = _assemblyName.Version?.ToString();
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(name, version));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var storm = await client.GetStringAsync(uri);
                return await ctx.serializer.DeserializeAsync(storm, ctx);
            }
        }
    }
}
