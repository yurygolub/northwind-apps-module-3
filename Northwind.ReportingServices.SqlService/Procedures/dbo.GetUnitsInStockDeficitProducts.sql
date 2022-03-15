CREATE PROCEDURE [dbo].GetUnitsInStockDeficitProducts
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
	WHERE UnitsInStock < UnitsOnOrder
RETURN
