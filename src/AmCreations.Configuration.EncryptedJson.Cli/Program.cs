using System;
using AmCreations.Configuration.EncryptedJson.CertificateLoaders;
using AmCreations.Configuration.EncryptedJson.Crypters;
using McMaster.Extensions.CommandLineUtils;

namespace AmCreations.Configuration.EncryptedJson.Cli
{
    class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "conf-encrypt",
                Description = "Configuration encryption utility",
            };
            
            app.HelpOption(inherited: true);
            
            app.Command("encrypt", encryptCmd =>
            {
                encryptCmd.Description = "Encrypts data using the certificates public key.";
                    
                var cert = encryptCmd.Argument("cert", "Certificate file (or public key)").IsRequired();
                var data = encryptCmd.Argument("content", "Content to encrypt").IsRequired();
                
                encryptCmd.OnExecute(() =>
                {
                    var loader = new FilesystemCertificateLoader(cert.Value);
                    var crypter = new RsaCrypter(loader);

                    Console.WriteLine(crypter.EncryptString(data.Value));
                });
            });
            
            app.Command("decrypt", decryptCmd =>
            {
                decryptCmd.Description = "Decrypts data using the certificates private key.";
                    
                var cert = decryptCmd.Argument("cert", "Certificate file (or private key)").IsRequired();
                var data = decryptCmd.Argument("content", "Content to decrypt").IsRequired();
                
                decryptCmd.OnExecute(() =>
                {
                    var loader = new FilesystemCertificateLoader(cert.Value);
                    var crypter = new RsaCrypter(loader);

                    Console.WriteLine(crypter.DecryptString(data.Value));
                });
            });

            app.OnExecute(() =>
            {
                Console.WriteLine("Specify a subcommand");
                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }
    }
}