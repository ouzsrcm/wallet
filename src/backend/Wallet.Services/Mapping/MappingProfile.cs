using AutoMapper;
using Wallet.Entities.EntityObjects;
using Wallet.Services.DTOs.Person;
using Wallet.Services.DTOs.AuditLog;
using Wallet.Services.DTOs.Finance;


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
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Transaction, TransactionDto>().ReverseMap();
        CreateMap<Receipt, ReceiptDto>().ReverseMap();
        CreateMap<ReceiptItem, ReceiptItemDto>().ReverseMap();
        
        // Currency mappings
        CreateMap<Currency, CurrencyDto>().ReverseMap();
        CreateMap<CreateCurrencyDto, Currency>();
        CreateMap<UpdateCurrencyDto, Currency>()
            .ForAllMembers(opts => opts
                .Condition((src, dest, srcMember) => srcMember != null));
    }
} 