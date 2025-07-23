using AutoMapper;
using PFM.Backend.Database.Entities;
using PFM.Backend.Database.Entities.CategoriesDTO;
using PFM.Backend.Models;

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

        }
    }
}
