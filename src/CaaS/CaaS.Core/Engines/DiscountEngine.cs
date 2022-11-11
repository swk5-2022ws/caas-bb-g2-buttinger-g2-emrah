using CaaS.Core.Domainmodels;
using CaaS.Core.Interfaces.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Engines
{
    public class DiscountEngine : IDiscountEngine
    {
        public List<Discount> Discounts { get; }

        public DiscountEngine(List<Discount> discounts)
        {
            Discounts = discounts;
        }

        public void ApplyValidDiscounts(Cart cart)
        {
            if (cart == null) throw new ArgumentException($"Can not apply discounts to a null. Check parameter {nameof(cart)}.");

            var validDiscounts = Discounts
                .Where(x => x.DiscountRule.RuleObject.IsQualifiedForDiscount(cart));

            var orderedDiscounts = OrderDiscounts(validDiscounts);

            cart.Discounts = new List<Discount>();

            foreach (var discount in orderedDiscounts)
            {
                cart.Discounts.Add(discount);
            }
        }

        public double CalculateDiscountPrice(Cart cart)
        {
            if (cart.Price < 0.0) throw new ArgumentException($"Property {nameof(cart.Price)} can not be negative.");
            if (cart.Discounts == null || cart.Discounts.Count == 0) return 0.0;

            // this is redundant, but the order is important -> better save than sorry
            var orderedDiscounts = OrderDiscounts(cart.Discounts);

            double discountAmount = 0.0;
            foreach (var discount in orderedDiscounts)
            {
                discountAmount += discount.DiscountAction.ActionObject.GetDiscount(cart);
            }

            return Math.Max(cart.Price - discountAmount, 0);
        }

        private static IOrderedEnumerable<Discount> OrderDiscounts(IEnumerable<Discount> validDiscounts)
        {
            return validDiscounts
                .OrderBy(x => x.DiscountAction.ActionObject.ApplyPriority)
                .ThenBy(x => x.DiscountAction.ActionObject.SubApplyPriority);
        }
    }
}
