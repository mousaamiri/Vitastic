using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Domain.Shared.Repositories;

public interface IPaymentTransactionRepository : IRepository<PaymentTransaction, PaymentTransactionId>
{
    Task<bool> WalletExistsAsync(WalletId value, CancellationToken cancellationToken);
    Task<PaymentTransaction?> GetPendingTransactionAsync(CancellationToken cancellation = default);

    /// <summary>
    /// Receive transaction based on Authority (request code from the portal)
    /// </summary>
    Task<PaymentTransaction?> GetByAuthorityAsync(string authority, CancellationToken cancellation = default);

    Task<bool> ExistsAsync(InstructorId instructorId, CancellationToken cancellationToken);

    Task<PaymentTransaction?> IsExistsAsync(GuidBasedId<PaymentTransactionId> value, CancellationToken cancellationToken);
}
