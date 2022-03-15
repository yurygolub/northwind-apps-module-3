CREATE PROCEDURE [dbo].GetPriceLessThanProducts
	@value money
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
	WHERE UnitPrice < @value
RETURN
