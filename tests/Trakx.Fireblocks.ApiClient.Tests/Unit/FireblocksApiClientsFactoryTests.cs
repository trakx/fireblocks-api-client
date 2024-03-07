using System.Collections;
using System.Security.Cryptography;
using Trakx.Common.DateAndTime;
using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit;

public class FireblocksApiClientsFactoryTests
{
    [Theory]
    [ClassData(typeof(FireblocksApiClientsFactoryTestsData))]
    public void CreateApiClient_should_create_new_client_with_given_credentials(
        Func<FireblocksApiCredentialsConfiguration, IFireblocksApiClientBase> func,
        Type expectedInterfaceType,
        Type expectedImplementationType)
    {
        // Arrange
        using var rsa = RSA.Create();
        var privateKey = rsa.ExportPkcs8PrivateKey();

        var newApiCredentials = new FireblocksApiCredentialsConfiguration
        {
            ApiPubKey = "new-public-key",
            ApiPrivateKey = Convert.ToBase64String(privateKey)
        };

        // Act
        var client = func(newApiCredentials);

        // Assert
        client.Should().NotBeNull();
        client.Should().BeAssignableTo(expectedInterfaceType);
        client.Should().BeOfType(expectedImplementationType);

        var clientImplementation = (AuthorisedClient)client;
        clientImplementation.CredentialsProvider.Should().BeOfType<ApiKeyCredentialsProvider>();

        var credentialsProvider = (ApiKeyCredentialsProvider)clientImplementation.CredentialsProvider;
        credentialsProvider.ApiCredentialsConfiguration.Should().BeEquivalentTo(newApiCredentials);

        var bearerCredentialsProvider = (BearerCredentialsProvider)credentialsProvider.BearerCredentialsProvider;
        bearerCredentialsProvider.ApiCredentialsConfiguration.Should().BeEquivalentTo(newApiCredentials);
    }
}

public class FireblocksApiClientsFactoryTestsData : IEnumerable<object[]>
{
    public readonly FireblocksApiClientsFactory Factory;

    public FireblocksApiClientsFactoryTestsData()
    {
        var apiConfiguration = new FireblocksApiConfiguration
        {
            BaseUrl = new Uri("https://localhost:5001"),
            ApiPubKey = "original-public-key",
            ApiPrivateKey = "original-private-key"
        };

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();

        Factory = new FireblocksApiClientsFactory(apiConfiguration, httpClientFactory, dateTimeProvider);
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return MakeMemberData<IAudit_LogsClient, Audit_LogsClient>();
        yield return MakeMemberData<IContractsClient, ContractsClient>();
        yield return MakeMemberData<IExchange_accountsClient, Exchange_accountsClient>();
        yield return MakeMemberData<IExternal_walletsClient, External_walletsClient>();
        yield return MakeMemberData<IFiat_accountsClient, Fiat_accountsClient>();
        yield return MakeMemberData<IGas_stationsClient, Gas_stationsClient>();
        yield return MakeMemberData<IInternal_walletsClient, Internal_walletsClient>();
        yield return MakeMemberData<INetwork_connectionsClient, Network_connectionsClient>();
        yield return MakeMemberData<IOff_exchangesClient, Off_exchangesClient>();
        yield return MakeMemberData<ISupported_assetsClient, Supported_assetsClient>();
        yield return MakeMemberData<ITransactionsClient, TransactionsClient>();
        yield return MakeMemberData<IUsersClient, UsersClient>();
        yield return MakeMemberData<IVaultsClient, VaultsClient>();
        yield return MakeMemberData<IWebhooksClient, WebhooksClient>();
        yield return MakeMemberData<INFTs_BetaClient, NFTs_BetaClient>();
        yield return MakeMemberData<IWeb3_connectionsClient, Web3_connectionsClient>();
        yield return MakeMemberData<IPayments__Cross_Border_settlementClient, Payments__Cross_Border_settlementClient>();
        yield return MakeMemberData<IPayments__PayoutClient, Payments__PayoutClient>();
        yield return MakeMemberData<ITravel_Rule_BetaClient, Travel_Rule_BetaClient>();
    }

    private object[] MakeMemberData<TApiClientInterface, TApiClientImplementation>() where TApiClientInterface : IFireblocksApiClientBase
    {
        return
        [
            (FireblocksApiCredentialsConfiguration newApiCredentials) => Factory.CreateApiClient<TApiClientInterface>(newApiCredentials),
            typeof(TApiClientInterface),
            typeof(TApiClientImplementation)
        ];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}