using System;

namespace CaaS.Core.Domainmodels;

public record Cart
{
    public Cart(int id, string sessionId)
    {
        Id = id;
        SessionId = sessionId;
    }

    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public string SessionId { get; set; }
}
