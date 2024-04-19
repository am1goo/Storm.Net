using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Reflection;

namespace Ston.Serializers.Loaders
{
    public class HttpUrlStonLoader : IUrlStonLoader
    {
        public static HttpUrlStonLoader instance = new HttpUrlStonLoader();

        private static readonly AssemblyName _assemblyName = typeof(StonSerializer).Assembly.GetName();

        public async Task<StonObject> DeserializeAsync(Uri uri, StonContext ctx)
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

                var ston = await client.GetStringAsync(uri);
                return await ctx.serializer.DeserializeAsync(ston, ctx);
            }
        }
    }
}
