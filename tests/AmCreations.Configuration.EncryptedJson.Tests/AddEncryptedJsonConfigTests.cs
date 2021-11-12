using System;
using AmCreations.Configuration.EncryptedJson.CertificateLoaders;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AmCreations.Configuration.EncryptedJson.Tests
{
    public class AddEncryptedJsonConfigTests
    {
        [Fact]
        public void AddEncryptedJsonConfig_DecryptsValuesOnTheFly()
        {
            AddEncryptedJsonFile_DecryptsValuesOnTheFly(
                (builder, certLoader) => builder.AddEncryptedJsonFile(config =>
            {
                config.Path = "appsettings.Encrypted.json";
                config.CertificateLoader = certLoader;
            }));
            
            AddEncryptedJsonFile_DecryptsValuesOnTheFly(
                (builder, certLoader) => builder.AddEncryptedJsonFile("appsettings.Encrypted.json", certLoader));
        }
        
        private void AddEncryptedJsonFile_DecryptsValuesOnTheFly(Action<IConfigurationBuilder, ICertificateLoader> builder)
        {
            var certificateLoader = Mocks.GetCertificateLoader().Object;
            
            var configBuilder = new ConfigurationBuilder();
            
            builder.Invoke(configBuilder, certificateLoader);
            
            var configuration = configBuilder.Build();

            var decryptedValue = configuration["SomeKey:SomeSubKey"];

            Assert.Equal("This data is protected !", decryptedValue);
        }
    }
}
