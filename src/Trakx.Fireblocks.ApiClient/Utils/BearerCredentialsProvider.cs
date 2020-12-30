using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Trakx.Utils.Api;

namespace Trakx.Fireblocks.ApiClient.Utils
{
    public class BearerCredentialsProvider : IBearerCredentialsProvider
    {
        private readonly FireblocksApiConfiguration _fireblocksConfiguration;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly SigningCredentials _signingCredentials;


        public BearerCredentialsProvider(IOptions<FireblocksApiConfiguration> configuration, IDateTimeProvider dateTimeProvider)
        {
            _fireblocksConfiguration = configuration.Value;
            _dateTimeProvider = dateTimeProvider;
            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(_fireblocksConfiguration.ApiPrivateKey), out _);
            var securityKey = new RsaSecurityKey(rsa);
            _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        }


        private IEnumerable<Claim> GetClaims(HttpRequestMessage msg)
        {
            var nonce = GetNonce();
            var issuedTimestamp = GetIssuedTimestamp();
            var issuedTimestampString = issuedTimestamp.ToString(CultureInfo.InvariantCulture);
            var expirationTimestamp = (issuedTimestamp + 20).ToString(CultureInfo.InvariantCulture);
            var body = msg.Content?.ReadAsStringAsync().GetAwaiter().GetResult() ?? string.Empty;
            var hashBody = GetSignature(body);
            return new List<Claim>
            {
                new Claim("uri", $"{msg.RequestUri!.AbsolutePath}"),
                new Claim("nonce", $"{nonce}"),
                new Claim("iat", $"{issuedTimestampString}"),
                new Claim("exp", $"{expirationTimestamp}"),
                new Claim("sub", $"{_fireblocksConfiguration.ApiPubKey}"),
                new Claim("bodyHash", $"{hashBody}")
            };
        }


        public string GenerateJwtToken(HttpRequestMessage msg)
        {
            var claims = GetClaims(msg);
            var token = new JwtSecurityToken(new JwtHeader(_signingCredentials), new JwtPayload(claims));

            var sTokenHandler = new JwtSecurityTokenHandler();
            return sTokenHandler.WriteToken(token);
        }

        private string GetNonce() => _dateTimeProvider.UtcNowAsOffset.ToUnixTimeMilliseconds()
            .ToString(CultureInfo.InvariantCulture);

        private long GetIssuedTimestamp() => _dateTimeProvider.UtcNowAsOffset.ToUnixTimeSeconds();

        private static string GetSignature(string preHash) => new HMACSHA256().ComputeHash(Encoding.UTF8.GetBytes(preHash)).ToHexString();
    }
}
