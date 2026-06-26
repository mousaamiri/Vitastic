using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Instructors.ValueObjects;

public sealed class InstructorSkills : ValueObject
{
    private const int MaxSkills = 20;

    public IReadOnlyCollection<InstructorSkill> Values => _value.AsReadOnly();
    private readonly List<InstructorSkill> _value=[];
    //Ef ctor
    public InstructorSkills() { }
    //Private ctor
    private InstructorSkills(IEnumerable<InstructorSkill> skills)
    {
        _value = skills.ToList();
    }

    // Factory method
    public static Result<InstructorSkills> Create(IEnumerable<InstructorSkill> skills)
    {
        var list = skills?.ToList() ?? new List<InstructorSkill>();

        if (list.Count > MaxSkills)
            return InstructorSkillsErrors.TooMany(MaxSkills);

        // prevent duplicates
        if (list.Select(s => s.Value.ToLower()).Distinct().Count() != list.Count)
            return InstructorSkillsErrors.Duplicated;

        return Result.Success(new InstructorSkills(list));
    }

    // Domain behavior example: add skill
    public Result<InstructorSkills> Add(InstructorSkill skill)
    {
        if (_value.Any(s => s.Value.Equals(skill.Value, StringComparison.OrdinalIgnoreCase)))
            return InstructorSkillsErrors.Duplicated;

        if (_value.Count + 1 > MaxSkills)
            return InstructorSkillsErrors.TooMany(MaxSkills);

        IEnumerable<InstructorSkill> newList = _value.Append(skill);
        return Result.Success(new InstructorSkills(newList));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        foreach (InstructorSkill skill in _value.ToList())
            yield return skill;
    }
}

public static class InstructorSkillsErrors
{
    public static Error TooMany(int max) => Error.Validation(
        "InstructorSkills.TooMany",
        $"تعداد مهارت‌ها نباید بیشتر از {max} باشد");

    public static Error Duplicated => Error.Validation(
        "InstructorSkills.Duplicated",
        "مهارت تکراری مجاز نیست");
}
