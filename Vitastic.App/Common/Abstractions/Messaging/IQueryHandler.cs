using MediatR;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Common.Abstractions.Messaging;

internal interface IQueryHandler<in TQuery,TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>?;
