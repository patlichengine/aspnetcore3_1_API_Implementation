using DocumentTracking.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentTracking.API.Classes;
using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using DocumentTracking.API.DBContext;
using DocumentTracking.API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DocumentTracking.API.ResourceParameters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DocumentTracking.API.Helpers;

namespace DocumentTracking.API.Services
{
    public class cUsersRepository : ControllerBase, IUsersRepository, IDisposable
    {
        private readonly WDocumentTrackingContext _context;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public cUsersRepository(WDocumentTrackingContext context, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        //private readonly ConnectionString _connectionString;
        //public UsersRepository(ConnectionString connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        public async Task<UsersDto> CreateUser(UsersCreateDto user)
        {
            return await Task.Run(async () =>
            {
                if(user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }
                var userEntity = _mapper.Map<Entities.Users>(user);
                userEntity.Id = Guid.NewGuid();

                _context.Users.Add(userEntity);

                //call the save method
                bool saveResult = await Save();

                return _mapper.Map<UsersDto>(userEntity);
            });
        }

        public async Task<IEnumerable<UsersDto>> CreateUsers(IEnumerable<UsersCreateDto> userCollection)
        {
            return await Task.Run(async () =>
            {
                if (userCollection == null)
                {
                    throw new ArgumentNullException(nameof(userCollection));
                }
                var userEntities = _mapper.Map<IEnumerable<Entities.Users>>(userCollection);

                foreach (var userEntity in userEntities)
                {
                    userEntity.Id = Guid.NewGuid();
                    _context.Users.Add(userEntity);
                }

                //call the save method
                bool saveResult = await Save();

                //get the list of ids as string from the data
                return _mapper.Map<IEnumerable<UsersDto>>(userEntities);
            });
        }

        public async Task<UsersDto> DeleteUser(Guid userId)
        {
            return await Task.Run(async () =>
            {
                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                _context.Users.Remove(user);
                await Save();

                return _mapper.Map<UsersDto>(user);
            });
        }
        public async Task<IEnumerable<UsersDto>> GetUsers()
        {
            return await Task.Run(async () =>
            {
                var result = await _context.Users.ToListAsync<Users>();

                return _mapper.Map<IEnumerable<UsersDto>>(result);
            });
            //List<Users> objResutlt = new List<Users>();
            
        }

        public async Task<IEnumerable<UsersDto>> GetUsers(IEnumerable<Guid> userIds)
        {
            return await Task.Run(async () =>
            {
                if (userIds == null)
                {
                    throw new ArgumentNullException(nameof(userIds));
                }

                var result = await _context.Users.Where(a => userIds.Contains(a.Id))
                    .OrderBy(a => a.Surname)
                    .OrderBy(a => a.OtherNames)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<UsersDto>>(result);
            });
            
        }
        public PagedList<Users> GetUsers(UsersResourceParameters usersResourceParameters)
        {
            //return await Task.Run(async () =>
            //{
                if (usersResourceParameters == null)
                {
                    throw new ArgumentNullException(nameof(usersResourceParameters));
                }

                //Remove this code because paging has been applied
                //if(string.IsNullOrWhiteSpace(usersResourceParameters.EmailAddress) && string.IsNullOrWhiteSpace(usersResourceParameters.SearchQuery))
                //{
                //    return await GetUsers();
                //}

                //cast the collection into an IQueriable object
                var collection = _context.Users as IQueryable<Users>;

                if (!string.IsNullOrWhiteSpace(usersResourceParameters.EmailAddress))
                {
                    var emailAddress = usersResourceParameters.EmailAddress.Trim();
                    collection = collection.Where(c => c.Email == (emailAddress));
                }

                if (!string.IsNullOrWhiteSpace(usersResourceParameters.SearchQuery))
                {
                    var searchQuery = usersResourceParameters.SearchQuery.Trim();
                    collection = collection.Where(c => c.Email.Contains(searchQuery)
                    || c.Surname.Contains(searchQuery)
                    || c.OtherNames.Contains(searchQuery));
                }

            if (!string.IsNullOrWhiteSpace(usersResourceParameters.OrderBy))
            {
                //var searchQuery = usersResourceParameters.OrderBy.Trim();
                //if(usersResourceParameters.OrderBy.ToLowerInvariant() == "name")
                //{
                //    collection = collection.OrderBy(c => c.Surname).ThenBy(c => c.OtherNames);
                //}

                //get property mapping
                var usersPropertyMappingDictionary =
                    _propertyMappingService.GetPropertyMapping<Models.UsersDto, Users>();

                collection = collection.ApplySort(usersResourceParameters.OrderBy,
                    usersPropertyMappingDictionary);
            }

            //var result = await _context.Users.Where(c=>c.Email.Contains(emailAddress)).ToListAsync<Users>();

            //var resultList = _mapper.Map<List<UsersDto>>(collection.ToList()) as IQueryable<UsersDto>;
            //paginate the IQueryable object and continue
            //return PagedList<UsersDto>.Create(_mapper.Map<IQueryable<UsersDto>>(collection),
            //    usersResourceParameters.PageNumber,
            //    usersResourceParameters.PageSize);
            //return _mapper.Map<PagedList<UsersDto>>(
            //    await collection.Skip(usersResourceParameters.PageSize * (usersResourceParameters.PageNumber-1))
            //    .Take(usersResourceParameters.PageSize)
            //    .ToListAsync());

            //var dataToConvert = _mapper.Map<IEnumerable<UsersDto>>(collection).AsQueryable();

            //return PagedList<UsersDto>.Create(dataToConvert, usersResourceParameters.PageNumber, usersResourceParameters.PageSize);
            return PagedList<Users>.Create(collection, usersResourceParameters.PageNumber, usersResourceParameters.PageSize);
            //});
            //List<Users> objResutlt = new List<Users>();

        }



        public async Task<Users> GetUser(Guid userId)
        {
            return await Task.Run(async () =>
            {
                if (userId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(userId));
                }
                var result = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
                if (result == null)
                {
                    throw new ArgumentNullException(nameof(result));
                }

                return result;   // dataList.FirstOrDefault();
            });
            
            
        }

