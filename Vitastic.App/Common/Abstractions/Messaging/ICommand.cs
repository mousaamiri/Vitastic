using MediatR;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Abstractions.Messaging
{
	internal interface ICommand:IRequest<Result>;

	internal interface ICommand<TResponse> : IRequest<Result<TResponse>>;
}
