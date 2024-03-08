using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Trakx.Common.DateAndTime;
using Trakx.Common.Extensions;

namespace Trakx.Fireblocks.ApiClient.Utils;

/// <inheritdoc cref="IBearerCredentialsProvider" />
public sealed class BearerCredentialsProvider : IBearerCredentialsProvider, IDisposable
{
    internal readonly FireblocksApiCredentialsConfiguration ApiCredentialsConfiguration;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly SigningCredentials _signingCredentials;
    private readonly RSA _rsa;

    /// <summary>
    /// Constructor to use the default <see cref="FireblocksApiCredentialsConfiguration"/>.
    /// </summary>
    public BearerCredentialsProvider(FireblocksApiConfiguration configuration, IDateTimeProvider dateTimeProvider)
        : this((FireblocksApiCredentialsConfiguration)configuration, dateTimeProvider) { }

    /// <summary>
    /// Constructor to use a custom <see cref="FireblocksApiCredentialsConfiguration"/>.
    /// </summary>
    public BearerCredentialsProvider(FireblocksApiCredentialsConfiguration apiCredentialsConfiguration, IDateTimeProvider dateTimeProvider)
    {
        ApiCredentialsConfiguration = apiCredentialsConfiguration;
        _dateTimeProvider = dateTimeProvider;
        _rsa = RSA.Create();
        _rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(ApiCredentialsConfiguration.ApiPrivateKey), out _);
        var securityKey = new RsaSecurityKey(_rsa);
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
    }

    private JwtPayload GetPayload(HttpRequestMessage msg)
    {
        var nonce = GetNonce();
        var issuedTimestamp = GetIssuedTimestamp();
        var expirationTimestamp = issuedTimestamp + 20;
        var body = msg.Content?.ReadAsStringAsync().GetAwaiter().GetResult() ?? string.Empty;
        var hashBody = GetSignature(body);
        return new JwtPayload
        {
            {"uri", msg.RequestUri!.PathAndQuery},
            {"nonce", nonce},
            {"iat", issuedTimestamp},
            {"exp", expirationTimestamp},
            {"sub", ApiCredentialsConfiguration.ApiPubKey},
            {"bodyHash", hashBody}
        };
    }

    /// <inheritdoc />
    public string GenerateJwtToken(HttpRequestMessage msg)
    {
        var payload = GetPayload(msg);
        var token = new JwtSecurityToken(new JwtHeader(_signingCredentials), payload);

        var sTokenHandler = new JwtSecurityTokenHandler();
        return sTokenHandler.WriteToken(token);
    }

    private long GetNonce() => _dateTimeProvider.UtcNowAsOffset.ToUnixTimeMilliseconds();

    private long GetIssuedTimestamp() => _dateTimeProvider.UtcNowAsOffset.ToUnixTimeSeconds();

    private string GetSignature(string preHash)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(preHash)).ToHexString();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _rsa.Dispose();
    }
}