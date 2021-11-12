using AmCreations.Configuration.EncryptedJson.Crypters;
using Xunit;

namespace AmCreations.Configuration.EncryptedJson.Tests
{
    public class RsaCrypterTests
    {
        [Fact]
        public void Constructor_CallsLoadCertificate()
        {
            var certificateLoaderMock = Mocks.GetCertificateLoader();

            var rsaCrypter = new RsaCrypter(certificateLoaderMock.Object);

            certificateLoaderMock.Verify(loader => loader.LoadCertificate());
        }
        
        [Theory]
        [InlineData("test")]
        [InlineData("")]
        [InlineData("Some other string;+/\\~")]
        [InlineData("With unicode 😉")]
        public void Crypter_Encrypts(string value)
        {
            var certificateLoaderMock = Mocks.GetCertificateLoader();

            var rsaCrypter = new RsaCrypter(certificateLoaderMock.Object);
            
            Assert.Equal(value, rsaCrypter.DecryptString(rsaCrypter.EncryptString(value)));
        }
    }
}
