using DocumentTracking.API.Helpers;
using DocumentTracking.API.Models;
using DocumentTracking.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCollectionsController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;

        public UserCollectionsController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }

        [HttpGet("({ids})", Name = "GetUsersCollection")]
        public IActionResult GetUsersCollection([FromRoute] [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                return BadRequest();
            }

            var result = _usersRepository.GetUsers(ids).Result;
            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public ActionResult<IEnumerable<UsersDto>> CreateUserCollection(IEnumerable<UsersCreateDto> usersCollection)
        {
            var result = _usersRepository.CreateUsers(usersCollection).Result;

            var resultIdsString = string.Join(", ", result.Select(c => c.Id));

            //return the created list of records
            return CreatedAtRoute("GetUsersCollection", new { ids = resultIdsString }, result);


        }
    }
}