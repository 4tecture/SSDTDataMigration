using System;
using System.IO;
using System.Text;

namespace CustomSSDTMigrationScripts
{
    public abstract class PrePostScriptTaskBase : ScriptBaseTask
    {
        public PrePostScriptTaskBase()
        {
        }

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

    -- Only run scripts of type '{ScriptTypes.PreScript}' if the database exists, otherwise skip. {ScriptTypes.PostScript} scripts are always run.
    IF @DBEXISTS=1 AND '{ScriptTypeName}'='{ScriptTypes.PreScript}' AND OBJECT_ID(N'dbo._MigrationScriptsHistory', N'U') IS NOT NULL OR '{ScriptTypeName}'='{ScriptTypes.PostScript}'
    BEGIN
        -- Check if the script was already run in previous migrations
        IF NOT EXISTS(SELECT *
                        FROM dbo.[_MigrationScriptsHistory]
                        WHERE [ScriptNameId] = '{script.UniqueScriptId}')
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
        
            -- Register the script in the migration script history table to prevent duplicate runs
            PRINT ' > Register script in migration history table.'
            INSERT INTO dbo.[_MigrationScriptsHistory]
            VALUES('{script.UniqueScriptId}', GETDATE(), '{hashBase64}')

            PRINT '----------------- END RUN ------------------------'
            PRINT '|'
        END
        ELSE
        BEGIN
            -- The script was already run. Check if script hash has changed meanwhile.
            IF NOT EXISTS(SELECT *
                            FROM dbo.[_MigrationScriptsHistory]
                            WHERE [ScriptHash] = '{hashBase64}')
            BEGIN
                IF {Convert.ToInt32(CurrentScriptSettings.TreatHashMismatchAsError)}!=0
                    RAISERROR ('ERROR: The hash value for the {ScriptTypeName} migration script {script.Name} does not match with the origin registered and executed script.', 18, 1);
                ELSE
                    -- SEVERITY 0-9 is treated as warning
                    RAISERROR ('WARNING: The hash value for the {ScriptTypeName} migration script {script.Name} does not match with the past registered and executed script.', 5, 1);
            END
        
            PRINT 'Skip {ScriptTypeName}-script {script.Name} (already executed)'
        END
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
