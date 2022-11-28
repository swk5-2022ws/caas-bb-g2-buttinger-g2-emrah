-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Host: db
-- Erstellungszeit: 19. Okt 2022 um 17:37
-- Server-Version: 8.0.31
-- PHP-Version: 8.0.19

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Datenbank: `caas`
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Cart`
--

CREATE TABLE `Cart` (
  `Id` int NOT NULL,
  `CustomerId` int DEFAULT NULL,
  `SessionId` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Coupon`
--

CREATE TABLE `Coupon` (
  `Id` int NOT NULL,
  `ShopId` int NOT NULL,
  `CartId` int DEFAULT NULL,
  `Value` double NOT NULL,
  `Deleted` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Customer`
--

CREATE TABLE `Customer` (
  `Id` int NOT NULL,
  `ShopId` int NOT NULL,
  `Email` varchar(500) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Deleted` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Discount`
--

CREATE TABLE `Discount` (
  `Id` int NOT NULL,
  `RuleId` int NOT NULL,
  `ActionId` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `DiscountAction`
--

CREATE TABLE `DiscountAction` (
  `Id` int NOT NULL,
  `ShopId` int NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Value` double NOT NULL,
  `ActionType` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `DiscountCart`
--

CREATE TABLE `DiscountCart` (
  `CartId` int NOT NULL,
  `DiscountId` int NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `DiscountRule`
--

CREATE TABLE `DiscountRule` (
  `Id` int NOT NULL,
  `ShopId` int NOT NULL,
  `Name` varchar(100) NOT NULL,
  `Ruleset` varchar(50000) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Order`
--

CREATE TABLE `Order` (
  `Id` int NOT NULL,
  `CartId` int NOT NULL,
  `OrderDate` datetime NOT NULL,
  `Discount` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Product`
--

CREATE TABLE `Product` (
  `Id` int NOT NULL,
  `ShopId` int NOT NULL,
  `Label` varchar(100) NOT NULL,
  `Description` varchar(5000) DEFAULT NULL,
  `Price` double NOT NULL,
  `ImageUrl` varchar(200) NOT NULL,
  `Deleted` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `ProductCart`
--

CREATE TABLE `ProductCart` (
  `ProductId` int NOT NULL,
  `CartId` int NOT NULL,
  `Amount` int NOT NULL,
  `Price` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Shop`
--

CREATE TABLE `Shop` (
  `Id` int NOT NULL,
  `TenantId` int NOT NULL,
  `Label` varchar(50) NOT NULL,
  `AppKey` char(36) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle `Tenant`
--

CREATE TABLE `Tenant` (
  `Id` int NOT NULL,
  `Email` varchar(500) NOT NULL,
  `Name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle `Cart`
--
ALTER TABLE `Cart`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_Cart_Customer` (`CustomerId`);

--
-- Indizes für die Tabelle `Coupon`
--
ALTER TABLE `Coupon`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_Coupon_Shop` (`ShopId`),
  ADD KEY ` FK_Coupon_Cart` (`CartId`);

--
-- Indizes für die Tabelle `Customer`
--
ALTER TABLE `Customer`
  ADD PRIMARY KEY (`Id`);

--
-- Indizes für die Tabelle `Discount`
--
ALTER TABLE `Discount`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_Discount_DiscountAction` (`ActionId`),
  ADD KEY `FK_Discount_DiscountRule` (`RuleId`);

--
-- Indizes für die Tabelle `DiscountAction`
--
ALTER TABLE `DiscountAction`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK:_DiscountAction_Shop` (`ShopId`);

--
-- Indizes für die Tabelle `DiscountCart`
--
ALTER TABLE `DiscountCart`
  ADD PRIMARY KEY (`CartId`,`DiscountId`),
  ADD KEY `FK_DiscountCart_Discount` (`DiscountId`);

--
-- Indizes für die Tabelle `DiscountRule`
--
ALTER TABLE `DiscountRule`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_DiscountRule_Shop` (`ShopId`);

--
-- Indizes für die Tabelle `Order`
--
ALTER TABLE `Order`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_Order_Cart` (`CartId`);

--
-- Indizes für die Tabelle `Product`
--
ALTER TABLE `Product`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_Product_Shop` (`ShopId`);

--
-- Indizes für die Tabelle `ProductCart`
--
ALTER TABLE `ProductCart`
  ADD PRIMARY KEY (`ProductId`,`CartId`),
  ADD KEY `FK_ProductCart_Cart` (`CartId`);

--
-- Indizes für die Tabelle `Shop`
--
ALTER TABLE `Shop`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FK_Shop_Tenant` (`TenantId`);

--
-- Indizes für die Tabelle `Tenant`
--
ALTER TABLE `Tenant`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle `Cart`
--
ALTER TABLE `Cart`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `Coupon`
--
ALTER TABLE `Coupon`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `Customer`
--
ALTER TABLE `Customer`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `Discount`
--
ALTER TABLE `Discount`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `DiscountAction`
--
ALTER TABLE `DiscountAction`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `DiscountRule`
--
ALTER TABLE `DiscountRule`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `Order`
--
ALTER TABLE `Order`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `Product`
--
ALTER TABLE `Product`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `Shop`
--
ALTER TABLE `Shop`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT für Tabelle `Tenant`
--
ALTER TABLE `Tenant`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT;

--
-- Constraints der exportierten Tabellen
--

ALTER TABLE `Customer`
  ADD CONSTRAINT `FK_Customer_Shop` FOREIGN KEY (`ShopId`) REFERENCES `Shop` (`Id`);

--
-- Constraints der Tabelle `Cart`
--
ALTER TABLE `Cart`
  ADD CONSTRAINT `FK_Cart_Customer` FOREIGN KEY (`CustomerId`) REFERENCES `Customer` (`Id`);

--
-- Constraints der Tabelle `Coupon`
--
ALTER TABLE `Coupon`
  ADD CONSTRAINT ` FK_Coupon_Cart` FOREIGN KEY (`CartId`) REFERENCES `Cart` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  ADD CONSTRAINT `FK_Coupon_Shop` FOREIGN KEY (`ShopId`) REFERENCES `Shop` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Constraints der Tabelle `Discount`
--
ALTER TABLE `Discount`
  ADD CONSTRAINT `FK_Discount_DiscountAction` FOREIGN KEY (`ActionId`) REFERENCES `DiscountAction` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  ADD CONSTRAINT `FK_Discount_DiscountRule` FOREIGN KEY (`RuleId`) REFERENCES `DiscountRule` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Constraints der Tabelle `DiscountAction`
--
ALTER TABLE `DiscountAction`
  ADD CONSTRAINT `FK:_DiscountAction_Shop` FOREIGN KEY (`ShopId`) REFERENCES `Shop` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Constraints der Tabelle `DiscountCart`
--
ALTER TABLE `DiscountCart`
  ADD CONSTRAINT `FK_DiscountCart_Cart` FOREIGN KEY (`CartId`) REFERENCES `Cart` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  ADD CONSTRAINT `FK_DiscountCart_Discount` FOREIGN KEY (`DiscountId`) REFERENCES `Discount` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Constraints der Tabelle `DiscountRule`
--
ALTER TABLE `DiscountRule`
  ADD CONSTRAINT `FK_DiscountRule_Shop` FOREIGN KEY (`ShopId`) REFERENCES `Shop` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Constraints der Tabelle `Order`
--
ALTER TABLE `Order`
  ADD CONSTRAINT `FK_Order_Cart` FOREIGN KEY (`CartId`) REFERENCES `Cart` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT;

--
-- Constraints der Tabelle `Product`
--
ALTER TABLE `Product`
  ADD CONSTRAINT `FK_Product_Shop` FOREIGN KEY (`ShopId`) REFERENCES `Shop` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Constraints der Tabelle `ProductCart`
--
ALTER TABLE `ProductCart`
  ADD CONSTRAINT `FK_ProductCart_Cart` FOREIGN KEY (`CartId`) REFERENCES `Cart` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT,
  ADD CONSTRAINT `FK_ProductCart_Product` FOREIGN KEY (`ProductId`) REFERENCES `Product` (`Id`) ON DELETE CASCADE ON UPDATE RESTRICT;

--
-- Constraints der Tabelle `Shop`
--
ALTER TABLE `Shop`
  ADD CONSTRAINT `FK_Shop_Tenant` FOREIGN KEY (`TenantId`) REFERENCES `Tenant` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

/* CREATE USER 'service' IDENTIFIED WITH mysql_native_password BY 'mypass123';GRANT USAGE ON *.* TO 'service';ALTER USER 'service' REQUIRE NONE WITH MAX_QUERIES_PER_HOUR 0 MAX_CONNECTIONS_PER_HOUR 0 MAX_UPDATES_PER_HOUR 0 MAX_USER_CONNECTIONS 0;GRANT ALL PRIVILEGES ON `caas`.* TO 'service'; */