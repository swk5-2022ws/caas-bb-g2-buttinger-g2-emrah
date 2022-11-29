using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Util
{
    public static class StatusInfo
    {
        public static ProblemDetails InternalError() =>
            new()
            {
                Title = "Internal server error :-(",
                Detail = $"Please contact the administrator if the problem persists."
            };
    }
}
