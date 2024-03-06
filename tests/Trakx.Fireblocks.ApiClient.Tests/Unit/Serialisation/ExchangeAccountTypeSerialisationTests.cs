namespace Trakx.Fireblocks.ApiClient.Tests.Unit.Serialisation;

public class ExchangeAccountTypeSerialisationTests
{

    [Fact]
    public void Should_Serialise_exchange_response_back_and_forth()
    {
        var responseContent = File.ReadAllText("Unit/Serialisation/sampleGetAllExchangesResponse.json");
        var exchanges = Newtonsoft.Json.JsonConvert.DeserializeObject<ExchangeAccount[]>(responseContent);
        exchanges!.Length.Should().Be(9);
    }
}
