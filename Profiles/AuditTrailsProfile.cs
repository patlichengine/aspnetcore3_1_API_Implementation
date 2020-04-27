using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentTracking.API.Profiles
{
    public class AuditTrailsProfile : Profile
    {
        public AuditTrailsProfile()
        {
            CreateMap<Entities.AuditTrail, Models.AuditTrailsDto>()
                .ForMember(
                    dest => dest.Operation,
                    opt => opt.MapFrom(src => $"{src.Action}"))
                .ForMember(
                    dest => dest.Message,
                    opt => opt.MapFrom(src => $"{src.Description}"));

            CreateMap<Models.AuditTrailsCreateDto, Entities.AuditTrail>();
            CreateMap<Models.AuditTrailsUpdateDto, Entities.AuditTrail>();
        }
    }
}
