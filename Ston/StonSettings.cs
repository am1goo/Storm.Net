namespace Ston
{
    public class StonSettings
    {
        public static readonly StonSettings defaultSettings = new StonSettings();

        public Permissions permissions;

        public enum Permissions
        {
            IgnoreCase = 0,
        }
    }
}
