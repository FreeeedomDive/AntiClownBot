using AntiClown.Api.Core.Transactions.Domain;
using AntiClown.Api.Core.Transactions.Repositories;
using AutoMapper;

namespace AntiClown.Api.Core.Mappings;

public class TransactionsMapperProfile : Profile
{
    public TransactionsMapperProfile()
    {
        CreateMap<TransactionStorageElement, Transaction>().ReverseMap();
    }
}