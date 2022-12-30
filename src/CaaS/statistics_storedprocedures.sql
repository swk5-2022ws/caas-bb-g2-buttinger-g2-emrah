DELIMITER $$
CREATE PROCEDURE `sp_GetAverageCartPriceInPeriod`(IN pshopid int, IN pstartdate datetime, IN penddate datetime)
BEGIN
    SELECT cu.ShopId, AVG(pc.Price * pc.Amount) as AverageCartPrice
    FROM `Order` o
    INNER JOIN Cart c on c.Id = o.CartId AND c.CustomerId IS NOT NULL
    INNER JOIN Customer cu on cu.Id = c.Id AND cu.ShopId = pshopid
    INNER JOIN ProductCart pc on pc.CartId = c.Id
    WHERE o.OrderDate > pstartdate AND o.OrderDate < penddate
    GROUP BY cu.ShopId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `sp_GetCartOrderCount`(IN pshopid int)
BEGIN

    SELECT sum((case when o.id is not null then 1 else 0 end)) as OrderCount, count(ca.Id) as CartCount, c.ShopId as ShopId FROM Customer c inner join Cart ca on ca.CustomerId = c.Id left JOIN `Order` o on o.CartId = ca.Id group by c.ShopId having c.Shopid = pshopid;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `sp_GetCouponStatistics`(IN pshopid int)
BEGIN
    SELECT Count(*) FROM Coupon WHERE ShopId = pshopid AND CartId IS NOT NULL AND Deleted IS NULL;
END$$
DELIMITER ;
