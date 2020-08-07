using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApi.Tests
{
    public class AllowAnonymousHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirment in context.Requirements.ToList())
            {
                context.Succeed(requirment);
            }

            return Task.CompletedTask;
        }
    }
}
