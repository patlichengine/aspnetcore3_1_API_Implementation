using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentTracking.API.Entities;
using DocumentTracking.API.Helpers;
using DocumentTracking.API.Models;
using DocumentTracking.API.ResourceParameters;
using Microsoft.AspNetCore.JsonPatch;

namespace DocumentTracking.API.Services
{
    public interface IUsersRepository
    {
        Task<IEnumerable<UsersDto>> GetUsers();

        Task<IEnumerable<UsersDto>> GetUsers(IEnumerable<Guid> userIds);

        PagedList<Users> GetUsers(UsersResourceParameters usersResourceParameters);

        Task<Users> GetUser(Guid userId);

        Task<bool> UserExists(Guid userId);
        Task<UsersDto> CreateUser(UsersCreateDto user);

        Task<IEnumerable<UsersDto>> CreateUsers(IEnumerable<UsersCreateDto> userCollection);

        Task<bool> Save();
        Task<UsersDto> UpdateUser(Guid userId, UsersUpdateDto usersUpdateDto);
        Task<UsersDto> PatchUser(Guid userId, JsonPatchDocument<UsersUpdateDto> patchDocument);
        Task<UsersDto> DeleteUser(Guid userId);
    }
}
