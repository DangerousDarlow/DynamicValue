namespace DynamicValue;

public interface IDynamicValues
{
    DynamicValue Get(DynamicValueId id);

    bool IsEnabled(DynamicValueId id);

    DynamicValue Get(DynamicValueId id, IEnumerable<Guid> context);

    bool IsEnabled(DynamicValueId id, IEnumerable<Guid> context);
}