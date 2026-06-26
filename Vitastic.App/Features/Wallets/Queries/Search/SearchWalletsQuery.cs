using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Wallets.Queries.Search;

public record SearchWalletsQuery(
    string? SearchTerm,
    int PageNumber=1,
    int PageSize=10):IQuery<PaginatedResult<WalletDto>>;


public sealed class SearchWalletsQueryValidation : AbstractValidator<SearchWalletsQuery>
{
    public SearchWalletsQueryValidation()
    {
        When(x => !string.IsNullOrWhiteSpace(x.SearchTerm), () =>
        {
            RuleFor(x => x.SearchTerm)
                .MinimumLength(1).WithMessage("عبارت جستجو باید حداقل 1 کاراکتر باشد")
                .MaximumLength(100).WithMessage("عبارت جستجو نمی‌تواند بیش از 100 کاراکتر باشد");
        });


        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("شماره صفحه باید بیشتر از 0 باشد");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("تعداد آیتم‌ها باید بین 1 و 100 باشد");
    }
}
public sealed class SearchWalletsQueryHandler
(IWalletQueryService walletQueryService)
    :IQueryHandler<SearchWalletsQuery,PaginatedResult<WalletDto>>
{
    public async Task<Result<PaginatedResult<WalletDto>>> Handle(SearchWalletsQuery request, CancellationToken cancellationToken)
    {
        (IReadOnlyList<WalletDto> wallets,var totalCounts) =await walletQueryService.SearchUsersWalletsAsync(request.SearchTerm, request.PageNumber, request.PageSize,cancellationToken);
        return Result.Success(new PaginatedResult<WalletDto>(wallets,totalCounts,request.PageNumber,request.PageSize)) ;
    }
}
