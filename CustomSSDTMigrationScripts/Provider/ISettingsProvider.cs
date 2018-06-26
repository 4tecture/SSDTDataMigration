namespace CustomSSDTMigrationScripts
{
    public interface ISettingsProvider
    {
        Settings GetSettings(string directory);
    }
}
