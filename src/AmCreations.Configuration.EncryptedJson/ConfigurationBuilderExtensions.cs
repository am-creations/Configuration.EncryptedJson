using System;
using AmCreations.Configuration.EncryptedJson.CertificateLoaders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace AmCreations.Configuration.EncryptedJson
{
    /// <summary>
    /// Extension methods for adding <see cref="EncryptedJsonConfigurationProvider"/>.
    /// </summary>
    public static class ConfigurationBuilderExtenions
    {
        /// <summary>
        /// Adds the encrypted JSON configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="certificateLoader">The certificate loader (either <see cref="FilesystemCertificateLoader"/> or
        /// <see cref="StoreCertificateLoader"/></param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEncryptedJsonFile(this IConfigurationBuilder builder, string path, ICertificateLoader certificateLoader)
        {
            return AddEncryptedJsonFile(builder, provider: null, path: path, optional: false, reloadOnChange: false, certificateLoader);
        }

        /// <summary>
        /// Adds the encrypted JSON configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="certificateLoader">The certificate loader (either <see cref="FilesystemCertificateLoader"/> or
        /// <see cref="StoreCertificateLoader"/></param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEncryptedJsonFile(this IConfigurationBuilder builder, string path, bool optional, ICertificateLoader certificateLoader)
        {
            return AddEncryptedJsonFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false, certificateLoader);
        }

        /// <summary>
        /// Adds an encrypted JSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="provider">The <see cref="IFileProvider"/> to use to access the file.</param>
        /// <param name="path">Path relative to the base path stored in
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <param name="optional">Whether the file is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
        /// <param name="certificateLoader">The certificate loader (either <see cref="FilesystemCertificateLoader"/> or
        /// <see cref="StoreCertificateLoader"/></param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEncryptedJsonFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange, ICertificateLoader certificateLoader)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("File path must be a non-empty string", nameof(path));
            }

            return builder.AddEncryptedJsonFile(s =>
            {
                s.FileProvider = provider;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.CertificateLoader = certificateLoader;
                
                s.ResolveFileProvider();
            });
        }

        /// <summary>
        /// Adds an encrypted JSON configuration source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddEncryptedJsonFile(this IConfigurationBuilder builder, Action<EncryptedJsonConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}
