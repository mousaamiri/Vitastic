using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Discounts.Commands.Create.Percentage
{
    public sealed class CreatePercentageDiscountCommandHandler(
        IDiscountRepository discountRepository,
        IMapper mapper)
        : ICommandHandler<CreatePercentageDiscountCommand,Guid>
    {
        public async Task<Result<Guid>> Handle(
            CreatePercentageDiscountCommand command,
            CancellationToken ct)
        {
            var startDateUtc = command.StartDate.ToUniversalTime();
            var endDateUtc = command.EndDate.ToUniversalTime();
            Result<DiscountCode> codeResult = DiscountCode.Create(command.Code);
            if (codeResult.IsFailure)
                return codeResult.Error;
            Result<Title> titleResult = Title.Create(command.Title);
            if (titleResult.IsFailure)
                return titleResult.Error;

            var codeIsExist =await discountRepository.CodeIsExistAsync(codeResult.Value, ct);
            if (codeIsExist)
                return Error.Conflict("CreatePercentageDiscountCommand.DiscountCodeIsExist",
                    "کد تخفیفی با این کد موجود است، لطفا کد دیگری انتخاب کنید.");
            var titleIsExist =await discountRepository.TitleIsExistAsync(titleResult.Value, ct);
            if (titleIsExist)
                return Error.Conflict("CreatePercentageDiscountCommand.DiscountTitleIsExist",
                    "کد تخفیفی با این عنوان موجود است، لطفا عنوان دیگری انتخاب کنید.");

            Result<Discount> discountResult = Discount.CreatePercentage(
                codeResult.Value,
                titleResult.Value,
                command.Scope,
                command.Percentage,
                startDateUtc,
                endDateUtc);

            if (discountResult.IsFailure)
                return discountResult.Error;

            var discount = discountResult.Value;

            await discountRepository.AddAsync(discount, ct);

            return discount.Id.Value;
        }
    }
}
