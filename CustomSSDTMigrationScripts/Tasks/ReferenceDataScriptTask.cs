using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CustomSSDTMigrationScripts
{
    public class ReferenceDataScriptTask : ScriptBaseTask
    {
        public ReferenceDataScriptTask()
        {
        }

        public override string ScriptTypeName => ScriptTypes.ReferenceDataScript;

        protected override ScriptSettings DefaultScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = Path.Combine(ProjectRootDirectory, "Scripts", "ReferenceDataScripts"),
            ScriptNamePattern = @"^(\d+)_(.*).sql",
            ScriptRecursiveSearch = true,
            GeneratedScriptPath = Path.Combine(ProjectRootDirectory, "Scripts", "RunReferenceDataScriptsGenerated.sql"),
            ExecutionFilterMode = ScriptExecutionFilterMode.FILTER_BY_ALL,
            ExecutionFilterValue = null,
            TreatScriptNamePatternMismatchAsError = true,
            TreatHashMismatchAsError = true
        };

        public override ScriptSettings ScriptSettings => new ScriptSettings
        {
            ScriptBaseDirectory = Settings.ReferenceDataScripts.ScriptBaseDirectory ?? DefaultScriptSettings.ScriptBaseDirectory,
            ScriptNamePattern = Settings.ReferenceDataScripts.ScriptNamePattern ?? DefaultScriptSettings.ScriptNamePattern,
            ScriptRecursiveSearch = Settings.ReferenceDataScripts.ScriptRecursiveSearch ?? DefaultScriptSettings.ScriptRecursiveSearch,
            GeneratedScriptPath = Settings.ReferenceDataScripts.GeneratedScriptPath ?? DefaultScriptSettings.GeneratedScriptPath,
            ExecutionFilterMode = Settings.ReferenceDataScripts.ExecutionFilterMode ?? DefaultScriptSettings.ExecutionFilterMode,
            ExecutionFilterValue = Settings.ReferenceDataScripts.ExecutionFilterValue ?? DefaultScriptSettings.ExecutionFilterValue,
            TreatScriptNamePatternMismatchAsError = Settings.ReferenceDataScripts.TreatScriptNamePatternMismatchAsError ?? DefaultScriptSettings.TreatScriptNamePatternMismatchAsError,
            TreatHashMismatchAsError = Settings.ReferenceDataScripts.TreatHashMismatchAsError ?? DefaultScriptSettings.TreatHashMismatchAsError,
        };

        protected override void ExecuteScriptTask()
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

            sqlScript += $@"
    
    -- Prefill the _MigrationScriptsHistory table with the existing migration scripts when applying the first time
    IF @DBEXISTS=1 AND '{ScriptTypeName}'='{ScriptTypes.ReferenceDataScript}'
    BEGIN    
        IF OBJECT_ID(N'dbo._MigrationScriptsHistory', N'U') IS NOT NULL
        BEGIN
            IF NOT EXISTS (SELECT * FROM [dbo].[_MigrationScriptsHistory])
            BEGIN";

            var existingScripts = new List<Script>();

            // add all PreScripts
            var preScriptTask = new PreScriptTask();
            preScriptTask.ProjectRootDirectory = ProjectRootDirectory;
            preScriptTask.Initialize(false);
            existingScripts.AddRange(GetScripts(preScriptTask.ScriptSettings));

            // add all PostScripts
            var postScriptTask = new PostScriptTask();
            postScriptTask.ProjectRootDirectory = ProjectRootDirectory;
            postScriptTask.Initialize(false);
            existingScripts.AddRange(GetScripts(postScriptTask.ScriptSettings));

            if (existingScripts.Any())
            {
                sqlScript += $@"
                PRINT N'Set default values for MigrationScriptsHistory';

                INSERT INTO [dbo].[_MigrationScriptsHistory] 
                VALUES";

                var separator = string.Empty;
                foreach (var script in existingScripts)
                {
                    sqlScript += $@"
                    {separator}('{script.UniqueScriptId}', GETUTCDATE(),'{script.ScriptHash}')";

                    separator = ",";
                }

                // add entry into MigrationScriptsHistory table to set initialization status which blocks adding upcoming scripts
                sqlScript += $@"
                    {separator}('initialization\_migrationscriptshistory', GETUTCDATE(),'ZmI3OWI5ZThiOWU2ZGU5MTYwODFkYmJmN2M3ZjFlYzM=')";
            }
            else
            {
                sqlScript += $@"
                PRINT N'No scripts found to add to MigrationScriptsHistory';";
            }

            sqlScript += $@"
            END
            ELSE
            BEGIN
                PRINT N'MigrationScriptsHistory already has entries, skipping.';
            END
        END
        ELSE
        BEGIN
            RAISERROR ('ERROR: The migration script history table _MigrationScriptsHistory does not exists.', 18, 1);
        END
    END";

            var scripts = GetScripts();
            foreach (var script in scripts)
            {
                var hashBase64 = script.ScriptHash;

                sqlScript += $@"

    -- Only run scripts of type '{ScriptTypes.ReferenceDataScript}' if the database exists, otherwise skip.
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

            File.WriteAllText(ScriptSettings.GeneratedScriptPath, sqlScript, Encoding.UTF8);
            Logger.LogMessage($@"Script execution file '{ScriptSettings.GeneratedScriptPath}' has been generated.");
        }
    }
}
