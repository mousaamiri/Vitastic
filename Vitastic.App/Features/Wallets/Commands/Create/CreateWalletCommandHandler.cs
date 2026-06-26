using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Wallets.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Wallets.Commands.Create
{
    public sealed class CreateWalletCommandHandler(IWalletRepository walletRepository,IMapper mapper)
        : ICommandHandler<CreateWalletCommand,Guid>
    {
        public async Task<Result<Guid>> Handle(CreateWalletCommand command, CancellationToken cancellationToken)
        {
            var userIdResult = UserId.CreateFrom(command.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            // Check user is exist
            bool userIsExist = await walletRepository
                .CheckUserIsExist(userIdResult.Value, cancellationToken);
            if (!userIsExist)
                return Error.NotFound("CreateWalletCommand.UserNotFound", "کاربری با این شناسه یافت نشد.");
            bool userHasWallet = await walletRepository
                .CheckUserHasWalletAsync(userIdResult.Value, cancellationToken);
            if (userHasWallet)
                return Error.Conflict("CreateWalletCommand.UserHasWallet", "این کاربر از قبل یک کیف پول دارد.");

            //Create wallet
            var walletResult = Wallet.Create(userIdResult.Value, command.CurrencyCode);
            if (walletResult.IsFailure)
                return walletResult.Error;

            //Update
            await walletRepository.AddAsync(walletResult.Value, cancellationToken);
            return walletResult.Value.Id.Value;
        }
    }
}
