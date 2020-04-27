using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<Entities.Users, Models.UsersDto>()
                .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.Surname} {src.OtherNames}")
                );

            CreateMap< Models.UsersCreateDto, Entities.Users>();

            CreateMap<Models.UsersUpdateDto, Entities.Users>();

            CreateMap<Entities.Users, Models.UsersFullDto>();
        }
    }
}
