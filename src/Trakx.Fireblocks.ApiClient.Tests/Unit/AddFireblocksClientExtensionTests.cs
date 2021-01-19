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
        private readonly FireblocksApiConfiguration _fakeConfiguration;

        public AddFireblocksClientExtensionTests()
        {
            _fakeConfiguration = new FireblocksApiConfiguration
            {
                ApiPrivateKey = "MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBALXu9makTMIVzt5TeI9wfQ1mID9a6RxVoi/OfRu8Km5SM2/zq2W2h9jwa13a2lU4F1UmXLeFVKwpO+AKE/to5V5grg6VOcfSpJrccMU9i0rgN1DaUFIGR5YYrFqDdbxbZ+GmV64UpE+QIEDKTYTBpQEuXeaSDH7jpU1ZD+5B8QffAgMBAAECgYBF50w7gfOQO0rh99aB3qnd5RiM30RkQ67uiXtelb0D6IelA04e5bRqsW2/gSCUz9l/OR800IEJKUq9OYIV/mbqZ4jr9jCrqTZY1ZIAwETpJpnNYkic383WPrUH6G3RdbkFcAHs1TtRqhb491rcbKaH/PKpwfXC6JJXx1c0QhaiAQJBANjC9M6PMgAZQCKM/IAGBX4JyyIKbsUWfeqKjy2pmzrZGW0w7wGiKup1PXD1leqKLABbNbtUSrlw2MDoKjjVMt8CQQDW3gWLNPOOy6A5dZj0mwXZN3R/P+24yDJ0bgk2nBLwhtxiBtiKGyyDA7RC4z7NzZJhszPlx+bqzgFPhtsr28sBAkEA1BF43RYvvvT1UFhLOR2rBfKi8DzwvpYQvwSsq+oB2wNH4xI644g7IINvhzJeZ8ai4f5ahVokeWU0SxdZJ50+VQJBANI7O9tfvOq7i5KM4FJ8l7VX0BFtgHAq2UsYGsWm4dUbbugu4JIIUn4CEs7bP6jn+dDEIQQzmlmlpJymSy3XiQECQQDWxU6lzRQJyv7IOOVvAQebbNUmVnvYIw0G7f0TDn5I55+nE+z8/5dyKKNPhw4OMBTBO12BJyo5/a3eLUpTsKhv",
                ApiPubKey = "pubKey"
            };
        }

        [Fact]
        public void Services_should_be_built()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddFireblocksClient(_fakeConfiguration);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _ = serviceProvider.GetRequiredService<IFireblocksClient>();
        }
    }
}
