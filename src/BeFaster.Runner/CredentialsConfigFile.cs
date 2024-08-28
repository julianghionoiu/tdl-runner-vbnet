using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BeFaster.Runner.Exceptions;
using BeFaster.Runner.Utils;
using TDL.Client.Utils;

namespace BeFaster.Runner
{
    public static class CredentialsConfigFile
    {
        private static readonly Dictionary<string, string> Properties = new();

        static CredentialsConfigFile()
        {
            try
            {
                var credentialsPath = Path.Combine(PathHelper.RepositoryPath, "config", "credentials.config");

                foreach (var row in File.ReadAllLines(credentialsPath))
                {
                    var data = row.Split('=');
                    var key = data[0];
                    Properties[key] = string.Join("=", data.Skip(1)).Replace("\\=", "=");
                }
            }
            catch (IOException e)
            {
                throw new ConfigNotFoundException($@"The ""credentials.config"" has not been found. Please download from challenge page. ( Reason: ""{e.Message}"")", e);
            }
        }

        public static string Get(string key) =>
            Properties.TryGetValue(key, out var value)
                ? value
                : throw new ConfigNotFoundException($@"The ""credentials.config"" file does not contain key {key}");
                
        public static string Get(string key, string defaultValue) =>
            Properties.GetValueOrDefault(key) ?? defaultValue;

        public static T Get<T>(string key, T defaultValue) =>
            (T)Convert.ChangeType(Get(key), typeof(T));
    }
}
