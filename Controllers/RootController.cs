using DocumentTracking.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(
                    new LinkDto(Url.Link("GetRoot", new { }),
                    "self",
                    "GET"));
           links.Add(
                    new LinkDto(Url.Link("GetUsers", new { }),
                    "users",
                    "GET"));
            

            links.Add(
                    new LinkDto(Url.Link("CreateUser", new { }),
                    "create_user",
                    "POST"));

            return Ok(links);
        }
    }
}
