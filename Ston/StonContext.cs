namespace Ston
{
    public class StonContext
    {
        private StonSerializer _serializer;
        public StonSerializer serializer => _serializer;

        private StonSettings _settings;
        public StonSettings settings => _settings;

        private string _cwd;
        public string cwd { get => _cwd; set => _cwd = value; }

        public StonContext(StonSerializer serializer, StonSettings settings, string cwd)
        {
            _serializer = serializer;
            _settings = settings;
            _cwd = cwd;
        }
    }
}
