using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Instructors.Requests;
using Vitastic.Api.Features.Instructors.Responses;
using Vitastic.App.Features.Instructors.Commands.Create;
using Vitastic.App.Features.Instructors.Commands.DeActive;
using Vitastic.App.Features.Instructors.Commands.Update;
using Vitastic.App.Features.Instructors.Dtos;
using Vitastic.App.Features.Instructors.Queries.GetById;
using Vitastic.App.Features.Instructors.Queries.List;
using Vitastic.App.Features.Instructors.Queries.Search;

namespace Vitastic.Api.Features.Instructors;
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class InstructorsController(
    IMediator mediator,
    IMapper mapper,
    ILogger<InstructorsController> logger) : ControllerBase
{
    // ======================== COMMANDS ========================
#region Create Instructor

[HttpPost]
[ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
public async Task<ApiResponse<Guid>> CreateInstructor(
    [FromBody] CreateInstructorRequest request,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Creating Instructor - UserId: {UserId}", request.UserId);

    var command = mapper.Map<CreateInstructorCommand>(request);
    var result = await mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
        logger.LogWarning(
            "Create Instructor failed - {ErrorCode}: {ErrorMessage}",
            result.Error.Code, result.Error.Message);

        return result.ToApiResponse<Guid>("خطا در ایجاد مدرس");
    }

    logger.LogInformation("Instructor created - Id: {InstructorId}", result.Value);

    return result.ToApiResponse(
        t=>t,
        "مدرس با موفقیت ایجاد شد"
    );
}

#endregion


#region Deactivate Instructor

[HttpPatch("{instructorId:guid}/deactivate")]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
public async Task<ApiResponse> DeactivateInstructor(
    Guid instructorId,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Deactivating Instructor - Id: {InstructorId}", instructorId);

    var result = await mediator.Send(
        new DeActiveInstructorCommand(instructorId), cancellationToken);

    if (result.IsFailure)
    {
        logger.LogWarning(
            "Deactivate Instructor failed - Id: {InstructorId}, Error: {ErrorCode}",
            instructorId, result.Error.Code);

        return result.ToApiResponse("خطا در غیرفعال‌سازی مدرس");
    }

    logger.LogInformation("Instructor deactivated - Id: {InstructorId}", instructorId);

    return result.ToApiResponse("مدرس با موفقیت غیرفعال شد");
}

#endregion


#region Update Instructor Bio

[HttpPatch("{instructorId:guid}/bio")]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
public async Task<ApiResponse> UpdateInstructorBio(
    Guid instructorId,
    [FromBody] UpdateInstructorRequest request,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Updating Instructor Bio - Id: {InstructorId}", instructorId);

    var command = new UpdateInstructorCommand(instructorId, request.NewBio);
    var result = await mediator.Send(command, cancellationToken);

    if (result.IsFailure)
    {
        logger.LogWarning(
            "Update Instructor Bio failed - Id: {InstructorId}, Error: {ErrorCode}",
            instructorId, result.Error.Code);

        return result.ToApiResponse("خطا در به‌روزرسانی بیوگرافی مدرس");
    }

    logger.LogInformation("Instructor Bio updated - Id: {InstructorId}", instructorId);

    return result.ToApiResponse("بیوگرافی مدرس با موفقیت به‌روزرسانی شد");
}

#endregion

    // ======================== QUERIES ========================
   #region Get Instructor By Id

[AllowAnonymous]
[HttpGet("{instructorId:guid}")]
[ProducesResponseType(typeof(ApiResponse<InstructorDetailResponse>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
public async Task<ApiResponse<InstructorDetailResponse>> GetInstructor(
    Guid instructorId,
    CancellationToken cancellationToken)
{
    logger.LogInformation("Getting Instructor - Id: {InstructorId}", instructorId);

    var result = await mediator.Send(
        new GetInstructorByIdQuery(instructorId), cancellationToken);

    if (result.IsFailure)
    {
        logger.LogWarning("Instructor not found - Id: {InstructorId}", instructorId);
        return result.ToApiResponse<InstructorDetailResponse>("مدرس یافت نشد");
    }


    logger.LogInformation("Instructor retrieved - Id: {InstructorId}", instructorId);

    return result.ToApiResponse(
        mapper.Map<InstructorDetailResponse>,
        "اطلاعات مدرس با موفقیت دریافت شد"
    );
}

#endregion


#region List Instructors

[AllowAnonymous]
[HttpGet]
[ProducesResponseType(typeof(ApiResponse<PaginatedResponse<InstructorResponse>>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
public async Task<ApiResponse<PaginatedResponse<InstructorResponse>>> ListInstructors(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
{
    logger.LogInformation(
        "Listing Instructors - Page: {Page}, Size: {Size}",
        pageNumber, pageSize);

    var result = await mediator.Send(
        new ListInstructorsQuery(pageNumber, pageSize), cancellationToken);

    if (result.IsFailure)
        return result.ToApiResponse<PaginatedResponse<InstructorResponse>>("خطا در دریافت لیست مدرسین");

    var response = mapper.Map<PaginatedResponse<InstructorResponse>>(result.Value);

    logger.LogInformation(
        "Instructors listed - Count: {Count}, Page: {Page}/{Total}",
        response.Items.Count, pageNumber, result.Value.TotalPages);

    return result.ToApiResponse(
        mapper.Map<PaginatedResponse<InstructorResponse>>,
        "لیست مدرسین با موفقیت دریافت شد"
    );
}

#endregion


#region Search Instructors

[AllowAnonymous]
[HttpGet("search")]
[ProducesResponseType(typeof(ApiResponse<PaginatedResponse<InstructorResponse>>), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
public async Task<ApiResponse<PaginatedResponse<InstructorResponse>>> SearchInstructors(
    [FromQuery] string searchTerm,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] InstructorStatusResponse? status=null,
    CancellationToken cancellationToken = default)
{
    logger.LogInformation(
        "Searching Instructors - Term: {Term}, Page: {Page}",
        searchTerm, pageNumber);

    // Convert API enum to domain enum
    var statusDto = (InstructorStatusDto)(status ?? InstructorStatusResponse.Active);


    var result = await mediator.Send(
        new SearchInstructorsQuery(searchTerm, pageNumber, pageSize, statusDto),
        cancellationToken);

    if (result.IsFailure)
        return result.ToApiResponse<PaginatedResponse<InstructorResponse>>("خطا در جست‌وجوی مدرسین");

    logger.LogInformation(
        "Search completed - Term: {Term}, Results: {Count}",
        searchTerm, result.Value.Items.Count);

    return result.ToApiResponse(
        mapper.Map<PaginatedResponse<InstructorResponse>>,
        "نتایج جست‌وجوی مدرسین با موفقیت دریافت شد"
    );
}

#endregion

}
