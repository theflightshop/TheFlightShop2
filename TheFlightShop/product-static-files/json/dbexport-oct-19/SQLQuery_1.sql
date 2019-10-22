SELECT TOP (1000) [uid]
      ,[AccessLevel]
      ,[WebName]
      ,[Code]
      ,[ProductOrder]
      ,[ManufacturerId]
      ,[VendorId]
      ,[IsActive]
      ,[Name]
      ,[NamePlural]
      ,[ShortDescription]
      ,[Description]
      ,[UpSellMessage]
      ,[ImageSmallPath]
      ,[ImageLargePath]
      ,[ImageFilesID]
      ,[FileName]
      ,[Cost]
      ,[Price]
      ,[ToBeQuoted]
      ,[MinOrderQty]
      ,[OrderQty]
      ,[IsOnSale]
      ,[SalePrice]
      ,[IsShipable]
      ,[ShipPrice]
      ,[Weight]
      ,[Length]
      ,[Width]
      ,[Height]
      ,[HasCountryTax]
      ,[HasStateTax]
      ,[HasLocalTax]
      ,[DateAdded]
      ,[DateModified]
      ,[Keywords]
      ,[DetailLink]
      ,[Inventory_Tracked]
      ,[DropShip]
      ,[DownloadOneTime]
      ,[DownloadExpire]
      ,[DealTimeIsActive]
      ,[MMIsActive]
      ,[MetaPageTitle]
      ,[MetaDescription]
      ,[MetaKeywords]
  FROM [FlightShop].[dbo].[Products]
  order by ProductOrder

  -- 1. query product/category data and export to CSV
  -- 2. write/run script to import CSV and transform data (e.g. assign guids, etc) 
  -- 2a. finish script w/ saving data to DB (have dict in transform step: dict<int, order> and dict<int, category> and 
  --    then save .Values)

  select p.uid as ProductId, p.IsActive, c.uid as CategoryId, c.Name as CategoryName, p.Code, p.ShortDescription, p.Price
  from Products p 
  inner join ProductCategory pc on p.uid = pc.ProductID
  inner join Categories c on pc.CategoryID = c.uid 