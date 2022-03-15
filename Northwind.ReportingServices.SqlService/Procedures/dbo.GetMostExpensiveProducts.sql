CREATE PROCEDURE [dbo].GetMostExpensiveProducts
	@count int
AS
	SET ROWCOUNT @count
	SELECT
		ProductID,
		ProductName,
		SupplierID,
		CategoryID,
		QuantityPerUnit,
		UnitPrice,
		UnitsInStock,
		UnitsOnOrder,
		ReorderLevel,
		Discontinued
	FROM Products
	ORDER BY UnitPrice DESC
RETURN
