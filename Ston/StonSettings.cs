using Ston.Serializers;
using System.Collections.Generic;

namespace Ston
{
    public class StonSettings
    {
        public static readonly StonSettings defaultSettings = new StonSettings
        (
            options: 0,
            converters: null
        );

        private Options _options;
        public Options options => _options;

        private List<IStonConverter> _converters;
        public IReadOnlyList<IStonConverter> converters => _converters;

        public StonSettings(Options options, List<IStonConverter> converters)
        {
            _options = options;
            _converters = converters;
        }

        public enum Options
        {
            IgnoreCase = 0,
        }
    }
}
