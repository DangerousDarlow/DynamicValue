namespace DynamicValue;

public interface IDynamicValues
{
    DynamicValue Get(DynamicValueId id);

    bool IsEnabled(DynamicValueId id);

    DynamicValue Get(DynamicValueId id, ReadOnlySpan<Guid> context);

    bool IsEnabled(DynamicValueId id, ReadOnlySpan<Guid> context);
}