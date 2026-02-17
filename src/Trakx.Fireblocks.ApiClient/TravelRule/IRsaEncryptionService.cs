namespace Trakx.Fireblocks.ApiClient.TravelRule;

/// <summary>
/// Service for encrypting PII data using RSA-OAEP with SHA-256.
/// </summary>
public interface IRsaEncryptionService
{
    /// <summary>
    /// Encrypts a string value using RSA-OAEP with SHA-256.
    /// </summary>
    /// <param name="plainText">The plain text value to encrypt.</param>
    /// <param name="publicKeyPem">The RSA public key in PEM format.</param>
    /// <returns>Base64-encoded encrypted value.</returns>
    string Encrypt(string plainText, string publicKeyPem);

    /// <summary>
    /// Encrypts a boolean value using RSA-OAEP with SHA-256.
    /// The boolean is converted to lowercase JSON representation ("true" or "false") before encryption.
    /// </summary>
    /// <param name="plainValue">The boolean value to encrypt.</param>
    /// <param name="publicKeyPem">The RSA public key in PEM format.</param>
    /// <returns>Base64-encoded encrypted value.</returns>
    string Encrypt(bool plainValue, string publicKeyPem);
}
