namespace octalforty.Wizardby.Core.Db
{
    public static class DbPlatformCapabilitiesExtensions
    {
        public static bool IsSupported(this DbPlatformCapabilities capabilities, DbPlatformCapabilities capability)
        {
            return (capabilities & capability) == capability;
        }
    }
}
