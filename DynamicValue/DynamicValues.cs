using System.Collections.Concurrent;

namespace DynamicValue;

public class DynamicValues : IDynamicValues
{
    private readonly ConcurrentDictionary<DynamicValueId, ConcurrentDictionary<Guid, DynamicValue>> _values = new();

    public static readonly DynamicValue Default = new(false, null);

    public DynamicValue Get(DynamicValueId id)
    {
        if (_values.TryGetValue(id, out var valuesForId) is false)
            return Default;

        return valuesForId.GetValueOrDefault(id.Value, Default);
    }

    public bool IsEnabled(DynamicValueId id)
    {
        if (_values.TryGetValue(id, out var valuesForId) is false)
            return false;

        return valuesForId.TryGetValue(id.Value, out var valueForIdAndDefault) && valueForIdAndDefault.Enabled;
    }

    public DynamicValue Get(DynamicValueId id, IEnumerable<Guid> context)
    {
        if (_values.TryGetValue(id, out var valuesForId) is false)
            return Default;

        foreach (var contextId in context)
        {
            if (valuesForId.TryGetValue(contextId, out var valueForIdAndContext))
                return valueForIdAndContext;
        }

        return valuesForId.GetValueOrDefault(id.Value, Default);
    }

    public bool IsEnabled(DynamicValueId id, IEnumerable<Guid> context)
    {
        if (_values.TryGetValue(id, out var valuesForId) is false)
            return false;

        foreach (var contextId in context)
        {
            if (valuesForId.TryGetValue(contextId, out var valueForIdAndContext))
                return valueForIdAndContext.Enabled;
        }

        return valuesForId.TryGetValue(id.Value, out var valueForIdAndDefault) && valueForIdAndDefault.Enabled;
    }

    public void Set(DynamicValueId id, DynamicValue value) => Set(id, id.Value, value);

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

    public void Set(DynamicValueId id, bool enabled) => Set(id, id.Value, enabled);

    public void Set(DynamicValueId id, Guid context, bool enabled) => _values.AddOrUpdate(
        id,
        _ => new() { [id.Value] = new(enabled, null) },
        (_, valuesForId) =>
        {
            valuesForId.AddOrUpdate(
                context,
                _ => new(enabled, null),
                (_, existing) => existing with { Enabled = enabled });

            return valuesForId;
        });
}