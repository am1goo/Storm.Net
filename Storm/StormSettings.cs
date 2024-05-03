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
        public StormEnumFormat defaultEnumFormat            = StormEnumFormat.String;
        public string numberDecimalSeparator                = ",";
        public int intentSize                               = 2;

        private Dictionary<int, string> _intentCache = new Dictionary<int, string>();

        public static StormSettings Default()
        {
            return new StormSettings();
        }

        public string GetIntent(int intents)
        {
            if (!_intentCache.TryGetValue(intents, out var exist))
            {
                exist = new string(' ', intents * intentSize);
                _intentCache.Add(intents, exist);
            }
            return exist;
        }

        public enum Options
        {
            IgnoreCase = 0,
        }
    }
}
