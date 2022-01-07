using System;
using System.IO;
using System.Text.Json;
using AmCreations.Configuration.EncryptedJson.Crypters;
using Microsoft.Extensions.Configuration;

namespace AmCreations.Configuration.EncryptedJson
{
    /// <summary>
    /// An Encrypted JSON file based on <see cref="FileConfigurationProvider"/>.
    /// </summary>
    public class EncryptedJsonConfigurationProvider : FileConfigurationProvider
    {
        /// <summary>
        /// Initializes a new instance with the specified source.
        /// </summary>
        /// <param name="source">The source settings.</param>
        public EncryptedJsonConfigurationProvider(EncryptedJsonConfigurationSource source) : base(source) { }
        
        /// <summary>
        /// Loads JSON configuration key/values from a stream into a provider.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        public override void Load(Stream stream)
        {
            var source = (EncryptedJsonConfigurationSource)Source;
            
            try
            {
                var crypter = new RsaCrypter(source.CertificateLoader);
                Data = EncryptedJsonConfigurationFileParser.Parse(stream, crypter);
            }
            catch (JsonException e)
            {
                throw new FormatException("Could not parse the encrypted JSON file", e);
            }
        }
    }
}
