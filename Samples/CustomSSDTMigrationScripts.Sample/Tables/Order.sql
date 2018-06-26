CREATE TABLE [dbo].[Order]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [CustomerId] INT NOT NULL, 
    [ProductId] INT NOT NULL, 
    [Quantity] INT NOT NULL,
	[Comment] NVARCHAR(MAX) NULL, 
    [OrderDate] DATETIME2 NULL, 
    constraint [fk_customers] foreign key ([CustomerId]) references Customer(Id),
	constraint [fk_products] foreign key ([ProductId]) references Product(Id) 
)
