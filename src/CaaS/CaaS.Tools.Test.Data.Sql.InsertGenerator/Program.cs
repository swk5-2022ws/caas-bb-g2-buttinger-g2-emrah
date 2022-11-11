using CaaS.Core.Domainmodels;
using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Tools.Test.Data.Sql.InsertGenerator;
using System.Security.Cryptography;


//var update = new DiscountActionUpdateSqlGenerator().GenerateActionsForExistingDiscountActions();
//Console.WriteLine(string.Join(" ", update));

var update = new DiscountRuleUpdateSqlGenerator().GenerateRulesForExistingDiscountRules();
Console.WriteLine(string.Join(" ", update));
