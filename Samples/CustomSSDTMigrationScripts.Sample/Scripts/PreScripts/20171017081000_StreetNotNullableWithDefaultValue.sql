-- Update NULL entries with default values

IF OBJECT_ID(N'dbo.Customer', N'U') IS NOT NULL
BEGIN
	UPDATE
	  Customer
	SET
	  StreetName = 'undefined'
	WHERE
	  StreetName IS NULL;
END
