using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Tags.Requests;
using Vitastic.Api.Features.Tags.Responses;
using Vitastic.App.Features.Tags.Commands.Create;
using Vitastic.App.Features.Tags.Commands.Deactivate;
using Vitastic.App.Features.Tags.Commands.Update;
using Vitastic.App.Features.Tags.Queries.GetById;
using Vitastic.App.Features.Tags.Queries.List;
using Vitastic.App.Features.Tags.Queries.Search;

namespace Vitastic.Api.Features.Tags;
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class TagsController(
    IMediator mediator,
    IMapper mapper,
    ILogger<TagsController> logger) : ControllerBase
{
// ======================== COMMANDS ========================

#region ==================== CREATE TAG ====================

[HttpPost]
[ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
public async Task<ApiResponse<Guid>> CreateTag(
    [FromBody] CreateTagRequest request,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Creating tag - Name: {Name}", request.Name);

    var command = mapper.Map<CreateTagCommand>(request);
    var result = await mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
        logger.LogWarning(
            "Create tag failed - {ErrorCode}: {ErrorMessage}",
            result.Error.Code, result.Error.Message);

        return result.ToApiResponse<Guid>("ایجاد تگ انجام نشد");
    }

    logger.LogInformation("Tag created - Id: {TagId}", result.Value);

    return result
        .ToApiResponse(t=>t,"تگ با موفقیت ایجاد شد.");
}

#endregion

#region ==================== UPDATE TAG NAME ====================

[HttpPatch("{tagId:guid}/name")]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
public async Task<ApiResponse> UpdateTagName(
    Guid tagId,
    [FromBody] UpdateTagNameRequest request,
    CancellationToken cancellationToken)
{
    logger.LogInformation(
        "Updating tag name - Id: {TagId}, NewName: {Name}",
        tagId, request.NewName);

    var command = new UpdateTagCommand(tagId, request.NewName);
    var result = await mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
        logger.LogWarning(
            "Update tag name failed - Id: {TagId}, Error: {ErrorCode}",
            tagId, result.Error.Code);

        return result.ToApiResponse("به‌روزرسانی نام تگ انجام نشد");
    }

    logger.LogInformation("Tag name updated - Id: {TagId}", tagId);

    return result.ToApiResponse("نام تگ با موفقیت به‌روزرسانی شد.");
}

#endregion

#region ==================== DEACTIVATE TAG ====================

[HttpPatch("{tagId:guid}/deactivate")]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
public async Task<ApiResponse> DeactivateTag(
    Guid tagId,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Deactivating tag - Id: {TagId}", tagId);

    var command = new DeactivateTagCommand(tagId);
    var result = await mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
        logger.LogWarning(
            "Deactivate tag failed - Id: {TagId}, Error: {ErrorCode}",
            tagId, result.Error.Code);

        return result.ToApiResponse("غیرفعال‌سازی تگ انجام نشد");
    }

    logger.LogInformation("Tag deactivated - Id: {TagId}", tagId);

    return result.ToApiResponse("تگ با موفقیت غیرفعال شد.");
}

#endregion

 // ======================== QUERIES ========================

#region ==================== GET TAG ====================

[AllowAnonymous]
[HttpGet("{tagId:guid}")]
[ProducesResponseType(typeof(ApiResponse<TagResponse>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
public async Task<ApiResponse<TagResponse>> GetTag(
    Guid tagId,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Getting tag - Id: {TagId}", tagId);

    var result = await mediator.Send(
        new GetTagByIdQuery(tagId), cancellationToken);

    if (result.IsFailure)
    {
        logger.LogWarning("Tag not found - Id: {TagId}", tagId);
        return result.ToApiResponse<TagResponse>( "تگی با این شناسه یافت نشد");
    }

    logger.LogInformation("Tag retrieved - Id: {TagId}", tagId);

    return result.ToApiResponse(
        mapper.Map<TagResponse>,
        "تگ با موفقیت بازیابی شد.");
}

#endregion

#region ==================== LIST TAGS ====================

[AllowAnonymous]
[HttpGet]
[ProducesResponseType(typeof(ApiResponse<PaginatedResponse<TagResponse>>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
public async Task<ApiResponse<PaginatedResponse<TagResponse>>> ListTags(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
{
    logger.LogInformation(
        "Listing tags - Page: {Page}, Size: {Size}",
        pageNumber, pageSize);

    var result = await mediator.Send(
        new ListTagsQuery(pageNumber, pageSize),
        cancellationToken);

    if (result.IsFailure)
        return result.ToApiResponse<PaginatedResponse<TagResponse>>("دریافت لیست تگ‌ها انجام نشد");

    var dto = result.Value;

    logger.LogInformation(
        "Tags listed - Count: {Count}, Page: {Page}/{Total}",
        dto.Items.Count, pageNumber, dto.TotalPages);

    return result.ToApiResponse(
        mapper.Map<PaginatedResponse<TagResponse>>,
        "لیست تگ‌ها با موفقیت بازیابی شد.");
}

#endregion

#region ==================== SEARCH TAGS ====================

[AllowAnonymous]
[HttpGet("search")]
[ProducesResponseType(typeof(ApiResponse<PaginatedResponse<TagResponse>>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
public async Task<ApiResponse<PaginatedResponse<TagResponse>>> SearchTags(
    [FromQuery] string? searchTerm,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
{
    logger.LogInformation(
        "Searching tags - Term: {Term}, Page: {Page}",
        searchTerm, pageNumber);

    var result = await mediator.Send(
        new SearchTagsQuery(searchTerm, pageNumber, pageSize),
        cancellationToken);

    if (result.IsFailure)
        return result.ToApiResponse<PaginatedResponse<TagResponse>>( "جستجوی تگ‌ها انجام نشد");

    var dto = result.Value;

    logger.LogInformation(
        "Search completed - Term: {Term}, Results: {Count}",
        searchTerm, dto.Items.Count);

    return result.ToApiResponse(
        mapper.Map<PaginatedResponse<TagResponse>>,
        "جستجو با موفقیت انجام شد.");
}

#endregion


}
