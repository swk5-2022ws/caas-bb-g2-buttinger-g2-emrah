using CaaS.Core.Domainmodels.DiscountActions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Tools.Test.Data.Sql.InsertGenerator
{
    internal class DiscountActionUpdateSqlGenerator
    {
        readonly Random random = new();

        internal IEnumerable<string> GenerateActionsForExistingDiscountActions()
        {
            int count = 200;
            return GenerateDiscountActionsUpdateStatement(count);
        }

        private IEnumerable<string> GenerateDiscountActionsUpdateStatement(int count)
        {
            List<string> discountActions = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                int type = i % 2;
                DiscountActionBase? action = null;
                action = type switch
                {
                    0 => new FixedValueDiscountAction(random.Next(5, 100)),
                    1 => new TotalPercentageDiscountAction(random.NextDouble()),
                    _ => throw new ArgumentOutOfRangeException($"type {type} not implemented."),
                };
                string json = DiscountActionBase.Serialize(action);

                string sql = $"UPDATE DiscountAction SET Action = '{json}' WHERE Id = {i};";
                yield return sql;
            }
        }
    }
}
