using System.Collections.ObjectModel;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Courses.Events;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Courses;

public sealed class Course : AggregateRoot<CourseId>
{
    private readonly List<Section> _sections = [];
    private readonly HashSet<CourseTag> _tags = [];
    private readonly HashSet<CourseCategory> _categories = [];

    // ------------------------
    // Properties
    // ------------------------
    public CourseTitle Title { get; private set; }
    public Description Description { get; private set; }
    public ShortDescription ShortDescription { get; private set; }
    public Slug Slug { get; private set; }

    public CourseImageName? ImageName { get; private set; }
    public CourseThumbnailName? ThumbnailName { get; private set; }
    public CourseVideoName? DemoVideoName { get; private set; }

    public Money Price => GetTotalContentValue();
    public CourseStatus Status { get; private set; }
    public CourseLevel Level { get; private set; }

    public bool HasCertificate { get; private set; }
    public bool IsPublished => Status == CourseStatus.Published;

    public InstructorId InstructorId { get; private set; }

    public DateTimeOffset?  PublishedAt { get; private set; }
    public DateTimeOffset?  ArchivedAt { get; private set; }

    // Read-only collections
    public IReadOnlyCollection<Section> Sections => _sections.AsReadOnly();
    public IReadOnlyCollection<CourseTag> Tags => _tags;
    public IReadOnlyCollection<CourseCategory> Categories => _categories;
    public List<CourseRating> Ratings { get; set; } = [];
    public decimal AverageRating => Ratings.Any() ? Math.Round(Ratings.Average(r => r.Rating.Value),1) : 0;
    public int TotalRatings => Ratings.Count;

    // ------------------------
    // Constructors
    // ------------------------
    private Course(
        CourseId id,
        CourseTitle title,
        Description description,
        ShortDescription shortDescription,
        Slug slug,
        CourseLevel level,
        InstructorId instructorId) : base(id)
    {
        Title = title;
        Description = description;
        ShortDescription = shortDescription;
        Slug = slug;
        Level = level;
        InstructorId = instructorId;
        Status = CourseStatus.Draft;
        HasCertificate = false;
    }

    private Course() : base()
    {
    } // EF Core

    // ------------------------
    // Factory Method
    // ------------------------

    /// <summary>
    /// Creates a new course
    /// </summary>
    public static Result<Course> Create(
        string title,
        string description,
        string shortDescription,
        string slug,
        CourseLevel level,
        InstructorId instructorId,
        string? currency = null
    )
    {
        var titleResult = CourseTitle.Create(title);
        if (titleResult.IsFailure)
            return titleResult.Error;

        var descriptionResult = Description.Create(description);
        if (descriptionResult.IsFailure)
            return descriptionResult.Error;

        var shortDescResult = ShortDescription.Create(shortDescription);
        if (shortDescResult.IsFailure)
            return shortDescResult.Error;

        var slugResult = Slug.Create(slug);
        if (slugResult.IsFailure)
            return slugResult.Error;

        var currencyResult = Currency.FromCode(currency ?? Money.DefaultCurrencyCode);
        if (currencyResult.IsFailure)
            return CourseErrors.InvalidCurrency;

        if (instructorId is null)
            return CourseErrors.InvalidInstructor;

        var course = new Course(
            CourseId.New(),
            titleResult.Value,
            descriptionResult.Value,
            shortDescResult.Value,
            slugResult.Value,
            level,
            instructorId
        );

        course.RaiseDomainEvent( CourseCreatedDomainEvent.Create(
            course.Id,
            course.Title,
            course.InstructorId
        ));

        return course;
    }

    // ------------------------
    // Basic Information Updates
    // ------------------------

    public Result UpdateTitle(CourseTitle newTitle)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        Title = newTitle;


