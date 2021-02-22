using System.IO;
using System.Text;

namespace CustomSSDTMigrationScripts
{
    public class ReferenceDataScriptTask : ScriptBaseTask
    {
        public ReferenceDataScriptTask()
        {

        }

        public override string ScriptTypeName => ScriptTypes.ReferenceDataScript;

        public override ScriptSettings DefaultScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = Path.Combine(base.ProjectRootDirectory, "Scripts", "ReferenceDataScripts"),
            ScriptNamePattern = @"^(\d+)_(.*).sql",
            ScriptRecursiveSearch = true,
            GeneratedScriptPath = Path.Combine(base.ProjectRootDirectory, "Scripts", "RunReferenceDataScriptsGenerated.sql"),
            ExecutionFilterMode = ScriptExecutionFilterMode.FILTER_BY_ALL,
            ExecutionFilterValue = null,
            TreatScriptNamePatternMismatchAsError = true,
            TreatHashMismatchAsError = true
        };

        public override ScriptSettings CurrentScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = settings.ReferenceDataScripts.ScriptBaseDirectory ?? DefaultScriptSettings.ScriptBaseDirectory,
            ScriptNamePattern = settings.ReferenceDataScripts.ScriptNamePattern ?? DefaultScriptSettings.ScriptNamePattern,
            ScriptRecursiveSearch = settings.ReferenceDataScripts.ScriptRecursiveSearch ?? DefaultScriptSettings.ScriptRecursiveSearch,
            GeneratedScriptPath = settings.ReferenceDataScripts.GeneratedScriptPath ?? DefaultScriptSettings.GeneratedScriptPath,
            ExecutionFilterMode = settings.ReferenceDataScripts.ExecutionFilterMode ?? DefaultScriptSettings.ExecutionFilterMode,
            ExecutionFilterValue = settings.ReferenceDataScripts.ExecutionFilterValue ?? DefaultScriptSettings.ExecutionFilterValue,
            TreatScriptNamePatternMismatchAsError = settings.ReferenceDataScripts.TreatScriptNamePatternMismatchAsError ?? DefaultScriptSettings.TreatScriptNamePatternMismatchAsError,
            TreatHashMismatchAsError = settings.ReferenceDataScripts.TreatHashMismatchAsError ?? DefaultScriptSettings.TreatHashMismatchAsError,
        };

        public override void ExecuteScriptTask()
        {
            var sqlScript = $@"
{SqlSnippets.ManufacturerHeader}

PRINT '******************* {ScriptTypeName} scripts *******************'

-- Start transaction for all {ScriptTypeName} scripts
BEGIN TRANSACTION;
BEGIN TRY

    DECLARE @DBEXISTS bit;
    SET @DBEXISTS = 0;

    DECLARE @DBNAME NVARCHAR(128);
    SET @DBNAME = DB_NAME();

    DECLARE @DBID INT;
    SET @DBID = DB_ID();

    IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = @DBNAME)
    BEGIN
        PRINT 'The database ' +  @DBNAME + ' does not yet exist. All {ScriptTypeName} deployment scripts will be skipped.'
        SET @DBEXISTS = 0
    END
    ELSE
    BEGIN
        PRINT 'The database ' +  @DBNAME + ' already exists. {ScriptTypeName} deployment scripts will be executed.'
        SET @DBEXISTS = 1
    END";

            var scripts = GetScripts();
            foreach (var script in scripts)
            {
                var hashBase64 = script.ScriptHash;

                sqlScript += $@"

    -- Only run scripts of type '{ScriptTypes.PreScript}' if the database exists, otherwise skip.
    IF @DBEXISTS=1 AND '{ScriptTypeName}'='{ScriptTypes.ReferenceDataScript}'
    BEGIN    
        -- Run the new migration script
        PRINT '------------------ RUN --------------------------'
        PRINT 'Script Id:      {script.UniqueScriptId}'
        PRINT 'Script Name:    {script.Name}'
        PRINT 'Order Criteria: {script.OrderCriteria}'
        PRINT 'Script Type:    {script.MigrationType}'
        PRINT 'Script Path:    {script.FullFilePath}'
        PRINT 'Script Hash:    {hashBase64}'
        
        PRINT ' > Start {ScriptTypeName}-script run....'
        {TSqlHelper.GetScopedTsqlByFile(script.FullFilePath)}
        PRINT ' > Finished {ScriptTypeName}-script run....'
        
        PRINT '----------------- END RUN ------------------------'
        PRINT '|'
    END";
            }

            // Add transaction handling and catch errors
            sqlScript += $@"
END TRY
BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT @ErrorMessage = ERROR_MESSAGE(),
           @ErrorSeverity = ERROR_SEVERITY(),
           @ErrorState = ERROR_STATE();

    -- Rollback all transactions if any of the {ScriptTypeName} scripts failed.
    IF @@TRANCOUNT > 0  
        ROLLBACK TRANSACTION;

    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);

END CATCH;

-- Commit transaction if all {ScriptTypeName} scripts have been successfully run.
IF @@TRANCOUNT > 0  
    COMMIT TRANSACTION;  
GO";

            File.WriteAllText(CurrentScriptSettings.GeneratedScriptPath, sqlScript, Encoding.UTF8);
            Logger.LogMessage($@"Script execution file '{CurrentScriptSettings.GeneratedScriptPath}' has been generated.");
        }
    }
}
