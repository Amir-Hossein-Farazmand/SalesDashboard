-- 1. Create WebAppDB
CREATE DATABASE WebAppDB;
GO

USE WebAppDB;
GO


-- 2. Create Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(50) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);


-- 3. Insert Users
INSERT INTO Users (Username, [Password], FullName, IsActive)
VALUES
(N'admin', N'Demo@123', N'System Administrator', 1),
(N'user01', N'Demo@123', N'User One', 1),
(N'user02', N'Demo@123', N'User Two', 1),
(N'user03', N'Demo@123', N'User Three', 1),
(N'user04', N'Demo@123', N'User Four', 1),
(N'user05', N'Demo@123', N'User Five', 1),
(N'user06', N'Demo@123', N'User Six', 1),
(N'user07', N'Demo@123', N'User Seven', 1),
(N'user08', N'Demo@123', N'User Eight', 1),
(N'inactive', N'Demo@123', N'Inactive Test Account', 0);
GO


-- 4. Sellers View
CREATE OR ALTER VIEW vw_Sellers
AS
SELECT
    SellerId,
    FirstName,
    LastName,
    Phone,
    City
FROM SalesDB.dbo.Sellers;
GO


-- 5. Customers View
CREATE OR ALTER VIEW vw_Customers
AS
SELECT
    CustomerId,
    FullName,
    Phone,
    City,
    Address
FROM SalesDB.dbo.Customers;
GO


-- 6. Products View
CREATE OR ALTER VIEW vw_Products
AS
SELECT
    p.ProductId,
    p.ProductName,
    p.Category,
    p.Unit,
    pl.Price AS LatestPrice,
    pl.StartDate AS LatestPriceStartDate
FROM SalesDB.dbo.Products AS p
OUTER APPLY (
    SELECT TOP 1
        Price,
        StartDate
    FROM SalesDB.dbo.PriceList
    WHERE ProductId = p.ProductId
      AND StartDate <= GETDATE()
    ORDER BY StartDate DESC, PriceId DESC
) AS pl;
GO


-- 7. Stock View
CREATE OR ALTER VIEW vw_Stock
AS
SELECT
    ProductId,
    ProductName,
    Unit,
    StockQuantity
FROM SalesDB.dbo.Products;
GO


-- 8. Sales View
CREATE OR ALTER VIEW vw_Sales
AS
SELECT
    sa.SaleId,
    sa.SellerId,
    CONCAT(se.FirstName, N' ', se.LastName) AS SellerName,
    p.ProductName,
    c.FullName AS CustomerName,
    sa.Quantity,
    sa.UnitPrice,
    CAST(sa.Quantity * sa.UnitPrice AS DECIMAL(29,0)) AS TotalAmount,
    sa.SaleDate
FROM SalesDB.dbo.Sales AS sa
INNER JOIN SalesDB.dbo.Sellers AS se
    ON sa.SellerId = se.SellerId
INNER JOIN SalesDB.dbo.Customers AS c
    ON sa.CustomerId = c.CustomerId
INNER JOIN SalesDB.dbo.Products AS p
    ON sa.ProductId = p.ProductId;
GO


