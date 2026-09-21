using ChildcareCalculator.Domain.Enums;

namespace ChildcareCalculator.Domain.Models.Base;

public class ValueWithCategory<T> : ValueObject where T : struct
{
    public T Value { get; private set; }
    public InputCategory Category { get; private set; }
    public string? Unit { get; private set; }
    public string? Note { get; private set; }

    public ValueWithCategory(T value, InputCategory category, string? unit = null, string? note = null)
    {
        Value = value;
        Category = category;
        Unit = unit;
        Note = note;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Category;
        yield return Unit ?? string.Empty;
        yield return Note ?? string.Empty;
    }

    public static implicit operator T(ValueWithCategory<T> valueObject) => valueObject.Value;
}
