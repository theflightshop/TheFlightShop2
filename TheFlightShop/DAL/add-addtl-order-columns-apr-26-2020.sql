# NOTE: deleted tables and recreated w/ addt'l columns because customers weren't yet using the site.

CREATE TABLE `orders` (
  `Id` char(38) NOT NULL,
  `ContactId` char(38) NOT NULL,
  `Completed` bit(1) NOT NULL,
  `Shipped` bit(1) NOT NULL,
  `DateCreated` datetime NOT NULL,
  `ShippingType` smallint not null,
  `PurchaseOrderNumber` nvarchar(55) default null,
  `Notes` nvarchar(1024) default null,
  PRIMARY KEY (`Id`),
  KEY `orders_fk_contacts` (`ContactId`),
  CONSTRAINT `orders_fk_contacts` FOREIGN KEY (`ContactId`) REFERENCES `contacts` (`Id`)
);

CREATE TABLE `orderlines` (
  `Id` char(38) NOT NULL,
  `OrderId` char(38) NOT NULL,
  `ProductId` char(38) NOT NULL,
  `PartId` char(38) DEFAULT NULL,
  `PartNumber` varchar(100) NOT NULL,
  `Quantity` int NOT NULL,
  `DateCreated` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `orderLines_fk_orders` (`OrderId`),
  CONSTRAINT `orderLines_fk_orders` FOREIGN KEY (`OrderId`) REFERENCES `orders` (`Id`)
);