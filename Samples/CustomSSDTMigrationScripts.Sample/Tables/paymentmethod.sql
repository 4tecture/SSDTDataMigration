CREATE TABLE [dbo].[PaymentMethod]
(
	[Identifier] NCHAR(255) NOT NULL PRIMARY KEY, 
    [Description] NCHAR(255) NULL, 
    [Name] NCHAR(50) NOT NULL
)
