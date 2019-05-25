using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.SecretsEngines.Consul;

namespace vaultsharp_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new ConfigurationBuilder()
                .AddCommandLine(args)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables().Build();

            var vaultUrl = cfg.GetSection("vault-url").Value;
            if (string.IsNullOrEmpty(vaultUrl))
                throw new ArgumentNullException("vault-url not provided in the appsettings!");

            var vaultToken = cfg.GetSection("vault-token").Value;

            if (string.IsNullOrEmpty(vaultToken))
                throw new ArgumentNullException("vault-token not provided in the appsettings!");

            // Initialize one of the several auth methods.
            var authMethod = new TokenAuthMethodInfo(vaultToken);

            // Initialize settings. You can also set proxies, custom delegates etc. here.
            var vaultClientSettings = new VaultClientSettings(vaultUrl, authMethod);

            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

            Secret<SecretData> kv2Secret = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync("sample-secret").Result;
            var login = kv2Secret.Data.Data["login"];
            var password = kv2Secret.Data.Data["password"];
            Console.WriteLine($"data from vault.\nLogin:{login} \nPassword{password}");
        }
    }
}