        private Users GetUserById(Guid userId)
        {

            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            var result = _context.Users.FirstOrDefault(c => c.Id == userId);
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return result;   // dataList.FirstOrDefault();
        }

        public async Task<UsersDto> PatchUser(Guid userId, JsonPatchDocument<UsersUpdateDto> patchDocument)
        {
            return await Task.Run(async () =>
            {
                if (patchDocument == null)
                {
                    throw new ArgumentNullException(nameof(patchDocument));
                }

                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                //map the extracted result with the Update class
                var userToPatch = _mapper.Map<UsersUpdateDto>(user);

                //apply the patch where there are changes and resolve error
                patchDocument.ApplyTo(userToPatch, ModelState);

                if(!TryValidateModel(userToPatch))
                {
                    throw new ArgumentNullException(nameof(patchDocument));
                    //return ValidationProblem(ModelState);
                }

                //map back the patched record to the previously extracted record from DB
                _mapper.Map(userToPatch, user);

                bool save = await Save();

                return _mapper.Map<UsersDto>(user);
            });
        }


        public async Task<UsersDto> UpdateUser(Guid userId, UsersUpdateDto usersUpdateDto)
        {
            return await Task.Run(async () =>
            {
                if (usersUpdateDto == null)
                {
                    throw new ArgumentNullException(nameof(usersUpdateDto));
                }

                var user = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                _mapper.Map(user, user);

                bool save = await Save();

                return _mapper.Map<UsersDto>(user);
            });
        }

        public async Task<bool> UserExists(Guid userId)
        {
            return await Task.Run(async () =>
            {
                if (userId == Guid.Empty)
                {
                    throw new ArgumentNullException(nameof(userId));
                }

                return await _context.Users.AnyAsync(a => a.Id == userId);
            });
            
        }

        public async Task<bool> Save()
        {
            return await Task.Run(async () =>
            {
                return (await _context.SaveChangesAsync() >= 0);
            });
            
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }

        public override ActionResult ValidationProblem(
            [ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();

            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
            
        }
    }
}
