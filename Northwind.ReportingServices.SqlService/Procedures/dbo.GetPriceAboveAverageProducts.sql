CREATE PROCEDURE [dbo].GetPriceAboveAverageProducts
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
	WHERE UnitPrice > ALL (SELECT AVG(UnitPrice) FROM Products);
RETURN
