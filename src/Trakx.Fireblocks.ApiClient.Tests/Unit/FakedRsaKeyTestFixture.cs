using System;
using System.Security.Cryptography;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit
{
    [Collection(nameof(FakeConfigurationTestCollection))]
    public class FakeConfigurationTestsBase
    {
        protected ILogger Logger;
        protected string RsaPrivatePcs8PrivateKey { get; }
        public RSA RsaKey { get; }
        protected FireblocksApiConfiguration Configuration { get; }

        protected FakeConfigurationTestsBase(FakedRsaKeyFixture fixture, ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration().WriteTo.TestOutput(output).CreateLogger();
            RsaKey = fixture.RsaKey;
            RsaPrivatePcs8PrivateKey = Convert.ToBase64String(RsaKey.ExportPkcs8PrivateKey());
            Configuration = new FireblocksApiConfiguration
            {
                ApiPrivateKey = RsaPrivatePcs8PrivateKey, 
                ApiPubKey = "pubKey"
            };
        }
    }

    [CollectionDefinition(nameof(FakeConfigurationTestCollection))]
    public class FakeConfigurationTestCollection : ICollectionFixture<FakedRsaKeyFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class FakedRsaKeyFixture : IDisposable
    {
        public RSA RsaKey { get; }

        public FakedRsaKeyFixture()
        {
            RsaKey = RSA.Create(4096);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            RsaKey.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
