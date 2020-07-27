using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DefaultController : ControllerBase
    {
        [Route("/")]
        [Route("/swagger")]
        public RedirectResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
