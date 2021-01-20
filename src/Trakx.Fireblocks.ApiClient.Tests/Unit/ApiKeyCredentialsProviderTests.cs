using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Trakx.Fireblocks.ApiClient.Utils;
using Trakx.Utils.DateTimeHelpers;
using Xunit;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit
{
    public class ApiKeyCredentialsProviderTests
    {
        private readonly ApiKeyCredentialsProvider _provider;
        private readonly FireblocksApiConfiguration _configuration;
        private readonly DateTimeOffset nonce;

        public ApiKeyCredentialsProviderTests()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            nonce = DateTimeOffset.FromUnixTimeMilliseconds(12345678789);
            dateTimeProvider.UtcNowAsOffset.ReturnsForAnyArgs(nonce);
            _configuration = new FireblocksApiConfiguration
            {
                ApiPrivateKey = "MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCVicoa9GJwj4wQZgwuDvFoomBp+fOYG+w5qPO3VfnOadu34N0jIlcOOYuK3v/3TySg4yUiBJ07W6mq8A4Wt8fRwOcBy9DlIA3nFqctLw00zykNKv5gEzUPN5yIhG5qXnU33HOK1zPCvKF3wF5Mxz9z5rsv6v+ZqZRkTG4Vk7YrU8M2NxsEdx+bD5CcH49ME12hjiE3cL8xCOQq+EIJ6pV+154bMTXbwTihZABW0VobLLr5syimrViUHdBG2Syrcd7M2JiucnfVm1YDSHPFq0kUzj9I4Ja+2ikLevFcSbdVNW6UyI0bvHR4WOFBBbpD1wP0YxrZlxjs+tIuZqMC00l5AgMBAAECggEANURk8t8U5cWsffbr5cg65CvbLkT0Kb3gktWFI0yD9KwusTxvaWGFIzMKawhuo3jUsl7XTlvYalmHgbWLPzpt+mC9qR0iJ0wYx7WBDY5SJPqaQ++Jt02APBByOQr7jfZhyOlFWzK3ZGnvFoo2zSC4kJ+jFDmgnOsUSfoTyKnaXWoomIhDBy+2RXT1QKccxICZnYWt+jAdOELlg9jv8zYkSZwr9UicuLrVVed/lxYVM/Vuo9xwDc2ZSKP6dUl+T/gZeo3O94VrdzV2rmItEaN5xaWZCPtvLcs2amvNlOI/97CuzoqY6of3d/ilMARJha1tDl6Kp2D8v8HWyOc/SBQnNQKBgQDHGEVeEDpDyC739E9UCaNHxpBt4x6MOEm+VryO/QJ8crBvHYat33HU4wFEXb73znE+EeN5rXdFXGsbFbd+wHquLwAxDi9TZWQl9/uMmnYJ0kXk/DmxgIItxX4ghaitxUrvb9f98DcxcEZLiI3WeuerbN/3XW8xaaFmCbrwZ9CKzwKBgQDAR3lQndcDT20P8oU4RSGlCVRyVe6c5gmZGEs6uv1xbLxvT+eLsDYZ97p6dLo28J9XvRvooTlrMdmwzXk8YeAzS8uDm4VPnYcTZxveimW76JBfUMJO/yG1cj5kYK5icRsD+Or+l/P+93Js64VDJ/dl0oNFG4GHEgVRk1mBgeTZNwKBgH49aoaZPYiLHxUD6WT883Zodl3m/BCnBENXVhpIXVIxmqPqEeKf4tq80ana02LnmLBlwVn+3rixQAydeYs6Ws+lZ7azKtjzWkwk8HCX5C6YEM+ACsj9Fg4VwsK2XV1J0xj9TvRqY2TKrxfnzx/nMIxm7aA4s2+ebcN4eftDqFzLAoGBALh7GwNlHWmYqBxX5MezVG+C/xngJts0vtq7e1pzFD3iRP1Lne3heFb731gkbJHP1iIZWBCft3aI0sOWsQ5lHfY5b57q9UII8xODpzKieK+MmBBLbNLcaNwtK32lYUskF/Z8ugLhigIEKi4k3qwrSWvRSY9hr6oHeUDmbkcaeJaLAoGAAhGsxg0jEayuP5PNXUDyGPcBbidfCOSMTLT06N/sDjwubBU+gNbgkR/X5aORXPqp/vbmoABcW49mVaMmEWSQBNX93O29WCadelth3Cchs6AeZ2ciSIPQk9JGmsLNdZISjQua6cAN0uDgAsqUV8DgnXxKOGvFyCUg55+HkGiCVVI=",
                ApiPubKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAlYnKGvRicI+MEGYMLg7xaKJgafnzmBvsOajzt1X5zmnbt+DdIyJXDjmLit7/908koOMlIgSdO1upqvAOFrfH0cDnAcvQ5SAN5xanLS8NNM8pDSr+YBM1DzeciIRual51N9xzitczwryhd8BeTMc/c+a7L+r/mamUZExuFZO2K1PDNjcbBHcfmw+QnB+PTBNdoY4hN3C/MQjkKvhCCeqVfteeGzE128E4oWQAVtFaGyy6+bMopq1YlB3QRtksq3HezNiYrnJ31ZtWA0hzxatJFM4/SOCWvtopC3rxXEm3VTVulMiNG7x0eFjhQQW6Q9cD9GMa2ZcY7PrSLmajAtNJeQIDAQAB"
            };
            var options = Substitute.For<IOptions<FireblocksApiConfiguration>>();
            options.Value.ReturnsForAnyArgs(_configuration);
            var bearerCredentials = new BearerCredentialsProvider(options, dateTimeProvider);
            _provider = new ApiKeyCredentialsProvider(options, bearerCredentials);
        }

        [Fact]
        public void Request_header_should_contains_the_api_pub_key_and_JWT_bearer()
        {
            var message = new HttpRequestMessage { RequestUri = new Uri("https://test.com/test1/validate") };
            _provider.AddCredentials(message);

            message.Headers.GetValues(ApiKeyCredentialsProvider.ApiKeyHeader).Single()
                .Should().Be(_configuration.ApiPubKey);
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
            payload.Iat.Should().Be(Convert.ToInt32(nonce.ToUnixTimeSeconds()));
            payload.Exp.Should().Be(Convert.ToInt32(nonce.ToUnixTimeSeconds()) + 20);
            payload.Sub.Should().Be(_configuration.ApiPubKey);
            payload.TryGetValue("nonce", out var retrievedNonce);
            retrievedNonce.Should().Be(nonce.ToUnixTimeMilliseconds());
        }

        private TokenValidationParameters GetValidationParameters()
        {
            var rsa = RSA.Create();
            
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(_configuration.ApiPrivateKey), out _);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateLifetime = false,
                ValidateIssuer = false,
                ValidateAudience = false
            };
            return validationParameters;
        }
    }
}
