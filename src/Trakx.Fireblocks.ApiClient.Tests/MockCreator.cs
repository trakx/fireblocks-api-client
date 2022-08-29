using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests;

public class MockCreator : Trakx.Utils.Testing.MockCreator
{
    public MockCreator(ITestOutputHelper output) : base(output)
    {
    }
}
