using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Categories.Dtos;

namespace Vitastic.App.Features.Categories.Queries.GetById;

public record GetCategoryByIdQuery(Guid CategoryId) : IQuery<CategoryDetailDto>;
