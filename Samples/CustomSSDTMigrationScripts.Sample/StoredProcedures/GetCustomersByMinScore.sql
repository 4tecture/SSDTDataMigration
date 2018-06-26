CREATE PROCEDURE [dbo].[GetCustomersByMinScore]
	@score decimal(18,2)
AS
	DECLARE @CScoreCount int
	SELECT @CScoreCount=COUNT(*) FROM Customer WHERE Score > @score

RETURN 3
