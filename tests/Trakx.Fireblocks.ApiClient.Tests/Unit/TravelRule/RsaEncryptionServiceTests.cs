using System.Security.Cryptography;
using System.Text;
using Trakx.Fireblocks.ApiClient.TravelRule;

namespace Trakx.Fireblocks.ApiClient.Tests.Unit.TravelRule;

public class RsaEncryptionServiceTests : IDisposable
{
    private readonly RsaEncryptionService _sut;
    private readonly RSA _rsa;
    private readonly string _publicKeyPem;

    public RsaEncryptionServiceTests()
    {
        _sut = new RsaEncryptionService();
        _rsa = RSA.Create(2048);
        _publicKeyPem = _rsa.ExportSubjectPublicKeyInfoPem();
    }

    [Fact]
    public void Encrypt_string_should_produce_base64_that_can_be_decrypted()
    {
        var plainText = "Hello World";

        var encrypted = _sut.Encrypt(plainText, _publicKeyPem);

        encrypted.Should().NotBeNullOrEmpty();
        var encryptedBytes = Convert.FromBase64String(encrypted);
        var decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        var decrypted = Encoding.UTF8.GetString(decryptedBytes);
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Encrypt_bool_true_should_encrypt_as_raw_byte()
    {
        var encrypted = _sut.Encrypt(true, _publicKeyPem);

        encrypted.Should().NotBeNullOrEmpty();
        var encryptedBytes = Convert.FromBase64String(encrypted);
        var decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        BitConverter.ToBoolean(decryptedBytes).Should().BeTrue();
    }

    [Fact]
    public void Encrypt_bool_false_should_encrypt_as_raw_byte()
    {
        var encrypted = _sut.Encrypt(false, _publicKeyPem);

        encrypted.Should().NotBeNullOrEmpty();
        var encryptedBytes = Convert.FromBase64String(encrypted);
        var decryptedBytes = _rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        BitConverter.ToBoolean(decryptedBytes).Should().BeFalse();
    }

    [Fact]
    public void Encrypt_should_throw_when_plainText_is_null_or_empty()
    {
        var action = () => _sut.Encrypt(string.Empty, _publicKeyPem);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Encrypt_should_throw_when_publicKeyPem_is_null_or_empty()
    {
        var action = () => _sut.Encrypt("test", string.Empty);
        action.Should().Throw<ArgumentException>();
    }

    public void Dispose()
    {
        _rsa.Dispose();
    }
}
