using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DocumentTracking.API.Services;
using DocumentTracking.API.Models;
using AutoMapper;
using DocumentTracking.API.ResourceParameters;
using Microsoft.AspNetCore.JsonPatch;
using DocumentTracking.API.Helpers;
using System.Text.Json;
using System.Collections;
using Microsoft.Net.Http.Headers;

namespace DocumentTracking.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
   // [ResponseCache(CacheProfileName = "240SecondsCacheProfile")]        //name used in the Startup.cs ConfigureServices
    //or [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IPropertyCheckerService _propertyCheckerService;
        private readonly IMapper _mapper;

        //private readonly IMapper iMapper;

        public UsersController(IUsersRepository usersRepository, 
            IMapper mapper,
            IPropertyMappingService propertyMappingService, 
            IPropertyCheckerService propertyCheckerService)
        {
            _usersRepository = usersRepository ??
                throw new ArgumentNullException(nameof(usersRepository));
            _propertyMappingService = propertyMappingService ??
                throw new ArgumentNullException(nameof(propertyMappingService));

            _propertyCheckerService = propertyCheckerService ??
                throw new ArgumentNullException(nameof(propertyCheckerService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet(Name = "GetUsers")]
        [HttpHead]
        //public ActionResult<IEnumerable<UsersDto>> GetUsers([FromQuery] UsersResourceParameters usersResourceParameters)
        //Change to IActionResult to accomodate Data Shaping
        public IActionResult GetUsers([FromQuery] UsersResourceParameters usersResourceParameters)
        {
            if(!_propertyMappingService.ValidateMappingExistsFor<UsersDto, Entities.Users>(usersResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            //check if the fields are part of the UserDto object
            if (!_propertyCheckerService.TypeHasProperties<UsersDto>(usersResourceParameters.Fields))
            {
                return BadRequest();
            }

            var objUsers = _usersRepository.GetUsers(usersResourceParameters);
            //return new JsonResult(objUsers.Result);

            //create the previous page
            //var previousPageLink = objUsers.HasPrevious ?
            //    CreateUsersResourceUri(usersResourceParameters,
            //    ResourceUriType.PreviousPage) : null;

            ////create the next page
            //var nextPageLink = objUsers.HasNext ?
            //    CreateUsersResourceUri(usersResourceParameters,
            //    ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = objUsers.TotalCount,
                pageSize = objUsers.PageSize,
                currentPage = objUsers.CurrentPage,
                totalPages = objUsers.TotalPages
                //previousPageLink,
                //nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            //Implement support for HATEOAS
            var links = CreateLinksForUsers(usersResourceParameters, objUsers.HasNext, objUsers.HasPrevious);

            var shappedUsers = objUsers.ShapeDatas(usersResourceParameters.Fields);

            var shapedUsersWithLinks = shappedUsers.Select(user =>
            {
                var userAsDictionary = user as IDictionary<string, object>;
                var userLinks = CreateLinksForUser((Guid)userAsDictionary["Id"], null);
                userAsDictionary.Add("links", userLinks);
                return userAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedUsersWithLinks,
                links
            };

            return Ok(linkedCollectionResource);
            //return Ok(objUsers.ShapeDatas(usersResourceParameters.Fields));     //This code will take care of Status Codes
        }

        [Produces("application/json",
            "application/vnd.waec.hateoas+json",
            "application/vnd.waec.user.full+json",
            "application/vnd.waec.user.full.hateoas+json",
            "application/vnd.waec.user.friendly+json",
            "application/vnd.waec.user.friendly.hateoas+json")]

        [HttpGet()]
        [Route("{userId:guid}", Name = "GetUser")]
        public IActionResult GetUser(Guid userId, 
            string fields, [FromHeader(Name = "Accept")] string mediaType)
        {

            if(!MediaTypeHeaderValue.TryParse(mediaType, 
                out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }
            //check if the fields are part of the UserDto object
            if (!_propertyCheckerService.TypeHasProperties<UsersDto>(fields))
            {
                return BadRequest();
            }
            var objUsers = _usersRepository.GetUser(userId).Result;
            if(objUsers == null)
            {
                return NotFound();
            }

            var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<LinkDto> links = new List<LinkDto>();
            if(includeLinks)
            {
                links = CreateLinksForUser(userId, fields);
            }

            var primaryMediaType = includeLinks ?
                parsedMediaType.SubTypeWithoutSuffix
                .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            //full object dto
            if(primaryMediaType == "vnd.waec.user.full")
            {
                var fullResourceToReturn = _mapper.Map<UsersFullDto>(objUsers)
                    .ShapeData(fields) as IDictionary<string, object>;
                
                if(includeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            //friendly author
            var friendlyResourceToReturn = _mapper.Map<UsersDto>(objUsers)
                    .ShapeData(fields) as IDictionary<string, object>;

            if(includeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);
            //return new JsonResult(objUsers.Result);
            //if (parsedMediaType.MediaType == "application/vnd.waec.hateoas+json")
            //{
            //    var links = CreateLinksForUser(userId, fields);

            //    var linkedResourceToReturn = objUsers.ShapeData(fields)
            //        as IDictionary<string, object>;

            //    linkedResourceToReturn.Add("links", links);

            //    return Ok(linkedResourceToReturn);
            //} 
            //return Ok(objUsers.ShapeData(fields));     //This code will take care of Status Codes
        }

        [HttpPost(Name = "CreateUser")]
        public ActionResult<UsersDto> CreateUser(UsersCreateDto user)
        {
            var result = _usersRepository.CreateUser(user).Result;
            //Return the named user using the specified URI name

            //Create a links to support HATEOAS
            var links = CreateLinksForUser(result.Id, null);

            //Create a linked resource
            var linkedResourceToReturn = result.ShapeData(null)
                as IDictionary<string, object>;

            //Add the links to the linked resource
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetUser",
                new { userId = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }

        [HttpOptions]
        public IActionResult GetUserOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        [HttpPut("{userId}")]
        public ActionResult UpdateUser(Guid userId, UsersUpdateDto usersUpdate)
        {
            if(!_usersRepository.UserExists(userId).Result)
            {
                return NotFound();
            }

            var result = _usersRepository.UpdateUser(userId, usersUpdate).Result;

            return NoContent();
        }

        [HttpPatch("{userId}")]
        public ActionResult PatchUser(Guid userId, JsonPatchDocument<UsersUpdateDto> patchDocument)
        {
            if(!_usersRepository.UserExists(userId).Result)
            {
                return NotFound();
            }

            var result = _usersRepository.PatchUser(userId, patchDocument);

            return NoContent();
        }

        [HttpDelete("{userId}", Name = "DeleteUser")]
        public ActionResult DeleteUser(Guid userId)
        {
            if(!_usersRepository.UserExists(userId).Result)
            {
                return NotFound();
            }

            var result = _usersRepository.DeleteUser(userId);

            return NoContent();
        }

        private string CreateUsersResourceUri(
            UsersResourceParameters  usersResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetUsers",
                        new
                        {
                            fields = usersResourceParameters.Fields,
                            orderBy = usersResourceParameters.OrderBy,
                            pageNumber = usersResourceParameters.PageNumber - 1,
                            pageSize = usersResourceParameters.PageSize,
                            emailAddress = usersResourceParameters.EmailAddress,
                            searchQuery = usersResourceParameters.SearchQuery,
                        });
                case ResourceUriType.NextPage:
                case ResourceUriType.CurrentPage:
                    return Url.Link("GetUsers",
                        new
                        {
                            fields = usersResourceParameters.Fields,
                            orderBy = usersResourceParameters.OrderBy,
                            pageNumber = usersResourceParameters.PageNumber,
                            pageSize = usersResourceParameters.PageSize,
                            emailAddress = usersResourceParameters.EmailAddress,
                            searchQuery = usersResourceParameters.SearchQuery,
                        });
                default:
                    return Url.Link("GetUsers",
                        new
                        {
                            fields = usersResourceParameters.Fields,
                            orderBy = usersResourceParameters.OrderBy,
                            pageNumber = usersResourceParameters.PageNumber,
                            pageSize = usersResourceParameters.PageSize,
                            emailAddress = usersResourceParameters.EmailAddress,
                            searchQuery = usersResourceParameters.SearchQuery,
                        });
            }
        }
    
        private IEnumerable<LinkDto> CreateLinksForUser(Guid userId, string fields)
        {
            var links = new List<LinkDto>();

            if(string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkDto(Url.Link("GetUser", new { userId, fields }),
                    "self",
                    "GET"));
            } else
            {
                links.Add(
                    new LinkDto(Url.Link("GetUser", new { userId, fields }),
                    "self",
                    "GET"));
            }

            links.Add(
                    new LinkDto(Url.Link("DeleteUser", new { userId }),
                    "delete_user",
                    "DELETE"));

            links.Add(
                    new LinkDto(Url.Link("CreateUserAudit", new { userId }),
                    "create_user_audit_trail",
                    "POST"));

            //GetUserAuditTrail
            links.Add(
                    new LinkDto(Url.Link("GetUserAuditTrails", new { userId }),
                    "audit_trail",
                    "GET"));

            return links;
        }

        public IEnumerable<LinkDto> CreateLinksForUsers(
            UsersResourceParameters usersResourceParameters, 
            bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            //self
            links.Add(
                new LinkDto(CreateUsersResourceUri(
                usersResourceParameters, ResourceUriType.CurrentPage)
                , "self", "GET"));

            if(hasNext)
            {
                links.Add(
                new LinkDto(CreateUsersResourceUri(
                usersResourceParameters, ResourceUriType.NextPage)
                , "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                new LinkDto(CreateUsersResourceUri(
                usersResourceParameters, ResourceUriType.PreviousPage)
                , "previousPage", "GET"));
            }

            return links;
        }
    }
}