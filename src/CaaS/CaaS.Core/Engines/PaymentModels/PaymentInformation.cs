
namespace CaaS.Core.Engines.PaymentModels
{
    public class PaymentInformation
    {
        public string CreditCardNumber { get; set; } = null!;
        public string CVV { get; set; } = null!;
        public string Expiration { get; set; } = null!;
        public double AvailableAmount { get; set; }
        public bool IsInUse { get; set; }
    }
}
