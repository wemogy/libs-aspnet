using FluentAssertions;
using Wemogy.AspNet.Transformers;
using Xunit;

namespace Wemogy.AspNet.Tests.Transformers;

public class KebabCaseParameterTransformerTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("TypicalControllerName", "typical-controller-name")]
    public void ShouldTransformMixedCaseString(object? value, string? expected)
    {
        var transformer = new KebabCaseParameterTransformer();
        var result = transformer.TransformOutbound(value);

        result.Should().Be(expected);
    }
}
