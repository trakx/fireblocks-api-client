﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Trakx.Common.DateAndTime;
using Trakx.Common.Extensions;
using Trakx.Fireblocks.ApiClient.Utils;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit;

public sealed class BearerCredentialProviderTests : CredentialsTestsBase, IDisposable
{
    private readonly DateTimeOffset _nonce;
    private readonly BearerCredentialsProvider _bearerCredentialsProvider;

    public BearerCredentialProviderTests(RsaKeyFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _nonce = DateTimeOffset.FromUnixTimeSeconds(12345678);
        dateTimeProvider.UtcNowAsOffset.ReturnsForAnyArgs(_nonce);

        _bearerCredentialsProvider = new BearerCredentialsProvider(Configuration, dateTimeProvider);
    }


    [Fact]
    public void Bearer_token_should_have_correct_payload()
    {
        var message = new HttpRequestMessage { RequestUri = new Uri("https://test.com/test1/validate") };
        var messageBody = "this body is taken into account in the signature and payload";
        message.Content = new StringContent(messageBody, Encoding.UTF8);
        var jwt = _bearerCredentialsProvider.GenerateJwtToken(message);

        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(jwt, GetValidationParameters(), out _);
        var token = handler.ReadJwtToken(jwt);
        ValidatePayload(token, messageBody);
    }


    private void ValidatePayload(JwtSecurityToken retrievedToken, string messageBody)
    {
        var payload = retrievedToken.Payload;
        payload.IssuedAt.Should().Be(_nonce.UtcDateTime);
        payload.Expiration.Should().Be(Convert.ToInt32(_nonce.ToUnixTimeSeconds()) + 20);

        payload.Sub.Should().Be(Configuration.ApiPubKey);
        payload.TryGetValue("nonce", out var retrievedNonce).Should().BeTrue();
        retrievedNonce.Should().Be(_nonce.ToUnixTimeMilliseconds());
        payload.TryGetValue("bodyHash", out var retrievedBodyHash).Should().BeTrue();
        using var sha256 = SHA256.Create();
        retrievedBodyHash.Should()
            .Be(sha256.ComputeHash(Encoding.UTF8.GetBytes(messageBody)).ToHexString());
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

    public void Dispose()
    {
        _bearerCredentialsProvider.Dispose();
    }
}