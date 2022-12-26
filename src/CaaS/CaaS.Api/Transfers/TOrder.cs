using Caas.Core.Common.Attributes;
using CaaS.Core.Domainmodels;

namespace CaaS.Api.Transfers
{
    public class TOrder
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public double Discount { get; set; }
        public DateTime OrderDate { get; set; }
        public TCart? Cart { get; set; }
    }
}
