using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Tags.Dtos;

namespace Vitastic.App.Features.Tags.Queries.GetById;

public record GetTagByIdQuery(Guid Id) : IQuery<TagDto>;
