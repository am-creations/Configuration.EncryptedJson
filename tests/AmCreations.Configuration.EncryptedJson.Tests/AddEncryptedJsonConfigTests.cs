using System;
using System.IO;
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
            DecryptsValuesOnTheFly(
                (builder, certLoader) => builder.AddEncryptedJsonFile(config =>
            {
                config.Path = "appsettings.Encrypted.json";
                config.CertificateLoader = certLoader;
            }));
            
            DecryptsValuesOnTheFly(
                (builder, certLoader) => builder.AddEncryptedJsonFile("appsettings.Encrypted.json", certLoader));
            
            DecryptsValuesOnTheFly(
                (builder, certLoader) => builder.AddEncryptedJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Encrypted.json"), certLoader));
        }
        
        [Fact]
        public void AddEncryptedJsonConfig_OptionalShouldWork()
        {
            Build((builder, certLoader) => builder.AddEncryptedJsonFile("appsettings.Encrypted.DoesNotExist.json", optional:true, certLoader));
            
            Assert.Throws<FileNotFoundException>(() => Build((builder, certLoader) => builder.AddEncryptedJsonFile("appsettings.Encrypted.DoesNotExist.json", optional:false, certLoader)));
        }
        
        private void DecryptsValuesOnTheFly(Action<IConfigurationBuilder, ICertificateLoader> builder)
        {
            var configuration = Build(builder);

            var decryptedValue = configuration["SomeKey:SomeSubKey"];

            Assert.Equal("This data is protected !", decryptedValue);
        }
        
        private IConfigurationRoot Build(Action<IConfigurationBuilder, ICertificateLoader> builder)
        {
            var certificateLoader = Mocks.GetCertificateLoader().Object;
            
            var configBuilder = new ConfigurationBuilder();
            
            builder.Invoke(configBuilder, certificateLoader);
            
            return configBuilder.Build();
        }
    }
}
