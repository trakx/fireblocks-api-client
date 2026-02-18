using System.Security.Cryptography;
using System.Text;

namespace Trakx.Fireblocks.ApiClient.TravelRule;

/// <inheritdoc />
internal class RsaEncryptionService : IRsaEncryptionService
{
    /// <inheritdoc />
    public string Encrypt(string plainText, string publicKeyPem)
    {
        ArgumentException.ThrowIfNullOrEmpty(plainText);
        ArgumentException.ThrowIfNullOrEmpty(publicKeyPem);

        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);

        return Convert.ToBase64String(encryptedBytes);
    }

    /// <inheritdoc />
    public string Encrypt(bool plainValue, string publicKeyPem)
    {
        ArgumentException.ThrowIfNullOrEmpty(publicKeyPem);

        var boolBytes = BitConverter.GetBytes(plainValue);

        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);

        var encryptedBytes = rsa.Encrypt(boolBytes, RSAEncryptionPadding.OaepSHA256);

        return Convert.ToBase64String(encryptedBytes);
    }
}
