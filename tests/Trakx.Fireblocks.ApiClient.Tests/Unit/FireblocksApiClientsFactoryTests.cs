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
        System.Type expectedInterfaceType,
        System.Type expectedImplementationType)
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
        yield return MakeMemberData<ITransactionsClient, TransactionsClient>();
        yield return MakeMemberData<IUsersClient, UsersClient>();
        yield return MakeMemberData<IVaultsClient, VaultsClient>();
        yield return MakeMemberData<IWebhooksClient, WebhooksClient>();
        yield return MakeMemberData<IWeb3_connectionsClient, Web3_connectionsClient>();
        yield return MakeMemberData<IPayments__PayoutClient, Payments__PayoutClient>();
        yield return MakeMemberData<IAssetsClient, AssetsClient>();
        yield return MakeMemberData<IWorkspaceClient, WorkspaceClient>();
        yield return MakeMemberData<IBlockchains_and_assetsClient, Blockchains_and_assetsClient>();
        yield return MakeMemberData<IUser_groups__BetaClient, User_groups__BetaClient>();
        yield return MakeMemberData<IStaking__BetaClient, Staking__BetaClient>();
        yield return MakeMemberData<IAdmin_QuorumClient, Admin_QuorumClient>();
        yield return MakeMemberData<INFTsClient, NFTsClient>();
        yield return MakeMemberData<ITravel_Rule__BetaClient, Travel_Rule__BetaClient>();
        yield return MakeMemberData<IComplianceClient, ComplianceClient>();
        yield return MakeMemberData<ICompliance_Screening_ConfigurationClient, Compliance_Screening_ConfigurationClient>();
        yield return MakeMemberData<IOTA__BetaClient, OTA__BetaClient>();
        yield return MakeMemberData<IWorkspace_Status__BetaClient, Workspace_Status__BetaClient>();
        yield return MakeMemberData<IPolicy_Editor__BetaClient, Policy_Editor__BetaClient>();
        yield return MakeMemberData<IConsole_UserClient, Console_UserClient>();
        yield return MakeMemberData<IApi_UserClient, Api_UserClient>();
        yield return MakeMemberData<IReset_deviceClient, Reset_deviceClient>();
        yield return MakeMemberData<IWhitelist_ip_addressesClient, Whitelist_ip_addressesClient>();
        yield return MakeMemberData<ISmart_TransferClient, Smart_TransferClient>();
        yield return MakeMemberData<IJobsClient, JobsClient>();
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