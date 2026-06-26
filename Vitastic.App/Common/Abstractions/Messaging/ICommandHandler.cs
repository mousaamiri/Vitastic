using MediatR;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Abstractions.Messaging
{
	internal interface ICommandHandler<in TCommand> : IRequestHandler<TCommand,Result>
    where TCommand : ICommand;
	internal interface ICommandHandler<in TCommand,TResponse> : IRequestHandler<TCommand,Result<TResponse>>
		where TCommand : ICommand<TResponse>;
}