        return Result.Success();
    }

    public Result UpdateDescription(Description description)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        Description = description;
        return Result.Success();
    }
    public Result UpdateDescriptionWithShortDescription(Description description,ShortDescription shortDescription)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;
        Description = description;
        ShortDescription = shortDescription;
        return Result.Success();
    }
    public Result UpdateShortDescription(ShortDescription shortDescription)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        ShortDescription = shortDescription;


        return Result.Success();
    }

    public Result UpdateSlug(Slug newSlug)
    {
        if (newSlug is null)
            return CourseErrors.InvalidSlug;

        if (IsPublished)
            return CourseErrors.CannotChangeSlugAfterPublish;

        Slug = newSlug;


        return Result.Success();
    }

    public Result ChangeLevel(CourseLevel newLevel)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        Level = newLevel;


        return Result.Success();
    }

    public Result ChangeInstructor(InstructorId newInstructorId)
    {
        if (newInstructorId is null)
            return CourseErrors.InvalidInstructor;

        if (IsPublished)
            return CourseErrors.CannotChangeInstructorAfterPublish;

        InstructorId = newInstructorId;


        return Result.Success();
    }

    // ------------------------
    // Media Management
    // ------------------------

    public Result SetCourseImage(CourseImageName imageName, CourseThumbnailName? thumbnailName = null)
    {
        if (imageName is null)
            return CourseErrors.InvalidImageName;

        ImageName = imageName;
        ThumbnailName = thumbnailName ?? ThumbnailName;


        return Result.Success();
    }

    public Result SetDemoVideo(CourseVideoName videoName)
    {
        if (videoName is null)
            return CourseErrors.InvalidVideoName;

        DemoVideoName = videoName;


        return Result.Success();
    }

    public Result RemoveDemoVideo()
    {
        DemoVideoName = null;


        return Result.Success();
    }

    // ------------------------
    // Section Management
    // ------------------------
    public Result UpdateSections(IEnumerable<Section> sections)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        IEnumerable<Section> enumerable = sections as Section[] ?? sections.ToArray();
        if ( !enumerable.Any())
            return CourseErrors.SectionsCannotBeEmpty;

        // Check for non-duplicates titles
        var titles = enumerable.Select(s => s.Title.Value).ToList();
        if (titles.Count != titles.Distinct(StringComparer.OrdinalIgnoreCase).Count())
            return CourseErrors.SectionTitleDuplicate;

        // Check for non-duplicates DisplayOrder
        var displayOrders = enumerable.Select(s => s.DisplayOrder).ToList();
        if (displayOrders.Count != displayOrders.Distinct().Count())
            return CourseErrors.DuplicateDisplayOrder;

        // check to all section belong to this course
        if (enumerable.Any(s => !s.CourseId.Equals(Id)))
            return CourseErrors.SectionDoesNotBelongToCourse;

        _sections.Clear();
        _sections.AddRange(enumerable);

        return Result.Success();
    }

    public Result<Section> AddSection(string title, int displayOrder)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        var titleResult = SectionTitle.Create(title);
        if (titleResult.IsFailure)
            return titleResult.Error;

        // Check for duplicate title
        if (_sections.Any(s => s.Title.Value.Equals(title, StringComparison.OrdinalIgnoreCase)))
            return CourseErrors.SectionTitleDuplicate;

        var sectionResult = Section.Create(Id, titleResult.Value, displayOrder);
        if (sectionResult.IsFailure)
            return sectionResult.Error;

        _sections.Add(sectionResult.Value);


        RaiseDomainEvent( SectionAddedDomainEvent.Create(
            Id,
            sectionResult.Value.Id,
            titleResult.Value
        ));

        return sectionResult.Value;
    }

    public Result RemoveSection(SectionId sectionId)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        Section? section = _sections.FirstOrDefault(s => s.Id.Equals(sectionId));
        if (section is null)
            return CourseErrors.SectionNotFound;

        if(section.Episodes.Count>0)
            return CourseErrors.CannotRemoveSectionThatHasEpisode;
        _sections.Remove(section);


        RaiseDomainEvent( SectionRemovedDomainEvent.Create(Id, sectionId));

        return Result.Success();
    }

    public Result UpdateSectionTitle(SectionId sectionId, SectionTitle newTitle)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        var section = _sections.FirstOrDefault(s => s.Id.Equals(sectionId));
        if (section is null)
            return CourseErrors.SectionNotFound;

        var result = section.UpdateTitle(newTitle);
        if (result.IsFailure)
            return result;


        return Result.Success();
    }

    public Result ReorderSections(List<SectionId> orderedSectionIds)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        if (orderedSectionIds.Count != _sections.Count)
            return CourseErrors.InvalidSectionOrder;

        // Verify all section IDs exist
        if (!orderedSectionIds.All(id => _sections.Any(s => s.Id.Equals(id))))
            return CourseErrors.InvalidSectionOrder;

        // Reorder
        var orderedSections = orderedSectionIds
            .Select(id => _sections.First(s => s.Id.Equals(id)))
            .ToList();

        _sections.Clear();
        _sections.AddRange(orderedSections);

        // Update display order
        for (int i = 0; i < _sections.Count; i++)
        {
            _sections[i].UpdateDisplayOrder(i + 1);
        }


        return Result.Success();
    }

    // ------------------------
    // Episode Management (через Section)
    // ------------------------

    public Result<Episode> AddEpisode(SectionId sectionId,
        EpisodeId episodeId,
        EpisodeTitle title,
        TimeSpan duration,
        Money price,
        EpisodeVideoName? videoFileName = null)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        Section? section = _sections.FirstOrDefault(s => s.Id.Equals(sectionId));
        if (section is null)
            return CourseErrors.SectionNotFound;
        Result<Episode> episodeResult = section.AddEpisode(episodeId,title, duration, price, videoFileName);
        if (episodeResult.IsFailure)
            return episodeResult.Error;


        RaiseDomainEvent( EpisodeAddedDomainEvent.Create(
            Id,
            sectionId,
            episodeResult.Value.Id,
            title
        ));

        return episodeResult.Value;
    }

    public Result UpdateEpisode(
        SectionId sectionId,
        EpisodeId episodeId,
        string? title = null,
        TimeSpan? duration = null,
        decimal? price = null,
        string? currency = null,
        string? videoFileName = null)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        var section = _sections.FirstOrDefault(s => s.Id.Equals(sectionId));
        if (section is null)
            return CourseErrors.SectionNotFound;

        var result = section.UpdateEpisode(episodeId, title, duration, price, currency, videoFileName);
        if (result.IsFailure)
            return result;


        return Result.Success();
    }

    public Result RemoveEpisode(SectionId sectionId, EpisodeId episodeId)
    {
        if (IsPublished)
            return CourseErrors.CannotModifyPublishedCourse;

        Section? section = _sections.FirstOrDefault(s => s.Id.Equals(sectionId));
        if (section is null)
            return CourseErrors.SectionNotFound;

        var result = section.RemoveEpisode(episodeId);
        if (result.IsFailure)
            return result;


        return Result.Success();
    }

    // ------------------------
    // Tags & Categories
    // ------------------------

    public Result AddTag(TagId tagId)
    {
        if (tagId is null)
            return CourseErrors.InvalidTag;

        if (!_tags.Add( CourseTag.Create(Id, tagId)))
            return CourseErrors.TagAlreadyAdded;


        return Result.Success();
    }

    public Result RemoveTag(TagId tagId)
    {
        var tagToRemove = _tags.FirstOrDefault(t => t.Id.Equals(tagId));
        if (tagToRemove is null)
            return CourseErrors.TagNotFound;
        if (!_tags.Remove(tagToRemove))
            return CourseErrors.TagNotFound;


        return Result.Success();
    }

    public Result SetTags(IEnumerable<TagId> tagIds)
    {
        _tags.Clear();

        foreach (var tagId in tagIds)
        {
            if (tagId is not null)
                _tags.Add(CourseTag.Create(Id, tagId) );
        }


        return Result.Success();
    }

    public Result AddCategory(CategoryId categoryId)
    {
        if (categoryId is null)
            return CourseErrors.InvalidCategory;

        if (!_categories.Add(CourseCategory.Create(Id,categoryId)))
            return CourseErrors.CategoryAlreadyAdded;


        return Result.Success();
    }

    public Result RemoveCategory(CategoryId categoryId)
    {
        var categoryToRemove = _categories.FirstOrDefault(c => c.Id.Equals(categoryId));
        if (categoryToRemove is null)
            return CourseErrors.CategoryNotFound;
        if (!_categories.Remove(categoryToRemove))
            return CourseErrors.CategoryNotFound;


        return Result.Success();
    }

    public Result SetCategories(IEnumerable<CategoryId> categoryIds)
    {
        _categories.Clear();

        foreach (var categoryId in categoryIds)
        {
            if (categoryId is not null)
                _categories.Add(CourseCategory.Create(Id, categoryId));
        }


        return Result.Success();
    }

    // ------------------------
    // Certificate Management
    // ------------------------

    public Result EnableCertificate()
    {
        if (HasCertificate)
            return CourseErrors.CertificateAlreadyEnabled;

        HasCertificate = true;


        return Result.Success();
    }

    public Result DisableCertificate()
    {
        if (!HasCertificate)
            return CourseErrors.CertificateAlreadyDisabled;

        HasCertificate = false;


        return Result.Success();
    }

    // ------------------------
    // Publishing & Status
    // ------------------------

    public Result Publish()
    {
        if (IsPublished)
            return CourseErrors.AlreadyPublished;

        if (_sections.Count == 0)
            return CourseErrors.CannotPublishWithoutSections;

        if (_sections.Any(s => s.Episodes.Count == 0))
            return CourseErrors.CannotPublishWithEmptySections;

        Status = CourseStatus.Published;
        PublishedAt = DateTime.UtcNow;
        ArchivedAt = null;


        RaiseDomainEvent( CoursePublishedDomainEvent.Create(Id, Title));

        return Result.Success();
    }

    public Result Unpublish()
    {
        if (!IsPublished)
            return CourseErrors.NotPublished;

        Status = CourseStatus.Draft;
        PublishedAt = null;
        ArchivedAt = null;

        RaiseDomainEvent( CourseUnpublishedDomainEvent.Create(Id));

        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == CourseStatus.Archived)
            return CourseErrors.AlreadyArchived;

        Status = CourseStatus.Archived;
        ArchivedAt = DateTime.UtcNow;
        PublishedAt = null;

        RaiseDomainEvent( CourseArchivedDomainEvent.Create(Id));

        return Result.Success();
    }

    // ------------------------
    // Query Methods
    // ------------------------

    public int GetTotalEpisodes() => _sections.Sum(s => s.Episodes.Count);

    public int GetTotalSections() => _sections.Count;

    public TimeSpan GetTotalDuration() =>
        TimeSpan.FromSeconds(_sections.Sum(s => s.GetTotalDuration().TotalSeconds));

    public int GetFreeEpisodesCount() =>
        _sections.Sum(s => s.Episodes.Count(e => e.IsFree));

    public Money GetTotalContentValue()
    {
        var total = Money.Zero();
        foreach (Section section in _sections)
        {
            total.Add( section.GetTotalPrice());
        }
        return total;
        // return _sections
        //     .SelectMany(s => s.Episodes)
        //     .Aggregate(Money.Create(0, Price.Currency.Code).Value,
        //         (total, ep) => total.Add(ep.Price).Value);
    }

    public bool HasFreeContent() =>
        _sections.Any(s => s.Episodes.Any(e => e.IsFree));

    public Section? GetSection(SectionId sectionId) =>
        _sections.FirstOrDefault(s => s.Id.Equals(sectionId));


}
