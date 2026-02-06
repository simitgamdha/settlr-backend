using AutoMapper;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Models.Entities;

namespace Settlr.Web.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AppUser, UserDto>();

        CreateMap<GroupMember, GroupMemberDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));

        CreateMap<Group, GroupDto>();
        CreateMap<ExpenseSplit, ExpenseSplitDto>();
        CreateMap<Expense, ExpenseDto>();
    }
}
