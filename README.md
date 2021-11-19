# AM-Créations configuration encrypter

## What is AM-Créations configuration encrypter

This tool makes it easy to encrypt and decrypt JSON configuation files. It consists of two parts :
 * A command line utility that lets you encrypt values in your JSON configuration files 
 * A library that decrypts them on the fly in your .NET Standard applications

## Motivation

Projects often contains sensitive information like database connection strings, API keys or usernames 
and passwords for external services. This information should never be committed to source control and 
should be handled in a secure way. Key vaults like those provided by Azure and AWS aren't always
available for projects that can't be connected to the internet.

## Advantages

* Lets you encrypt only certain values in your configuation, so the rest of the config is still readable.
* Access the encrypted values the same way you are used to in your .NET Core applications.
* Lets you share config files or even check them in in your VCS without the need to remove sensitive information.
* The configuration file is not fully encrypted, only its **string** values, so you are aware 
  of which values are overriden and you don't need to have the full **unencrypted** file to add or change a key. 

## Installation

You can install the package via the NuGet Package Manager by searching for `AmCreations.Configuration.EncryptedJson`.

You can also install the package via PowerShell using the following command:

```ps
Install-Package AmCreations.Configuration.EncryptedJson
```

or via the dotnet CLI:

```ps
dotnet add package AmCreations.Configuration.EncryptedJson
```

## Getting started

 * To **decrypt** values for the configuration file you will need a certificate (private and public key), or at least its **private** key.
 * To **encrypt** values for the configuration file you will need a certificate (private and public key), or at least its **public** key.

Certificates can be generated by using openssl. An example certificate is already in the project
and the encrypted string in the example [appsettings.Encrypted.json](src/AmCreations.Configuration.EncryptedJson.SampleWebApp/appsettings.Encrypted.json) file has been encrypted with it.

To generate a certificate you could use the following commands:

```shell
openssl genrsa 2048 > private.key
openssl req -new -x509 -nodes -sha1 -days 365 -key private.key > public.cer
openssl pkcs12 -export -in public.cer -inkey private.key -out cert.pfx -passout pass:
```

### Loading the encrypted JSON configuration

To make an encrypted configuration file, just create a JSON file, like any appsettings file, 
containing only the values you need to encrypt, for example :

```json5
{
  "ConnectionStrings": {
    "Main": ""
  }
}
```

Now you need to encrypt the value used in "ConnectionStrings:Main", you'll need to use the CLI tool : 

This will output the encrypted value of "Content To Encode"

```shell
cont-encrypt encrypt "/path/to/the/public-key-or-certificate" "Content To Encode"
```

Then paste the encrypted value in the appsettings file (here next to "Main").

Add the following to your `Program.cs` file:

```csharp
using AmCreations.Configuration.EncryptedJson;
```

The encrypted JSON configuration can be loaded from a file in your `Program.cs` like this:

```csharp
Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddEncryptedJsonFile("appsettings.Encrypted.json", new FilesystemCertificateLoader("/etc/ssl/private/my-app-cert.pfx"));
    })
```

`AddEncryptedJsonFile()` also supports the `optional` and `reloadOnChange` parameters (like the 
classical `AddJsonFile` method).

You can now access your application's settings by injecting `IConfiguration` or `IOptions` in your 
classes, [as usual](https://docs.microsoft.com/aspnet/core/fundamentals/configuration/#appsettingsjson).

## Credits 

This library is based on the libraries :

 * https://github.com/iotec-io/Iotec.EncryptedConfiguration
 * https://github.com/miqoas/Miqo.EncryptedJsonConfiguration