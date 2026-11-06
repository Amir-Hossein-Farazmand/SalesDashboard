USE SalesDB;

-- 1. Insert Sellers
INSERT INTO Sellers (FirstName, LastName, Phone, City)
VALUES
(N'Ali', N'Rezaei', N'09120000001', N'Tehran'),
(N'Sara', N'Mohammadi', N'09120000002', N'Tehran'),
(N'Reza', N'Karimi', N'09130000001', N'Isfahan'),
(N'Mina', N'Ahmadi', N'09170000001', N'Shiraz'),
(N'Mohammad', N'Hosseini', N'09150000001', N'Mashhad'),
(N'Negar', N'Jafari', N'09140000001', N'Tabriz'),
(N'Amir', N'Moradi', N'09120000003', N'Tehran'),
(N'Parisa', N'Rahimi', N'09120000004', N'Karaj'),
(N'Hamed', N'Yousefi', N'09160000001', N'Ahvaz'),
(N'Zahra', N'Ebrahimi', N'09150000002', N'Mashhad');


-- 2. Insert Customers
INSERT INTO Customers (FullName, Phone, City, Address)
VALUES
(N'Paya Energy Co.', N'02144000001', N'Tehran', N'Azadi Street, No. 10'),
(N'Novin Office Group', N'02144000002', N'Tehran', N'Valiasr Street, No. 25'),
(N'Pars Data Center', N'03132000003', N'Isfahan', N'Chaharbagh Street, No. 8'),
(N'Shiraz Medical Center', N'07136000004', N'Shiraz', N'Zand Boulevard, No. 17'),
(N'Atlas Trading', N'05137000005', N'Mashhad', N'Imam Reza Street, No. 31'),
(N'Tabriz Industrial Co.', N'04135000006', N'Tabriz', N'Valiasr Avenue, No. 6'),
(N'Kian Computer Shop', N'02144000007', N'Tehran', N'Enghelab Street, No. 42'),
(N'Faraz Electronics', N'02632000008', N'Karaj', N'Gohardasht, No. 13'),
(N'Persia Telecom', N'06133000009', N'Ahvaz', N'Naderi Street, No. 19'),
(N'Rayan Solutions', N'02532000010', N'Qom', N'Safaeieh, No. 4');


-- 3. Insert Products
INSERT INTO Products (ProductName, Category, Unit, StockQuantity)
VALUES
(N'UPS 1KVA', N'UPS', N'Piece', 24),
(N'UPS 2KVA', N'UPS', N'Piece', 12),
(N'UPS Online 3KVA', N'UPS', N'Piece', 7),
(N'Stabilizer 5KVA', N'Stabilizer', N'Piece', 18),
(N'Stabilizer 10KVA', N'Stabilizer', N'Piece', 4),
(N'Battery 12V 9Ah', N'Battery', N'Piece', 65),
(N'Battery 12V 65Ah', N'Battery', N'Piece', 9),
(N'Inverter 1000W', N'Inverter', N'Piece', 11),
(N'Battery Charger 10A', N'Charger', N'Piece', 30),
(N'Automatic Voltage Regulator', N'Stabilizer', N'Piece', 6),
(N'Battery Cabinet', N'Accessory', N'Piece', 3),
(N'Transfer Switch 63A', N'Accessory', N'Piece', 15);


-- 4. Insert PriceList
INSERT INTO PriceList (ProductId, Price, StartDate)
VALUES
(1, 72000000, '2025-01-01'),
(1, 79000000, '2026-01-15'),
(2, 118000000, '2025-03-01'),
(2, 132000000, '2026-02-01'),
(3, 210000000, '2026-01-10'),
(4, 48000000, '2025-02-10'),
(4, 55000000, '2026-03-01'),
(5, 94000000, '2026-02-15'),
(6, 4500000, '2025-01-01'),
(6, 5200000, '2026-01-01'),
(7, 16500000, '2026-01-05'),
(8, 87000000, '2026-02-20'),
(9, 12500000, '2025-06-01'),
(9, 14800000, '2026-04-01'),
(10, 62000000, '2026-03-10'),
(11, 35000000, '2026-01-20'),
(12, 18000000, '2025-05-01'),
(12, 20500000, '2026-05-01'),
(3, 198000000, '2025-04-01'),
(7, 15200000, '2025-07-01');


-- 5. Insert Sales
INSERT INTO Sales (SellerId, CustomerId, ProductId, Quantity, UnitPrice, SaleDate)
VALUES
(1, 1, 1, 2, 74000000, '2026-01-18'),
(2, 2, 6, 8, 4800000, '2026-01-20'),
(3, 3, 2, 1, 125000000, '2026-02-05'),
(4, 4, 4, 3, 51000000, '2026-02-11'),
(5, 5, 7, 4, 15800000, '2026-02-16'),
(6, 6, 3, 1, 205000000, '2026-03-03'),
(7, 7, 9, 5, 13200000, '2026-03-12'),
(8, 8, 8, 2, 85000000, '2026-03-19'),
(9, 9, 5, 1, 90000000, '2026-04-02'),
(10, 10, 10, 2, 60000000, '2026-04-10'),
(1, 2, 11, 1, 35000000, '2026-04-17'),
(2, 1, 12, 3, 19000000, '2026-05-04'),
(3, 3, 1, 4, 79000000, '2026-05-12'),
(4, 4, 6, 10, 5200000, '2026-05-21'),
(6, 6, 2, 2, 132000000, '2026-06-02');
