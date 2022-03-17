GO
CREATE PROCEDURE [dbo].[GetAllProducts ]
AS
	SELECT * FROM Products 

GO
CREATE PROCEDURE [dbo].[GetMostExpensiveProducts]
	@count int
AS
	SET ROWCOUNT @count
	SELECT * FROM Products
	ORDER BY UnitPrice DESC

GO
CREATE PROCEDURE [dbo].GetPriceBetweenProducts
	@value1 money,
	@value2 money
AS
	SELECT * FROM Products
	WHERE UnitPrice BETWEEN @value1 AND @value2

GO
CREATE PROCEDURE [dbo].GetPriceAboveAverageProducts
AS
	SELECT * FROM Products
	WHERE UnitPrice > ALL (SELECT AVG(UnitPrice) FROM Products);

GO
CREATE PROCEDURE [dbo].GetPriceLessThanProducts
	@value money
AS
	SELECT * FROM Products
	WHERE UnitPrice < @value

GO
CREATE PROCEDURE [dbo].GetUnitsInStockDeficitProducts
AS
	SELECT * FROM Products
	WHERE UnitsInStock < UnitsOnOrder