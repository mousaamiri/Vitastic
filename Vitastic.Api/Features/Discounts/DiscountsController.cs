using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Discounts.Requests;
using Vitastic.Api.Features.Discounts.Responses;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Discounts.Commands.Create.Fixed;
using Vitastic.App.Features.Discounts.Commands.Create.Percentage;
using Vitastic.App.Features.Discounts.Commands.Deactivate;
using Vitastic.App.Features.Discounts.Commands.Extend;
using Vitastic.App.Features.Discounts.Commands.SetMaximumAmount;
using Vitastic.App.Features.Discounts.Commands.SetMinimumAmount;
using Vitastic.App.Features.Discounts.Commands.UpdateDiscount;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.App.Features.Discounts.Queries.Calculate;
using Vitastic.App.Features.Discounts.Queries.CheckValidity;
using Vitastic.App.Features.Discounts.Queries.GetByCode;
using Vitastic.App.Features.Discounts.Queries.GetById;
using Vitastic.App.Features.Discounts.Queries.List;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Features.Discounts;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class DiscountsController(
    IMediator mediator,
    IMapper mapper,
    ILogger<DiscountsController> logger) : ControllerBase
{
    // ======================== COMMANDS ========================

    #region Create Fixed Discount

    [HttpPost("fixed")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse<Guid>> CreateFixedDiscount(
        [FromBody] UpsertDiscountRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating fixed discount - Code: {Code}", request.Code);

        var command = mapper.Map<CreateFixedAmountDiscountCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Create fixed discount failed - {ErrorCode}: {ErrorMessage}", result.Error.Code,
                result.Error.Message);
            return result.ToApiResponse<Guid>("خطا در ایجاد کد تخفیف ثابت");
        }

        logger.LogInformation("Fixed discount created - Id: {DiscountId}", result.Value);
        return result.ToApiResponse(t=>t,"کد تخفیف ثابت با موفقیت ایجاد شد");
    }

    #endregion


    #region Create Percentage Discount

    [HttpPost("percentage")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse<Guid>> CreatePercentageDiscount(
        [FromBody] UpsertDiscountRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating percentage discount - Code: {Code}", request.Code);

        var command = mapper.Map<CreatePercentageDiscountCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Create percentage discount failed - {ErrorCode}: {ErrorMessage}", result.Error.Code,
                result.Error.Message);
            return result.ToApiResponse<Guid>("خطا در ایجاد کد تخفیف درصدی");
        }

        logger.LogInformation("Percentage discount created - Id: {DiscountId}", result.Value);
        return result.ToApiResponse(t=>t,"کد تخفیف درصدی با موفقیت ایجاد شد");
    }

    #endregion

    #region Update Discount

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ApiResponse> UpdateDiscount(
        [FromBody] UpsertDiscountRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating discount - Code: {Code}", request.Code);

        var command = mapper.Map<UpdateDiscountCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Update discount failed - {ErrorCode}: {ErrorMessage}", result.Error.Code,
                result.Error.Message);
            return result.ToApiResponse("خطا در به‌روزرسانی کد تخفیف");
        }

        logger.LogInformation("Discount updated - Id: {DiscountId}", result);
        return result.ToApiResponse("کد تخفیف با موفقیت به‌روزرسانی شد");
    }

    #endregion



    #region Deactivate Discount

    [HttpPatch("{discountId:guid}/deactivate")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> DeactivateDiscount(
        Guid discountId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deactivating discount - Id: {DiscountId}", discountId);

        var result = await mediator.Send(new DeactivateDiscountCommand(discountId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Deactivate discount failed - Id: {DiscountId}, Error: {ErrorCode}", discountId,
                result.Error.Code);
            return result.ToApiResponse("خطا در غیرفعال‌سازی کد تخفیف");
        }

        logger.LogInformation("Discount deactivated - Id: {DiscountId}", discountId);
        return result.ToApiResponse("کد تخفیف با موفقیت غیرفعال شد");
    }

    #endregion

    #region Extend Discount

    [HttpPatch("{discountId:guid}/extend")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> ExtendDiscount(
        Guid discountId,
        [FromBody] ExtendDiscountRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Extending discount - Id: {DiscountId}, NewEndDate: {EndDate}", discountId,
            request.EndDate);

        var result = await mediator.Send(new ExtendDiscountEndDateCommand(discountId, request.EndDate),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Extend discount failed - Id: {DiscountId}, Error: {ErrorCode}", discountId,
                result.Error.Code);
            return result.ToApiResponse("خطا در تمدید تاریخ اعتبار کد تخفیف");
        }

        logger.LogInformation("Discount extended - Id: {DiscountId}", discountId);
        return result.ToApiResponse("تاریخ اعتبار کد تخفیف با موفقیت تمدید شد");
    }

    #endregion

    #region Set Maximum Amount Discount

    [HttpPatch("{discountId:guid}/maximum-amount")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> SetMaxAmountDiscount(
        Guid discountId,
        [FromBody] SetMaximumAmountRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting max amount for discount - Id: {DiscountId}, MaxAmount: {MaxAmount}", discountId,
            request.MaxAmount);

        var result = await mediator.Send(
            new SetMaximumDiscountAmountCommand(discountId, request.MaxAmount), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Set max amount failed - Id: {DiscountId}, Error: {ErrorCode}", discountId,
                result.Error.Code);
            return result.ToApiResponse("خطا در تنظیم حداکثر مبلغ تخفیف");
        }

        logger.LogInformation("Max amount set for discount - Id: {DiscountId}", discountId);
        return result.ToApiResponse("حداکثر مبلغ تخفیف با موفقیت تنظیم شد");
    }

    #endregion

    #region Set Minimum Amount Discount

    [HttpPatch("{discountId:guid}/minimum-amount")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> SetMinAmountDiscount(
        Guid discountId,
        [FromBody] SetMinimumAmountRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting min amount for discount - Id: {DiscountId}, MinAmount: {MinAmount}", discountId,
            request.MinAmount);

        var result = await mediator.Send(
            new SetMinimumOrderAmountCommand(discountId, request.MinAmount), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Set min amount failed - Id: {DiscountId}, Error: {ErrorCode}", discountId,
                result.Error.Code);
            return result.ToApiResponse("خطا در تنظیم حداقل مبلغ سفارش");
        }

        logger.LogInformation("Min amount set for discount - Id: {DiscountId}", discountId);
        return result.ToApiResponse("حداقل مبلغ سفارش با موفقیت تنظیم شد");
    }

    #endregion

    // ======================== QUERIES ========================

    #region Get Discount By Id

    [HttpGet("{discountId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DiscountDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<DiscountDetailResponse>> GetDiscount(
        Guid discountId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting discount - Id: {DiscountId}", discountId);

        var result = await mediator.Send(new GetDiscountQuery(discountId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Discount not found - Id: {DiscountId}", discountId);
            return result.ToApiResponse<DiscountDetailResponse>("کد تخفیف یافت نشد");
        }


        logger.LogInformation("Discount retrieved - Id: {DiscountId}", discountId);

        return result.ToApiResponse(
            mapper.Map<DiscountDetailResponse>,
            "اطلاعات کد تخفیف با موفقیت دریافت شد"
        );
    }

    #endregion

    #region Get Discount By Code

    [HttpGet("code/{code:required}")]
    [ProducesResponseType(typeof(ApiResponse<DiscountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<DiscountResponse>> GetDiscountByCode(
        string code,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting discount - Code: {DiscountCode}", code);

        var result = await mediator.Send(new GetDiscountByCodeQuery(code), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Discount not found - Code: {DiscountCode}", code);
            return result.ToApiResponse<DiscountResponse>("کد تخفیف یافت نشد");
        }

        logger.LogInformation("Discount retrieved - Code: {DiscountCode}", code);

        return result.ToApiResponse(
            mapper.Map<DiscountResponse>,
            "اطلاعات کد تخفیف با موفقیت دریافت شد"
        );
    }

    #endregion

    #region Calculate Discount

    [HttpGet("{discountCode}/calculate")]
    [ProducesResponseType(typeof(ApiResponse<DiscountCalculationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<DiscountCalculationResponse>> CalculateDiscount(
        string discountCode,
        [FromQuery] decimal orderAmount,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Calculating discount - Code: {DiscountCode}, OrderAmount: {OrderAmount}",
            discountCode, orderAmount);
        Guid userId = User.GetUserId();
        Result<DiscountCalculationDto> result = await mediator.Send(
            new CalculateDiscountAmountQuery(userId,discountCode, orderAmount), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Calculate discount failed - Code: {DiscountCode}, Error: {ErrorCode}",
                discountCode, result.Error.Code);

            return result.ToApiResponse<DiscountCalculationResponse>("خطا در محاسبه مقدار تخفیف");
        }

        var response = mapper.Map<DiscountCalculationResponse>(result.Value);

        logger.LogInformation(
            "Discount calculated - Code: {DiscountCode}, DiscountAmount: {Amount}",
            discountCode, response.DiscountAmount);

        return result.ToApiResponse(
            mapper.Map<DiscountCalculationResponse>,
            "مقدار تخفیف با موفقیت محاسبه شد"
        );
    }

    #endregion

    #region Check Discount Validity

    [HttpGet("{discountId:guid}/validity")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<bool>> CheckDiscountValidity(
        Guid discountId,
        [FromQuery] Guid? userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Checking discount validity - Id: {DiscountId}, UserId: {UserId}",
            discountId, userId);

        Result<bool> result = await mediator.Send(
            new CheckDiscountValidityQuery(discountId, userId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Check discount validity failed - Id: {DiscountId}, Error: {ErrorCode}",
                discountId, result.Error.Code);

            return result.ToApiResponse<bool>("خطا در بررسی اعتبار کد تخفیف");
        }

        logger.LogInformation(
            "Discount validity checked - Id: {DiscountId}, IsValid: {IsValid}",
            discountId, result.Value);

        return result.ToApiResponse(
            t => t,
            "وضعیت اعتبار کد تخفیف با موفقیت بررسی شد"
        );
    }

    #endregion

    #region List Discounts

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<DiscountResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<DiscountResponse>>> ListDiscounts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Listing discounts - Page: {Page}, Size: {Size}",
            pageNumber, pageSize);

        Result<PaginatedResult<DiscountDto>> result = await mediator.Send(
            new ListDiscountsQuery(pageNumber, pageSize), cancellationToken);

        if (result.IsFailure)
            return result.ToApiResponse<PaginatedResponse<DiscountResponse>>("خطا در دریافت لیست تخفیف‌ها");

        var response = mapper.Map<PaginatedResponse<DiscountResponse>>(result.Value);

        logger.LogInformation(
            "Discounts listed - Count: {Count}, Page: {Page}/{Total}",
            response.Items.Count, pageNumber, result.Value.TotalPages);

        return result.ToApiResponse(
            mapper.Map<PaginatedResponse<DiscountResponse>>,
            "لیست تخفیف‌ها با موفقیت دریافت شد"
        );
    }

    #endregion
}
