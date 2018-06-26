SET NOCOUNT ON

MERGE INTO [dbo].[PaymentMethod] AS Target
USING (VALUES
  (N'creditcard',N'The credit card payment method',N'Credit Card')
 ,(N'debitcard',N'The debit card payment method',N'Debit Card')
) AS Source ([Identifier],[Description],[Name])
ON (Target.[Identifier] = Source.[Identifier])
WHEN MATCHED AND (
	NULLIF(Source.[Description], Target.[Description]) IS NOT NULL OR NULLIF(Target.[Description], Source.[Description]) IS NOT NULL OR 
	NULLIF(Source.[Name], Target.[Name]) IS NOT NULL OR NULLIF(Target.[Name], Source.[Name]) IS NOT NULL) THEN
 UPDATE SET
  [Description] = Source.[Description], 
  [Name] = Source.[Name]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([Identifier],[Description],[Name])
 VALUES(Source.[Identifier],Source.[Description],Source.[Name])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE
;

DECLARE @mergeError int
 , @mergeCount int
SELECT @mergeError = @@ERROR, @mergeCount = @@ROWCOUNT
IF @mergeError != 0
 BEGIN
 PRINT 'ERROR OCCURRED IN MERGE FOR [dbo].[PaymentMethod]. Rows affected: ' + CAST(@mergeCount AS VARCHAR(100)); -- SQL should always return zero rows affected
 END
ELSE
 BEGIN
 PRINT '[dbo].[PaymentMethod] rows affected by MERGE: ' + CAST(@mergeCount AS VARCHAR(100));
 END

