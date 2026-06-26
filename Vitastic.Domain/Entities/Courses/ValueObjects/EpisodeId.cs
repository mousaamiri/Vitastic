using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Courses.ValueObjects
{
    public sealed class EpisodeId: GuidBasedId<EpisodeId>
    {
        //Constructor
        private EpisodeId(Guid value) : base(value) { }
        //Override
        protected override EpisodeId Create(Guid value) => new(value);
        public static EpisodeId New() => new(Guid.NewGuid());
        public static Result<EpisodeId> CreateFrom(Guid value) =>
            CreateFrom(value, guid => new EpisodeId(guid), BaseIdErrors.Empty);
        public static Result<EpisodeId> CreateFrom(string value) =>
            CreateFrom(
                value,
                guid => new EpisodeId(guid),
                BaseIdErrors.Empty,
                BaseIdErrors.InvalidFormat(value));
    }

}
