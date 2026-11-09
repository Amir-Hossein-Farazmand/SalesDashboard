-- Create the business schema in a fresh database. Do not rerun against an existing database.
CREATE DATABASE SalesDB;
GO

USE SalesDB;

-- Sellers Table
CREATE TABLE Sellers (
    SellerId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Phone NVARCHAR(15) NOT NULL,
    City NVARCHAR(50) NOT NULL
);

-- Products Table
CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    StockQuantity INT NOT NULL CHECK (StockQuantity >= 0)
);

-- Customers Table
CREATE TABLE Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15) NOT NULL,
    City NVARCHAR(50) NOT NULL,
    Address NVARCHAR(200) NOT NULL
);

-- PriceList Table
CREATE TABLE PriceList (
    PriceId INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    Price DECIMAL(18,0) NOT NULL CHECK (Price >= 0),
    StartDate DATE NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- Sales Table
CREATE TABLE Sales (
    SaleId INT IDENTITY(1,1) PRIMARY KEY,
    SellerId INT NOT NULL,
    CustomerId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18,0) NOT NULL CHECK (UnitPrice >= 0),
    SaleDate DATE NOT NULL,
    FOREIGN KEY (SellerId) REFERENCES Sellers(SellerId),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
