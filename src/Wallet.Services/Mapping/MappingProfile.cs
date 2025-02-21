using AutoMapper;
using Wallet.Entities.EntityObjects;
using Wallet.Services.DTOs.Person;
using Wallet.Services.DTOs.AuditLog;

namespace Wallet.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Person, PersonDto>().ReverseMap();
        CreateMap<PersonAddress, PersonAddressDto>().ReverseMap();
        CreateMap<PersonContact, PersonContactDto>().ReverseMap();
        CreateMap<AuditLog, AuditLogDto>().ReverseMap();
        CreateMap<CreateAuditLogDto, AuditLog>();
    }
} 