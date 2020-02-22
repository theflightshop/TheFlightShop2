CREATE TABLE `contacts` (
  `Id` char(38) NOT NULL,
  `FirstName` varchar(100) DEFAULT NULL,
  `LastName` varchar(100) DEFAULT NULL,
  `Email` varchar(320) DEFAULT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `Address1` varchar(255) DEFAULT NULL,
  `Address2` varchar(100) DEFAULT NULL,
  `City` varchar(100) DEFAULT NULL,
  `State` varchar(55) DEFAULT NULL,
  `Zip` varchar(15) DEFAULT NULL,
  `DateCreated` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ;

CREATE TABLE `orders` (
  `Id` char(38) NOT NULL,
  `ContactId` char(38) NOT NULL,
  `Completed` bit(1) NOT NULL,
  `Shipped` bit(1) NOT NULL,
  `DateCreated` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `orders_fk_contacts` (`ContactId`),
  CONSTRAINT `orders_fk_contacts` FOREIGN KEY (`ContactId`) REFERENCES `contacts` (`Id`)
);

CREATE TABLE `categories` (
  `Id` char(38) NOT NULL,
  `IsActive` bit(1) NOT NULL,
  `CategoryId` char(38) DEFAULT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `ImageFilename` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ;

CREATE TABLE `products` (
  `Id` char(38) NOT NULL,
  `IsActive` bit(1) NOT NULL,
  `CategoryId` char(38) NOT NULL,
  `SubCategoryId` char(38) DEFAULT NULL,
  `Code` varchar(100) DEFAULT NULL,
  `ShortDescription` varchar(255) DEFAULT NULL,
  `LongDescription` varchar(1000) DEFAULT NULL,
  `MostPopular` bit(1) NOT NULL,
  `NumberOfInstallationExamples` int NOT NULL,
  `DrawingUrl` varchar(512) DEFAULT NULL,
  `ImageFilename` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `DrawingFilename` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `CategoryId` (`CategoryId`),
  KEY `SubCategoryId` (`SubCategoryId`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`Id`),
  CONSTRAINT `products_ibfk_2` FOREIGN KEY (`SubCategoryId`) REFERENCES `categories` (`Id`)
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

CREATE TABLE `parts` (
  `Id` char(38) NOT NULL,
  `IsActive` bit(1) NOT NULL,
  `ProductId` char(38) NOT NULL,
  `PartNumber` varchar(100) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Price` decimal(14,2) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `ProductId` (`ProductId`),
  CONSTRAINT `parts_ibfk_1` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`)
);



