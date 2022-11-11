using CaaS.Core.Domainmodels.DiscountActions;
using CaaS.Core.Domainmodels.DiscountRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Tools.Test.Data.Sql.InsertGenerator
{
    internal class DiscountRuleUpdateSqlGenerator
    {
         readonly Random random = new();

        internal IEnumerable<string> GenerateRulesForExistingDiscountRules()
        {
            int count = 100;
            return GenerateDiscountRuleUpdateStatement(count);
        }

        private IEnumerable<string> GenerateDiscountRuleUpdateStatement(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                int type = i % 2;

                DiscountRulesetBase action = type switch
                {
                    0 => GenerateDateDiscountRuleset(),
                    1 => new TotalAmountDiscountRuleset(random.Next(5, 800)),
                    _ => throw new ArgumentOutOfRangeException($"type {type} not implemented."),
                };
                string json = DiscountRulesetBase.Serialize(action);

                string sql = $"UPDATE DiscountRule SET Ruleset = '{json}' WHERE Id = {i};";
                yield return sql;
            }
        }

        private DiscountRulesetBase GenerateDateDiscountRuleset()
        {
            int monthBeginDelta = random.Next(0, 12);
            int monthEndDelta = random.Next(monthBeginDelta, 24);
            return new DateDiscountRuleset(DateTime.Now.AddMonths(-monthBeginDelta), DateTime.Now.AddDays(monthEndDelta), DateTime.Now);
        }
    }
}
