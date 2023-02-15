using Application.Models.Account;
using AutoMapper;
using Domain.Entities;

namespace Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateProjection<Account, AccountModel>();

        CreateMap<CreateAccountModel, Account>();
        CreateMap<Account, AccountModel>();
        CreateMap<AccountUpdateModel, Account>();
    }
}