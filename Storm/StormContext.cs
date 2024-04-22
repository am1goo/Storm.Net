using System.Globalization;

namespace Storm
{
    public class StormContext
    {
        private StormSerializer _serializer;
        public StormSerializer serializer => _serializer;

        private StormSettings _settings;
        public StormSettings settings => _settings;

        private string _cwd;
        public string cwd { get => _cwd; set => _cwd = value; }

        private int _intent;
        public int intent { get => _intent; set => _intent = value; }

        private CultureInfo _numberCultureInfo = new CultureInfo("en");
        public CultureInfo numberCultureInfo
        {
            get
            {
                _numberCultureInfo.NumberFormat.NumberDecimalSeparator = _settings.numberDecimalSeparator;
                return _numberCultureInfo;
            }
        }

        public StormContext(StormSerializer serializer, StormSettings settings, string cwd)
        {
            _serializer = serializer;
            _settings = settings;
            _cwd = cwd;
        }
    }
}
