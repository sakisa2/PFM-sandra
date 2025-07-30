using AutoMapper;
using PFM.Backend.Database.Entities;
using PFM.Backend.Database.Entities.CategoriesDTO;
using PFM.Backend.Models;
using PFM.Backend.Models.Requests;
using PFM.Backend.Models.Responses;

namespace PFM.Backend.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, CategorizeTransactionDTO>().ReverseMap();
            CreateMap<List<CreateCategoryDTO>, CreateCategoryListDTO>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src));

            CreateMap<Transaction, TransactionDTO>().ReverseMap();
            CreateMap<TransactionSplit, TransactionSplitDTO>().ReverseMap();


            CreateMap<Transaction, TransactionDTO>()
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => Enum.Parse<Direction>(src.Direction)))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => Convert.ToDecimal(src.Amount)))
                .ForMember(dest => dest.Mcc, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Mcc) ? 0 : int.Parse(src.Mcc)));

            CreateMap<TransactionDTO, Transaction>()
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Direction.ToString()))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => Convert.ToDouble(src.Amount)))
                .ForMember(dest => dest.Mcc, opt => opt.MapFrom(src => src.Mcc.ToString()));

        }
    }
}
