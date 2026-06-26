
using MediatR;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Abstractions.Messaging;

internal interface IQuery<TResponse>:IRequest<Result<TResponse>>;
