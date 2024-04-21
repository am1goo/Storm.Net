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

        public StormContext(StormSerializer serializer, StormSettings settings, string cwd)
        {
            _serializer = serializer;
            _settings = settings;
            _cwd = cwd;
        }
    }
}
