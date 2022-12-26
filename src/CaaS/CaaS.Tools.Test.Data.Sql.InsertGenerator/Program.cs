using CaaS.Tools.Test.Data.Sql.InsertGenerator;


//var update = new DiscountActionUpdateSqlGenerator().GenerateActionsForExistingDiscountActions();
//Console.WriteLine(string.Join(" ", update));

//var update = new DiscountRuleUpdateSqlGenerator().GenerateRulesForExistingDiscountRules();
//Console.WriteLine(string.Join(" ", update));

//var update = new CustomerUpdateSqlGenerator().GenerateCartIdForExistingCustomers();
//Console.WriteLine(string.Join(" ", update));

//var update = new CouponUpdateSqlGenerator().GenerateCouponKeyForExistingCoupons();
//var update = new CouponUpdateSqlGenerator().RemoveCartIdForLast10CouponsForExistingCoupons();
var update = new CustomerUpdateSqlGenerator().GenerateCreditInformationForExistingCustomers();
Console.WriteLine(string.Join(" ", update));
