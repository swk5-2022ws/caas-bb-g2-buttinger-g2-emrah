using CaaS.Core.Engines.PaymentModels;

namespace CaaS.Core.Interfaces.Repository
{
    public interface IPaymentRepository
    {
        /// <summary>
        /// Retrieves the amount of the current credit information if available.
        /// All data must be passed encrypted.
        /// </summary>
        /// <param name="creditCartnumber">credit card number</param>
        /// <param name="cvv">cvv</param>
        /// <param name="expiration">expiration</param>
        /// <returns></returns>
        Task<double?> Get(string creditCartnumber, string cvv, string expiration);
        /// <summary>
        /// Updates the amount of the passed credit information.
        /// All data must be encrypted.
        /// The amount must be a lower amount than it was before.
        /// </summary>
        /// <param name="creditCartnumber">credit card number</param>
        /// <param name="cvv">cvv</param>
        /// <param name="expiration">expiration</param>
        /// <param name="amount">the amount</param>
        /// <returns></returns>
        Task<bool> Update(string creditCartnumber, string cvv, string expiration, double amount);

        /// <summary>
        /// Retrieves all payment information. 
        /// This is used just for update statement creation
        /// </summary>
        /// <returns></returns>
        IList<PaymentInformation> GetAll();
    }
}
