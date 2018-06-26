PRINT 'Start initializing database data'

-- Products
PRINT 'Initialize product data...'

INSERT INTO dbo.product (Id, Price, Description, Name)
SELECT 1, 5, NULL, 'Surface Book'
WHERE NOT EXISTS (SELECT 1 FROM dbo.product WHERE Id = 1)

INSERT INTO dbo.product (Id, Price, Description, Name)
SELECT 2, 5, 'Microsoft Surface Pen', 'Surface Pen'
WHERE NOT EXISTS (SELECT 1 FROM dbo.product WHERE Id = 2)

INSERT INTO dbo.product (Id, Price, Description, Name)
SELECT 3, 5, NULL, 'Suraface Mouse'
WHERE NOT EXISTS (SELECT 1 FROM dbo.product WHERE Id = 3)

-- Customers
PRINT 'Initialize customer data...'

INSERT INTO dbo.customer (Id, FirstName, LastName, StreetName, City, StreetNumber)
SELECT 1, 'Brian', 'Herry', 'Anystreet', 'Zuric', '3'
WHERE NOT EXISTS (SELECT 1 FROM dbo.customer WHERE Id = 1)

INSERT INTO dbo.customer (Id, FirstName, LastName, StreetName, City, StreetNumber)
SELECT 2, 'Bara', 'Nellson', 'Topstreet', 'Madrid', '100'
WHERE NOT EXISTS (SELECT 1 FROM dbo.customer WHERE Id = 2)

INSERT INTO dbo.customer (Id, FirstName, LastName, StreetName, City, StreetNumber)
SELECT 3, 'Tom', 'Johnson', null, 'London', null
WHERE NOT EXISTS (SELECT 1 FROM dbo.customer WHERE Id = 3)

-- Orders 
PRINT 'Initialize order data...'

INSERT INTO dbo.[order] (Id, CustomerId, ProductId, Quantity, Comment, OrderDate)
SELECT 1, 1, 1, 1, 'Shipt as one package', GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.[order] WHERE Id = 1)

INSERT INTO dbo.[order] (Id, CustomerId, ProductId, Quantity, Comment, OrderDate)
SELECT 2, 1, 2, 2, NULL, GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.[order] WHERE Id = 2)

INSERT INTO dbo.[order] (Id, CustomerId, ProductId, Quantity, Comment, OrderDate)
SELECT 3, 2, 3, 4, NULL, GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM dbo.[order] WHERE Id = 3)

PRINT 'Finished initializing database data'