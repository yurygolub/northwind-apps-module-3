CREATE PROCEDURE [dbo].GetPriceBetweenProducts
	@value1 money,
	@value2 money
AS
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
	WHERE UnitPrice BETWEEN @value1 AND @value2
RETURN
