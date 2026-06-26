using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Payments.Commands.AssignInfo;

public sealed record AssignPaymentInfoCommand(
    Guid TransactionId,
    string Authority,
    string Provider) : ICommand;
