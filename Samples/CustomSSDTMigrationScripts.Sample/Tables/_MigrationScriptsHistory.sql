CREATE TABLE [dbo].[_MigrationScriptsHistory]
(
	[ScriptNameId] NVARCHAR(255) NOT NULL PRIMARY KEY, 
	[ExecutionDate] DATETIME2 NOT NULL, 
	[ScriptHash] NVARCHAR(255) NOT NULL
)
