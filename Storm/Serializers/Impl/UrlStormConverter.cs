using Storm.Serializers.Loaders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storm.Serializers
{
    public class UrlStormConverter : IStormConverter
    {
        private readonly Dictionary<string, IUrlStormLoader> _loaders = new Dictionary<string, IUrlStormLoader>
        {
            { "file", FileUrlStormLoader.instance },
            { "http", HttpUrlStormLoader.instance },
            { "https", HttpUrlStormLoader.instance },
        };

        public UrlStormConverter AddLoader(string scheme, IUrlStormLoader loader)
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

        public async Task<IStormValue> DeserializeAsync(string type, string text, StormContext ctx)
        {
            if (!Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out var uri))
                return StormValue.nil;

            if (!_loaders.TryGetValue(uri.Scheme, out var loader))
                return StormValue.nil;

            return await loader.DeserializeAsync(uri, ctx);
        }
    }
}
