using AutoMapper;
using Vitastic.App.Features.Categories.Dtos;
using Vitastic.Domain.Entities.Categories;

namespace Vitastic.App.Features.Common.Mapping;

public class CategoryMappingProfile:Profile
{
    public CategoryMappingProfile()
    {
        // Category → CategoryDto
        CreateMap<Category, CategoryDto>();

        // Category → CategoryDto
        CreateMap<Category, SubCategoryDto>();

        // Category → CategoryListDto
        CreateMap<Category, CategoryListDto>();

        // Category → CategoryDetailDto
        CreateMap<Category, CategoryDetailDto>()
            .ForMember(dest => dest.ParentCategoryName,
                opt => opt.Ignore()) //Set in handler
            .ForMember(dest => dest.SubCategories,
                opt => opt.Ignore()); // Set in handler
    }
}
