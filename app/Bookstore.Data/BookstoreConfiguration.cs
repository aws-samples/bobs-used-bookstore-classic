using System;
using System.Collections.Generic;
using System.Configuration;

namespace BobsBookstoreClassic.Data
{
    public sealed class BookstoreConfiguration
    {
        private static readonly Lazy<BookstoreConfiguration> Lazy = new Lazy<BookstoreConfiguration>(() => new BookstoreConfiguration());

        private static BookstoreConfiguration Instance => Lazy.Value;

        private readonly Dictionary<string, string> _appSettings = new Dictionary<string, string>();

        private BookstoreConfiguration()
        {
            foreach (string key in ConfigurationManager.AppSettings)
            {
                _appSettings[key] = ConfigurationManager.AppSettings[key];

                if (Environment.GetEnvironmentVariable(key) != null)
                {
                    _appSettings[key] = Environment.GetEnvironmentVariable(key);
                }
            }
        }

        public static void Add(string key, string value)
        {
            Instance._appSettings[key] = value;
        }

        public static string Get(string key)
        {
            return Instance._appSettings[key];
        }

        public static T Get<T>(string key)
        {
            var value = Instance._appSettings[key];

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
