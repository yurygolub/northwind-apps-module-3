﻿CREATE PROCEDURE dbo.GetAllProducts 
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
RETURN
