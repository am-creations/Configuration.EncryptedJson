using AmCreations.Configuration.EncryptedJson.CertificateLoaders;
using Microsoft.Extensions.Configuration;

namespace AmCreations.Configuration.EncryptedJson
{
    /// <summary>
    /// Represents a JSON file as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class EncryptedJsonConfigurationSource : FileConfigurationSource
    {
        /// <summary>
        /// A certificate loader instance. Custom loaders can be used.
        /// </summary>
        public ICertificateLoader CertificateLoader { get; set; }
        
        /// <summary>
        /// Builds the <see cref="EncryptedJsonConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="EncryptedJsonConfigurationProvider"/></returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new EncryptedJsonConfigurationProvider(this);
        }
    }
}