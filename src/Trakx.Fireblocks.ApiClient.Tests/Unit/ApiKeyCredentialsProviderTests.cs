using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Trakx.Fireblocks.ApiClient.Utils;
using Trakx.Utils.DateTimeHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit
{
    public class ApiKeyCredentialsProviderTests : FakeConfigurationTestsBase
    {
        private readonly ApiKeyCredentialsProvider _provider;
        private readonly DateTimeOffset _nonce;

        public ApiKeyCredentialsProviderTests(FakedRsaKeyFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            _nonce = DateTimeOffset.FromUnixTimeMilliseconds(12345678789);
            dateTimeProvider.UtcNowAsOffset.ReturnsForAnyArgs(_nonce);

            
            var options = Substitute.For<IOptions<FireblocksApiConfiguration>>();
            options.Value.ReturnsForAnyArgs(Configuration);
            var bearerCredentials = new BearerCredentialsProvider(options, dateTimeProvider);
            _provider = new ApiKeyCredentialsProvider(options, bearerCredentials);
        }

        [Fact]
        public void Request_header_should_contains_the_api_pub_key_and_JWT_bearer()
        {
            var message = new HttpRequestMessage { RequestUri = new Uri("https://test.com/test1/validate") };
            _provider.AddCredentials(message);

            message.Headers.GetValues(ApiKeyCredentialsProvider.ApiKeyHeader).Single()
                .Should().Be(Configuration.ApiPubKey);
            message.Headers.Authorization?.Scheme.Should().Be(ApiKeyCredentialsProvider.JwtScheme);
            message.Headers.Authorization?.Parameter.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Bearer_token_should_have_correct_payload()
        {
            var message = new HttpRequestMessage { RequestUri = new Uri("https://test.com/test1/validate") };
            _provider.AddCredentials(message);

            var retrievedRawJwt = message.Headers.Authorization?.Parameter;
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(retrievedRawJwt, GetValidationParameters(), out _);
            var token = handler.ReadJwtToken(retrievedRawJwt);
            ValidatePayload(token);
        }

        [Fact]
        public void Bearer_signature_should_depends_on_message_content()
        {
            var messageOne = new HttpRequestMessage { RequestUri = new Uri("https://test.com/test1/validate"), Content = new StringContent("body one")};
            _provider.AddCredentials(messageOne);
            var retrievedRawJwtOne = messageOne.Headers.Authorization?.Parameter;

            var messageTwo = new HttpRequestMessage { RequestUri = new Uri("https://test.com/test1/validate"), Content = new StringContent("body two") };
            _provider.AddCredentials(messageTwo);
            var retrievedRawJwtTwo = messageTwo.Headers.Authorization?.Parameter;

            retrievedRawJwtTwo.Should().NotBeEquivalentTo(retrievedRawJwtOne);
        }
        
        private void ValidatePayload(JwtSecurityToken retrievedToken)
        {
            var payload = retrievedToken.Payload;
            payload.Iat.Should().Be(Convert.ToInt32(_nonce.ToUnixTimeSeconds()));
            payload.Exp.Should().Be(Convert.ToInt32(_nonce.ToUnixTimeSeconds()) + 20);
            payload.Sub.Should().Be(Configuration.ApiPubKey);
            payload.TryGetValue("nonce", out var retrievedNonce);
            retrievedNonce.Should().Be(_nonce.ToUnixTimeMilliseconds());
        }

        private TokenValidationParameters GetValidationParameters()
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(RsaKey),
                ValidateLifetime = false,
                ValidateIssuer = false,
                ValidateAudience = false
            };
            return validationParameters;
        }
    }
}
