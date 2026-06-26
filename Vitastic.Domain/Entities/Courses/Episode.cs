using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Courses;

public sealed class Episode : FullEntity<EpisodeId>
{
    // ------------------------
    // Properties
    // ------------------------
    public SectionId SectionId { get; private set; }
    public EpisodeTitle Title { get; private set; }
    public EpisodeVideoName? VideoFileName { get; private set; }
    public TimeSpan Duration { get; private set; }
    public Money Price { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsFree => Price.Value == 0;

    // ------------------------
    // Constructors
    // ------------------------
    private Episode(
        EpisodeId id,
        SectionId sectionId,
        EpisodeTitle title,
        TimeSpan duration,
        Money price,
        int displayOrder,
        EpisodeVideoName? videoFileName) : base(id)
    {
        SectionId = sectionId;
        Title = title;
        Duration = duration;
        Price = price;
        DisplayOrder = displayOrder;
        VideoFileName = videoFileName;
    }

    private Episode() : base() { } // EF Core

    // ------------------------
    // Factory Method
    // ------------------------

    internal static Result<Episode> Create(EpisodeId episodeId, SectionId sectionId,
        EpisodeTitle title,
        TimeSpan duration,
        Money price,
        int displayOrder,
       EpisodeVideoName? videoFileName = null)
    {
        if (sectionId is null)
            return EpisodeErrors.InvalidSectionId;

        if (duration <= TimeSpan.Zero)
            return EpisodeErrors.InvalidDuration;

        if (displayOrder < 1)
            return EpisodeErrors.InvalidDisplayOrder;

        return new Episode(
            episodeId, sectionId, title, duration, price, displayOrder, videoFileName);
    }

    // ------------------------
    // Behaviors
    // ------------------------

    internal Result UpdateTitle(EpisodeTitle newTitle)
    {
        if (newTitle is null)
            return EpisodeErrors.InvalidTitle;

        Title = newTitle;
        return Result.Success();
    }

    internal Result UpdateDuration(TimeSpan newDuration)
    {
        if (newDuration <= TimeSpan.Zero)
            return EpisodeErrors.InvalidDuration;

        Duration = newDuration;
        return Result.Success();
    }

    internal Result UpdatePrice(Money newPrice)
    {
        if (newPrice is null || newPrice.Value < 0)
            return EpisodeErrors.InvalidPrice;

        if (newPrice.Currency != Price.Currency)
            return EpisodeErrors.CurrencyMismatch;

        Price = newPrice;
        return Result.Success();
    }

    internal Result MakeFree()
    {
        var freePrice = Money.Create(0, Price.Currency.Code).Value;
        Price = freePrice;
        return Result.Success();
    }

    internal Result SetVideoFile(EpisodeVideoName videoName)
    {
        if (videoName is null)
            return EpisodeErrors.InvalidVideoName;

        VideoFileName = videoName;
        return Result.Success();
    }

    internal void UpdateDisplayOrder(int order)
    {
        DisplayOrder = order;
    }

    public void UpdateVideoFileName(EpisodeVideoName value)
    {
        VideoFileName = value;
    }
}
