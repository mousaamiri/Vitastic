using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses;

public sealed class Section : FullEntity<SectionId>
{
    private readonly List<Episode> _episodes = [];

    // ------------------------
    // Properties
    // ------------------------
    public CourseId CourseId { get; private set; }
    public SectionTitle Title { get; private set; }
    public int DisplayOrder { get; private set; }

    public IReadOnlyCollection<Episode> Episodes => _episodes.AsReadOnly();

    // ------------------------
    // Constructors
    // ------------------------
    private Section(
        SectionId id,
        CourseId courseId,
        SectionTitle title,
        int displayOrder) : base(id)
    {
        CourseId = courseId;
        Title = title;
        DisplayOrder = displayOrder;
    }

    private Section() : base() { } // EF Core

    // ------------------------
    // Factory Method
    // ------------------------

    public static Result<Section> Create(CourseId courseId, SectionTitle title, int displayOrder)
    {
        if (courseId is null)
            return SectionErrors.InvalidCourseId;

        if (title is null)
            return SectionErrors.InvalidTitle;

        if (displayOrder < 1)
            return SectionErrors.InvalidDisplayOrder;

        return new Section(SectionId.New(), courseId, title, displayOrder);
    }

    // ------------------------
    // Behaviors
    // ------------------------

    public Result UpdateTitle(SectionTitle newTitle)
    {
        if (newTitle is null)
            return SectionErrors.InvalidTitle;

        Title = newTitle;
        return Result.Success();
    }

    public void UpdateDisplayOrder(int order)
    {
        DisplayOrder = order;
    }

    public Result<Episode> AddEpisode(EpisodeId episodeId, EpisodeTitle title,
        TimeSpan duration,
        Money price,
        EpisodeVideoName? videoFileName = null)
    {
        var displayOrder = _episodes.Count + 1;

        Result<Episode> episodeResult = Episode.Create(episodeId, Id, title, duration, price, displayOrder,
            videoFileName);

        if (episodeResult.IsFailure)
            return episodeResult.Error;

        _episodes.Add(episodeResult.Value);
        return episodeResult.Value;
    }

    public Result RemoveEpisode(EpisodeId episodeId)
    {
        var episode = _episodes.FirstOrDefault(e => e.Id.Equals(episodeId));
        if (episode is null)
            return SectionErrors.EpisodeNotFound;

        _episodes.Remove(episode);
        // Reorder remaining episodes
        for (int i = 0; i < _episodes.Count; i++)
        {
            _episodes[i].UpdateDisplayOrder(i + 1);
        }

        return Result.Success();
    }

    public Result ReorderEpisodes(List<EpisodeId> orderedEpisodeIds)
    {
        if (orderedEpisodeIds.Count != _episodes.Count)
            return SectionErrors.InvalidEpisodeOrder;

        if (!orderedEpisodeIds.All(id => _episodes.Any(e => e.Id.Equals(id))))
            return SectionErrors.InvalidEpisodeOrder;

        var orderedEpisodes = orderedEpisodeIds
            .Select(id => _episodes.First(e => e.Id.Equals(id)))
            .ToList();

        _episodes.Clear();
        _episodes.AddRange(orderedEpisodes);

        for (int i = 0; i < _episodes.Count; i++)
        {
            _episodes[i].UpdateDisplayOrder(i + 1);
        }


        return Result.Success();
    }

    // ------------------------
    // Query Methods
    // ------------------------

    public TimeSpan GetTotalDuration() =>
        TimeSpan.FromSeconds(_episodes.Sum(e => e.Duration.TotalSeconds));

    public int GetEpisodeCount() => _episodes.Count;

    public Episode? GetEpisode(EpisodeId episodeId) =>
        _episodes.FirstOrDefault(e => e.Id.Equals(episodeId));

    //Get section price || Just in IRToman for now //TODO: Handle multiple currencies
    public Money GetTotalPrice()
    {
        var total = Money.Zero();
        foreach (Episode episode in _episodes)
        {
            total.Add( episode.Price);
        }
        return total;
        //return _episodes.Aggregate(Money.Zero(), (total, episode) => total.Add(episode.Price).Value);
    }

    public Result UpdateEpisode(EpisodeId episodeId, string? title, TimeSpan? duration, decimal? price, string? currency, string? videoFileName)
    {
        var episode = _episodes.FirstOrDefault(e => e.Id.Equals(episodeId));
        if (episode is null)
            return SectionErrors.EpisodeNotFound;

        var titleResult = title != null
            ? EpisodeTitle.Create(title)
            : Result.Success(episode.Title);
        if (titleResult.IsFailure)
            return titleResult.Error;

        var priceResult = (price != null && currency != null)
            ? Money.Create(price.Value, currency)
            : Result.Success(episode.Price);
        if (priceResult.IsFailure)
            return priceResult.Error;

        var videoNameResult = videoFileName != null
            ? EpisodeVideoName.Create(videoFileName)
            : Result.Success(episode.VideoFileName)!;
        if (videoNameResult.IsFailure)
            return videoNameResult.Error;

        episode.UpdateTitle(titleResult.Value);
        if (duration != null)
            episode.UpdateDuration(duration.Value);
        if (price != null && currency != null)
            episode.UpdatePrice(priceResult.Value);
        if (videoFileName != null)
            episode.UpdateVideoFileName(videoNameResult.Value);

        return Result.Success();
    }
}
