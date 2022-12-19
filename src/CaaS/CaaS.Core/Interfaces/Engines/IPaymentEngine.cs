using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Interfaces.Engines
{
    public interface IPaymentEngine
    {
        /// <summary>
        /// The payment of an order is done using this interface.
        /// Everything passed to the engine is encrypted. 
        /// The engine is doing the actual encryption of the data
        /// </summary>
        /// <param name="creditCardNumber">the credit card number</param>
        /// <param name="cvv">the cvv</param>
        /// <param name="expiration">the expiration date in format MM/YY</param>
        /// <returns>True if the payment could be handled else it would return either an exception or false if the amount could not be booked from the creditCard</returns>
        bool Payment(double amount, string creditCardNumber, string cvv, string expiration);
    }
}
