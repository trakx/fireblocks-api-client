namespace Trakx.Fireblocks.ApiClient.Tests.Unit;

public class GeneratedCodeTests
{
    [Fact]
    public void Models_should_be_records()
    {
        var content = File.ReadAllText(ProjectPaths.ModelsFile);
        content.Should().Contain("public partial record");
    }
}