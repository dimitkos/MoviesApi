using Microsoft.AspNetCore.Mvc;
using MoviesApi.DTOs;
using MoviesApi.Helpers;
using System.Collections.Generic;

namespace MoviesApi.Controllers.V2
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        /// <summary>
        /// get roots
        /// </summary>
        /// <returns>A list of links</returns>
        [HttpGet(Name = "getRoot")]
        [HttpHeaderIsPresent("x-version", "2")]
        public ActionResult<IEnumerable<Link>> Get()
        {
            List<Link> links = new List<Link>();

            links.Add(new Link(href: Url.Link("getRoot", new { }), rel: "self", method: "GET"));

            return links;
        }
    }
}
