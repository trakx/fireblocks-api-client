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


        private JwtPayload GetPayload(HttpRequestMessage msg)
        {
            var nonce = GetNonce();
            var issuedTimestamp = GetIssuedTimestamp();
            var expirationTimestamp = issuedTimestamp + 20;
            var body = msg.Content?.ReadAsStringAsync().GetAwaiter().GetResult() ?? string.Empty;
            var hashBody = GetSignature(body);
            return new JwtPayload()
            {
                {"uri",msg.RequestUri!.AbsolutePath },
                {"nonce", nonce},
                { "iat", issuedTimestamp},
                { "exp", expirationTimestamp},
                { "sub", _fireblocksConfiguration.ApiPubKey},
                { "bodyHash", hashBody}
            };
        }


        public string GenerateJwtToken(HttpRequestMessage msg)
        {
            var payload = GetPayload(msg);
            var token = new JwtSecurityToken(new JwtHeader(_signingCredentials), payload);

            var sTokenHandler = new JwtSecurityTokenHandler();
            return sTokenHandler.WriteToken(token);
        }

        private long GetNonce() => _dateTimeProvider.UtcNowAsOffset.ToUnixTimeMilliseconds();

        private long GetIssuedTimestamp() => _dateTimeProvider.UtcNowAsOffset.ToUnixTimeSeconds();

        private static string GetSignature(string preHash) => new HMACSHA256().ComputeHash(Encoding.UTF8.GetBytes(preHash)).ToHexString();
    }
}
