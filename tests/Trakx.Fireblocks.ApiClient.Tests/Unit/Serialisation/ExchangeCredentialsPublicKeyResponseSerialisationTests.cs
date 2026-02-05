namespace Trakx.Fireblocks.ApiClient.Tests.Unit.Serialisation;

public class ExchangeCredentialsPublicKeyResponseSerialisationTests
{
    [Fact]
    public void Should_Deserialise_credentials_public_key_response()
    {
        var responseContent = File.ReadAllText("Unit/Serialisation/sampleCredentialsPublicKeyResponse.json");
        var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ExchangeCredentialsPublicKeyResponse>(responseContent);

        response.Should().NotBeNull();
        response!.PublicKey.Should().NotBeNullOrEmpty();
        response.PublicKey.Should().StartWith("-----BEGIN PUBLIC KEY-----");
        response.PublicKey.Should().EndWith("-----END PUBLIC KEY-----\n");
    }

    [Fact]
    public void Should_Serialise_credentials_public_key_response_back_and_forth()
    {
        var original = new ExchangeCredentialsPublicKeyResponse
        {
            PublicKey = "-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A\n-----END PUBLIC KEY-----"
        };

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(original);
        var deserialised = Newtonsoft.Json.JsonConvert.DeserializeObject<ExchangeCredentialsPublicKeyResponse>(json);

        deserialised.Should().NotBeNull();
        deserialised!.PublicKey.Should().Be(original.PublicKey);
    }
}
