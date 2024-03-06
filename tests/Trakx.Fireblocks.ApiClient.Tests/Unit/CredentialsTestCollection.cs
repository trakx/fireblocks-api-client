using System.Security.Cryptography;
using NSubstitute;
using Serilog;
using Trakx.Common.DateAndTime;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit;

[Collection(nameof(CredentialsTestCollection))]
public class CredentialsTestsBase
{
    protected ILogger Logger;
    protected string RsaPrivatePcs8PrivateKey { get; }
    protected RSA RsaKey { get; }
    protected FireblocksApiConfiguration Configuration { get; }

    protected CredentialsTestsBase(RsaKeyFixture fixture, ITestOutputHelper output)
    {
        Logger = new LoggerConfiguration().WriteTo.TestOutput(output).CreateLogger();
        RsaKey = fixture.RsaKey;
        RsaPrivatePcs8PrivateKey = Convert.ToBase64String(RsaKey.ExportPkcs8PrivateKey());
        Configuration = new FireblocksApiConfiguration
        {
            BaseUrl =  new Uri("https://api.fireblocks.io/v1"),
            ApiPrivateKey = RsaPrivatePcs8PrivateKey,
            ApiPubKey = "pubKey"
        };
    }
}

[CollectionDefinition(nameof(CredentialsTestCollection))]
public class CredentialsTestCollection : ICollectionFixture<RsaKeyFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public sealed class RsaKeyFixture : IDisposable
{
    public RSA RsaKey { get; }
    public IDateTimeProvider DateTimeProvider { get; } = Substitute.For<IDateTimeProvider>();

    public RsaKeyFixture()
    {
        RsaKey = RSA.Create(4096);
    }

    public void Dispose()
    {
        RsaKey.Dispose();
    }
}