﻿using CaaS.Common.Mappings;
using CaaS.Core.Domainmodels;
using System.Data;

namespace CaaS.Core.Repository.Mappings
{
    internal static class CustomerMapping 
    {
        internal static Customer ReadCustomerOnly(IDataRecord record) =>
            new (record.GetIntByName(nameof(Customer.Id)), 
                 record.GetIntByName(nameof(Customer.ShopId)), 
                 record.GetStringByName(nameof(Customer.Name)), 
                 record.GetStringByName(nameof(Customer.Email)),
                 record.GetNullableIntByName(nameof(Customer.CartId)),
                 record.GetNullableStringByName(nameof(Customer.CreditCardNumber)),
                 record.GetNullableStringByName(nameof(Customer.CVV)),
                 record.GetNullableStringByName(nameof(Customer.Expiration)))
            {
                Deleted = record.GetNullableDateTimeByName(nameof(Customer.Deleted))
            };
    }
}
