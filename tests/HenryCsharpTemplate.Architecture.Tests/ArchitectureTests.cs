using FluentAssertions;
using NetArchTest.Rules;

namespace HenryCsharpTemplate.Architecture.Tests;

public class ArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Reference_Application_And_Infrastructure()
    {
        var forbidden = new[] { "HenryCsharpTemplate.Application", "HenryCsharpTemplate.Infrastructure" };
        var result = Types.InNamespace("HenryCsharpTemplate.Domain").ShouldNot().HaveDependencyOnAny(forbidden).GetResult();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_Reference_Infrastructure()
    {
        var forbidden = new[] { "HenryCsharpTemplate.Infrastructure" };
        var result = Types.InNamespace("HenryCsharpTemplate.Application").ShouldNot().HaveDependencyOnAny(forbidden).GetResult();
        result.IsSuccessful.Should().BeTrue();
    }
}
