-- Merge columns StreetName and StreetNumber into StreetName

PRINT 'Merge StreetName and StreetNumber columns into new StreetName column'
UPDATE Customer SET StreetName=CONVERT(VARCHAR(50), CONCAT(RTRIM(StreetName), ' ', RTRIM(ISNULL(StreetNumber, ''))));

--PRINT 'Drop deprecated column StreetNumber from [dbo].[Customer]'
--ALTER TABLE Customer DROP COLUMN StreetNumber