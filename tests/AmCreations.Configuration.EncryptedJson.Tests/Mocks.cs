using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using AmCreations.Configuration.EncryptedJson.CertificateLoaders;
using AmCreations.Configuration.EncryptedJson.Crypters;
using Moq;

namespace AmCreations.Configuration.EncryptedJson.Tests
{
    public static class Mocks
    {
        public static Mock<ICertificateLoader> GetCertificateLoader()
        {
            var certLoaderMock = new Mock<ICertificateLoader>();
            certLoaderMock.Setup(loader => loader.LoadCertificate()).Returns(() =>
            {
                using var certStream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("AmCreations.Configuration.EncryptedJson.Tests.test-certificate.pfx");
                using var ms = new MemoryStream();
                certStream!.CopyTo(ms);

                return new X509Certificate2(ms.ToArray());
            });

            return certLoaderMock;
        }

        public static Mock<ICrypter> GetCrypter()
        {
            var crypterMock = new Mock<ICrypter>();
            crypterMock.Setup(crypter => crypter.EncryptString(It.IsAny<string>()))
                .Returns<string>(input => $"{input}_encrypted");
            crypterMock.Setup(crypter => crypter.DecryptString(It.IsAny<string>()))
                .Returns<string>(input =>
                {
                    var encryptedIndex = input.LastIndexOf("_encrypted", StringComparison.Ordinal);

                    return encryptedIndex > -1
                        ? input.Substring(0, encryptedIndex)
                        : input.Substring(0, input.Length);
                });

            return crypterMock;
        }
    }
}
