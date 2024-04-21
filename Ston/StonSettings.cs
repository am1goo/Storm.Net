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

        private string _cwd;
        public string cwd => _cwd;

        private List<IStonConverter> _converters;
        public IEnumerable<IStonConverter> converters => _converters;

        public StonSettings(Options options, List<IStonConverter> converters)
        {
            _options = options;
            _converters = converters;
        }

        internal void SetCwd(string cwd)
        {
            _cwd = cwd;
        }

        public enum Options
        {
            IgnoreCase = 0,
        }
    }
}
