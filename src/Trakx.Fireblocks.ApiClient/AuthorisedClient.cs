using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Trakx.Utils.Api;

namespace Trakx.Fireblocks.ApiClient
{
    internal abstract class AuthorisedClient
    {
        protected readonly ICredentialsProvider CredentialProvider;

        protected AuthorisedClient(ClientConfigurator clientConfigurator)
        {
            CredentialProvider = clientConfigurator.CredentialsProvider;
        }
    }
}