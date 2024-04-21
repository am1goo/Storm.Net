using Ston.Serializers.Loaders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ston.Serializers
{
    public class UrlStonConverter : IStonConverter
    {
        private readonly Dictionary<string, IUrlStonLoader> _loaders = new Dictionary<string, IUrlStonLoader>
        {
            { "file", FileUrlStonLoader.instance },
            { "http", HttpUrlStonLoader.instance },
            { "https", HttpUrlStonLoader.instance },
        };

        public UrlStonConverter AddLoader(string scheme, IUrlStonLoader loader)
        {
            if (string.IsNullOrWhiteSpace(scheme))
                throw new Exception($"{nameof(scheme)} cannot be null");

            if (loader == null)
                throw new Exception($"{nameof(loader)} cannot be null");

            if (_loaders.ContainsKey(scheme))
                throw new Exception($"already contains registered loader for scheme '{scheme}'");

            _loaders.Add(scheme, loader);
            return this;
        }

        public bool CanConvert(string type)
        {
            return type.Equals("url", StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<IStonValue> DeserializeAsync(string type, string text, StonContext ctx)
        {
            if (!Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out var uri))
                return StonValue.nil;

            if (!_loaders.TryGetValue(uri.Scheme, out var loader))
                return StonValue.nil;

            return await loader.DeserializeAsync(uri, ctx);
        }
    }
}
