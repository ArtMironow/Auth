using Auth.Entities.DataTransferObjects;
using AutoMapper;
using DAL.Auth.Models;

namespace Auth
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserToRegisterDto, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email))
                .ForMember(u => u.Nickname, opt => opt.MapFrom(x => x.Email));
        }
    }
}
