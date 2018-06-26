CREATE TABLE [dbo].[Customer]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FirstName] NCHAR(50) NOT NULL, 
    [LastName] NCHAR(50) NOT NULL, 
    [StreetName] VARCHAR(100) NULL, 
    [City] NCHAR(50) NULL, 
    [StreetNumber] VARCHAR(5) NULL, 
    [IsActive] BIT NULL,
    [Email] NCHAR(255) NOT NULL DEFAULT '-', 
    [Score] DECIMAL(18, 1) NOT NULL DEFAULT 0, 
    [GenderRen] NCHAR(10) NULL DEFAULT 'none' 
)
