namespace Ston
{
    public class StonContext
    {
        private StonSerializer _serializer;
        public StonSerializer serializer => _serializer;

        private StonSettings _settings;
        public StonSettings settings => _settings;

        public StonContext(StonSerializer serializer, StonSettings settings)
        {
            _serializer = serializer;
            _settings = settings;
        }
    }
}
