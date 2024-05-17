using Storm.Converters;
using Storm.Serializers;
using System.Collections.Generic;
using System.Text;

namespace Storm
{
    public class StormSettings
    {
        public Options options                              = 0;
        public string cwd                                   = null;
        public IEnumerable<IStormSerializer> serializers    = null;
        public IEnumerable<IStormConverter> converters      = null;
        public Encoding encoding                            = Encoding.UTF8;
        public StormBooleanStyle defaultBooleanStyle        = StormBooleanStyle.Default;
        public StormEnumFormat defaultEnumFormat            = StormEnumFormat.String;
        public string numberDecimalSeparator                = ",";
        public int intentSize                               = 2;

        private static readonly Dictionary<int, Dictionary<int, string>> _intentCache = new Dictionary<int, Dictionary<int, string>>();
        private static readonly char _intentChar = ' ';

        public static StormSettings Default()
        {
            return new StormSettings();
        }

        /*
        public string GetIntent(int intents)
        {
            if (!_intentCache.TryGetValue(intents, out var sizes))
            {
                sizes = new Dictionary<int, string>();
                _intentCache.Add(intents, sizes);
            }

            if (!sizes.TryGetValue(intentSize, out var exist))
            {
                exist = new string(_intentChar, intents * intentSize);
                sizes.Add(intentSize, exist);
            }
            return exist;
        }
        */
        public string GetIntent(int intents)
        {
            if (!_intentCache.TryGetValue(intentSize, out var sizes))
            {
                sizes = new Dictionary<int, string>();
                _intentCache.Add(intentSize, sizes);
            }

            if (!sizes.TryGetValue(intents, out var exist))
            {
                exist = new string(_intentChar, intents * intentSize);
                sizes.Add(intents, exist);
            }
            return exist;
        }

        public enum Options
        {
            IgnoreCase = 0,
        }
    }
}
