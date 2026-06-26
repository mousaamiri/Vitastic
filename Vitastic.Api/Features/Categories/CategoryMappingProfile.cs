using AutoMapper;
using Vitastic.Api.Features.Categories.Requests;
using Vitastic.Api.Features.Categories.Responses;
using Vitastic.App.Features.Categories.Commands.Create;
using Vitastic.App.Features.Categories.Commands.Update;
using Vitastic.App.Features.Categories.Commands.UpdateList;
using Vitastic.App.Features.Categories.Commands.UpdateName;
using Vitastic.App.Features.Categories.Dtos;
using Vitastic.App.Features.Common.Dtos;

namespace Vitastic.Api.Features.Categories;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<UpsertCategoryRequest, CreateCategoryCommand>();
        CreateMap<UpsertCategoryRequest, UpdateCategoryCommand>();
        CreateMap<UpdateCategoriesListRequest,UpdateCategoriesListCommand>();
        CreateMap<UpdateCategoryNameRequest, UpdateCategoryNameCommand>();
        CreateMap<UpdateCategoriesListRequest,UpdateCategoriesListCommand>();

        CreateMap<CategoryUpdateDto, CategoryUpdateResponse>().ReverseMap();
        CreateMap<SubCategoryDto, SubCategoryResponse>();
        CreateMap<CategoryDetailDto, CategoryDetailResponse>();
        CreateMap<CategoryListDto, CategoryListResponse>();
        CreateMap<PaginatedResult<CategoryListDto>,PaginatedResult<CategoryListResponse>>();


    }
}
