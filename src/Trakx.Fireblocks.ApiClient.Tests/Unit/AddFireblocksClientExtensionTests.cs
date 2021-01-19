using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit
{
    public class AddFireblocksClientExtensionTests
    {
        private readonly FireblocksApiConfiguration _configuration;

        public AddFireblocksClientExtensionTests()
        {
            _configuration = new FireblocksApiConfiguration
            {
                ApiPrivateKey = "privKey",
                ApiPubKey = "pubKey"
            };
        }

        [Fact]
        public void Services_should_be_built()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddFireblocksClient(_configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _ = serviceProvider.GetRequiredService<IFireblocksClient>();
        }
    }
}
