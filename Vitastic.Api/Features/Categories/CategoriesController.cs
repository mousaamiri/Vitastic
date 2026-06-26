using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Categories.Requests;
using Vitastic.Api.Features.Categories.Responses;
using Vitastic.App.Features.Categories.Commands.Create;
using Vitastic.App.Features.Categories.Commands.Deactivate;
using Vitastic.App.Features.Categories.Commands.Update;
using Vitastic.App.Features.Categories.Commands.UpdateDescription;
using Vitastic.App.Features.Categories.Commands.UpdateDisplayOrder;
using Vitastic.App.Features.Categories.Commands.UpdateList;
using Vitastic.App.Features.Categories.Commands.UpdateName;
using Vitastic.App.Features.Categories.Commands.UpdateSlug;
using Vitastic.App.Features.Categories.Dtos;
using Vitastic.App.Features.Categories.Queries.GetById;
using Vitastic.App.Features.Categories.Queries.GetCategoriesTree;
using Vitastic.App.Features.Categories.Queries.List;
using Vitastic.App.Features.Categories.Queries.Search;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Features.Categories;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CategoriesController(
    IMediator mediator,
    IMapper mapper,
    ILogger<CategoriesController> logger) : ControllerBase
{
    // ======================== COMMANDS ========================

    #region Create Category

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<Guid>> CreateCategory(
        [FromBody] UpsertCategoryRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating category - Name: {Name}, Slug: {Slug}", request.Name, request.Slug);

        CreateCategoryCommand command = mapper.Map<CreateCategoryCommand>(request);
        Result<Guid> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Create category failed - {Error}", result.Error);
            return result.ToApiResponse<Guid>("خطا در ایجاد دسته‌بندی");
        }

        logger.LogInformation("Category {CategoryId} created", result.Value);

        return result.ToApiResponse(
            id => id,
            "دسته‌بندی با موفقیت ایجاد شد");
    }

    #endregion

    #region Update Category

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> UpdateCategory(
        Guid id,
        [FromBody] UpsertCategoryRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating category - Id: {CategoryId}", id);

        UpdateCategoryCommand command = new(
            id,
            request.Name,
            request.Slug,
            request.DisplayOrder,
            request.ParentCategoryId,
            request.Description);

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Update category failed - Id: {CategoryId}, Error: {Error}", id, result.Error);
            return result.ToApiResponse("خطا در بروزرسانی دسته‌بندی");
        }

        logger.LogInformation("Category {CategoryId} updated", id);

        return result.ToApiResponse("دسته‌بندی با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Category List
    [HttpPut("update-range")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> UpdateCategoriesList(
        [FromBody] UpdateCategoriesListRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating categories list - Count: {Count}", request.Categories.Count);

        var command =new UpdateCategoriesListCommand
            (mapper.Map<List<CategoryUpdateDto>>(request.Categories));

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Update categories list failed - Error: {Error}", result.Error);
            return result.ToApiResponse("خطا در بروزرسانی لیست دسته‌بندی‌ها");
        }

        logger.LogInformation("Categories list updated successfully - Count: {Count}", request.Categories.Count);

        return result.ToApiResponse("لیست دسته‌بندی‌ها با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Category Name

    [HttpPatch("{id:guid}/name")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> UpdateCategoryName(
        Guid id,
        [FromBody] UpdateCategoryNameRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating category name - Id: {CategoryId}, NewName: {NewName}", id, request.Name);

        UpdateCategoryNameCommand command = new(id, request.Name);

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Update category name failed - Id: {CategoryId}, Error: {Error}", id, result.Error);
            return result.ToApiResponse("خطا در بروزرسانی نام دسته‌بندی");
        }

        logger.LogInformation("Category name updated - Id: {CategoryId}", id);

        return result.ToApiResponse("نام دسته‌بندی با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Category Slug

    [HttpPatch("{id:guid}/slug")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> UpdateCategorySlug(
        Guid id,
        [FromBody] UpdateCategorySlugRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating category slug - Id: {CategoryId}, NewSlug: {NewSlug}", id, request.Slug);

        UpdateCategorySlugCommand command = new(id, request.Slug);

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Update category slug failed - Id: {CategoryId}, Error: {Error}", id, result.Error);
            return result.ToApiResponse("خطا در بروزرسانی اسلاگ دسته‌بندی");
        }

        logger.LogInformation("Category slug updated - Id: {CategoryId}", id);

        return result.ToApiResponse("اسلاگ دسته‌بندی با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Category Display Order

    [HttpPatch("{id:guid}/display-order")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateCategoryDisplayOrder(
        Guid id,
        [FromBody] UpdateCategoryDisplayOrderRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating category display order - Id: {CategoryId}, NewOrder: {NewOrder}", id,
            request.DisplayOrder);

        UpdateCategoryDisplayOrderCommand command = new(id, request.DisplayOrder);
        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Update category display order failed - Id: {CategoryId}, Error: {Error}", id,
                result.Error);
            return result.ToApiResponse("خطا در بروزرسانی ترتیب نمایش دسته‌بندی");
        }

        logger.LogInformation("Category display order updated - Id: {CategoryId}", id);

        return result.ToApiResponse("ترتیب نمایش دسته‌بندی با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Category Description

    [HttpPatch("{id:guid}/description")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateCategoryDescription(
        Guid id,
        [FromBody] UpdateCategoryDescriptionRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating category description - Id: {CategoryId}", id);

        UpdateCategoryDescriptionCommand command = new(id, request.Description);
        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Update category description failed - Id: {CategoryId}, Error: {Error}", id,
                result.Error);
            return result.ToApiResponse("خطا در بروزرسانی توضیحات دسته‌بندی");
        }

        logger.LogInformation("Category description updated - Id: {CategoryId}", id);

        return result.ToApiResponse("توضیحات دسته‌بندی با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Deactivate Category

    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> DeactivateCategory(
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deactivating category - Id: {CategoryId}", id);

        DeactivateCategoryCommand command = new(id);
        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Deactivate category failed - Id: {CategoryId}, Error: {Error}", id, result.Error);
            return result.ToApiResponse("خطا در غیرفعال‌سازی دسته‌بندی");
        }

        logger.LogInformation("Category deactivated - Id: {CategoryId}", id);

        return result.ToApiResponse("دسته‌بندی با موفقیت غیرفعال شد");
    }

    #endregion

    #region Get Category By Id

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<CategoryDetailResponse>> GetCategory(
        Guid id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting category - Id: {CategoryId}", id);

        Result<CategoryDetailDto> result = await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Category not found - Id: {CategoryId}", id);
            return result.ToApiResponse<CategoryDetailResponse>
                ("دسته‌بندی مورد نظر یافت نشد");
        }

        logger.LogInformation("Category retrieved - Id: {CategoryId}", id);

        return result.ToApiResponse(
            mapper.Map<CategoryDetailResponse>,
            "دسته‌بندی با موفقیت دریافت شد");
    }

    #endregion

    #region List Categories

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CategoryListResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<CategoryListResponse>>> ListCategories(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool onlyParents = false,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Listing categories - OnlyParents: {OnlyParents}", onlyParents);

        Result<PaginatedResult<CategoryListDto>> result =
            await mediator.Send(new ListCategoriesQuery(pageNumber, pageSize, onlyParents), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to list categories: {Error}", result.Error);
            return result.ToApiResponse<PaginatedResponse<CategoryListResponse>>("خطا در دریافت لیست دسته‌بندی‌ها");
        }

        var mappedData = mapper.Map<PaginatedResponse<CategoryListResponse>>(result.Value);

        logger.LogInformation("Categories listed successfully - Count: {Count}", result.Value.TotalCount);

        return ApiResponse<PaginatedResponse<CategoryListResponse>>.Success(
            mappedData,
            "لیست دسته‌بندی‌ها با موفقیت دریافت شد");
    }


    #endregion

    #region Tree List Categories

    [HttpGet("tree")]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryDetailResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<List<CategoryDetailResponse>>> TreeListCategories(
        [FromQuery] bool onlyParents = false,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Listing categories - OnlyParents: {OnlyParents}", onlyParents);

        Result<List<CategoryDetailDto>> result =
            await mediator.Send(new GetCategoriesTreeQuery(onlyParents), cancellationToken);

        if (result.IsFailure)
        {
            return result.ToApiResponse<List<CategoryDetailResponse>>("خطا در دریافت لیست دسته‌بندی‌ها");
        }

        logger.LogInformation("Categories listed successfully - Count: {Count}", result.Value.Count);

        return result.ToApiResponse(
            mapper.Map<List<CategoryDetailResponse>>,
            "لیست دسته‌بندی‌ها با موفقیت دریافت شد");
    }


    #endregion

    #region Search Categories

    [AllowAnonymous]
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CategoryListResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<CategoryListResponse>>> SearchCategories(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool onlyParents = false,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Searching categories - Term: {Term}, Page: {Page}", searchTerm, pageNumber);

        Result<PaginatedResult<CategoryListDto>> result =
            await mediator.Send(new SearchCategoriesQuery(searchTerm, pageNumber, pageSize, onlyParents),
                cancellationToken);

        if (result.IsFailure)
        {
            return result.ToApiResponse<PaginatedResponse<CategoryListResponse>>("خطا در جستجوی دسته‌بندی‌ها");
        }

        logger.LogInformation("Search completed - Term: {Term}", searchTerm);

        return result.ToApiResponse(
            mapper.Map<PaginatedResponse<CategoryListResponse>>,
            "جستجوی دسته‌بندی‌ها با موفقیت انجام شد");
    }

    #endregion
}
