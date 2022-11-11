using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CaaS.Core.Domainmodels.DiscountRules
{
    /// <summary>
    /// DiscountRuleset which triggers if the current date is between a start and end date.
    /// </summary>
    [Serializable]
    public class DateDiscountRuleset : DiscountRulesetBase
    {
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public DateTime OrderDate { get; init; }

        [JsonConstructor]
        public DateDiscountRuleset(DateTime startDate, DateTime endDate, DateTime orderDate)
        {
            if (startDate >= endDate) throw new ArgumentException("End date must be after start date.");

            StartDate = startDate;
            EndDate = endDate;
            OrderDate = orderDate;
        }

        public DateDiscountRuleset(SerializationInfo info, StreamingContext context)
        {
            StartDate = (DateTime)(info.GetValue(nameof(StartDate), typeof(DateTime)) 
                ?? throw new SerializationException($"Can not deserialize null value of parameter {nameof(StartDate)}"));
            EndDate = (DateTime)(info.GetValue(nameof(EndDate), typeof(DateTime)) 
                ?? throw new SerializationException($"Can not deserialize null value of parameter {nameof(EndDate)}"));
            OrderDate = (DateTime)(info.GetValue(nameof(OrderDate), typeof(DateTime)) 
                ?? throw new SerializationException($"Can not deserialize null value of parameter {nameof(OrderDate)}"));
        }

        public override bool IsQualifiedForDiscount(Cart cart)
        {
            return StartDate <= OrderDate && EndDate >= OrderDate;
        }


        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(StartDate), StartDate, typeof(DateTime));
            info.AddValue(nameof(EndDate), EndDate, typeof(DateTime));
            info.AddValue(nameof(OrderDate), OrderDate, typeof(DateTime));
        }
    }
}
