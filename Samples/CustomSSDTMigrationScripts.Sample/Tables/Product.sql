CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Price] INT NOT NULL, 
    [Description] NCHAR(255) NULL, 
    [Name] NCHAR(50) NOT NULL
)
