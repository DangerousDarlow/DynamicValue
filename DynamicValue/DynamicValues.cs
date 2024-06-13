using System.Collections.Concurrent;

namespace DynamicValue;

public class DynamicValues : IDynamicValues
{
    public static readonly DynamicValue Default = new(false, null);

    private readonly ConcurrentDictionary<DynamicValueId, ConcurrentDictionary<Guid, DynamicValue>> _values = new();

    public DynamicValue Get(DynamicValueId id)
    {
        if (_values.TryGetValue(id, out var valuesForId) is false)
            return Default;

        return valuesForId.GetValueOrDefault(id.Value, Default);
    }

    public bool IsEnabled(DynamicValueId id) => Get(id).Enabled;

    public DynamicValue Get(DynamicValueId id, ReadOnlySpan<Guid> context)
    {
        if (_values.TryGetValue(id, out var valuesForId) is false)
            return Default;

        foreach (var contextId in context)
            if (valuesForId.TryGetValue(contextId, out var valueForIdAndContext))
                return valueForIdAndContext;

        return valuesForId.GetValueOrDefault(id.Value, Default);
    }

    public bool IsEnabled(DynamicValueId id, ReadOnlySpan<Guid> context) => Get(id, context).Enabled;

    public void Set(DynamicValueId id, DynamicValue value) => Set(id, id.Value, value);

    public void Set(DynamicValueId id, bool enabled) => Set(id, id.Value, enabled);

    public void Set(DynamicValueId id, Guid context, DynamicValue value) => _values.AddOrUpdate(
        id,
        _ => new() { [id.Value] = value },
        (_, valuesForId) =>
        {
            valuesForId.AddOrUpdate(
                context,
                _ => value,
                (_, _) => value);

            return valuesForId;
        });

    public void Set(DynamicValueId id, Guid context, bool enabled) => Set(id, context, new DynamicValue(enabled, null));
}