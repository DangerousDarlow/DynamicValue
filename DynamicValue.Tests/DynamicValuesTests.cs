namespace DynamicValue.Tests;

public class DynamicValuesTests
{
    private readonly Guid _context = Guid.Parse("e35ede8f-a931-494b-bf07-ab67b1b2b744");

    private readonly DynamicValueId _id = Guid.Parse("673c44c5-1291-46e1-a193-2d3db8d84cd6").ToDynamicValueId();

    private DynamicValues _dynamicValues;

    [SetUp]
    public void Setup() => _dynamicValues = new DynamicValues();

    [Test]
    public void Dynamic_value_for_unknown_id_is_default()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_dynamicValues.Get(_id), Is.EqualTo(DynamicValues.Default));
            Assert.That(_dynamicValues.IsEnabled(_id), Is.False);
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Dynamic_value_for_known_id_is_as_set(bool enabled)
    {
        _dynamicValues.Set(_id, new DynamicValue(enabled, "blob"));

        var dynamicValue = _dynamicValues.Get(_id);

        Assert.Multiple(() =>
        {
            Assert.That(dynamicValue.Enabled, Is.EqualTo(enabled));
            Assert.That(dynamicValue.Blob, Is.EqualTo("blob"));
            Assert.That(_dynamicValues.IsEnabled(_id), Is.EqualTo(enabled));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Dynamic_value_for_unknown_context_is_base(bool enabled)
    {
        var baseValue = new DynamicValue(enabled, "blob");
        _dynamicValues.Set(_id, baseValue);

        var dynamicValue = _dynamicValues.Get(_id, [_context]);

        Assert.Multiple(() =>
        {
            Assert.That(dynamicValue, Is.EqualTo(baseValue));
            Assert.That(_dynamicValues.IsEnabled(_id, [_context]), Is.EqualTo(enabled));
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Dynamic_value_contexts_are_evaluated_in_order(bool enabled)
    {
        var baseValue = new DynamicValue(!enabled, "base-blob");
        _dynamicValues.Set(_id, baseValue);

        var contextValue = new DynamicValue(enabled, "context-blob");
        _dynamicValues.Set(_id, _context, contextValue);

        var otherContext1 = Guid.Parse("2d2eaec9-4e70-4d5f-a07e-d3dc2aa2d57a");

        var otherContext2 = Guid.Parse("7464d176-dad7-4d87-a997-d5ad949fe115");
        var otherContext2Value = new DynamicValue(!enabled, "context2-blob");
        _dynamicValues.Set(_id, otherContext2, otherContext2Value);

        var dynamicValue = _dynamicValues.Get(_id, [otherContext1, _context, otherContext2]);

        Assert.Multiple(() =>
        {
            Assert.That(dynamicValue, Is.EqualTo(contextValue));
            Assert.That(_dynamicValues.IsEnabled(_id, [otherContext1, _context, otherContext2]), Is.EqualTo(enabled));
        });
    }
}