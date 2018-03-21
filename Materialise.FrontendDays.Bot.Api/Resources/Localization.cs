using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Materialise.FrontendDays.Bot.Api.Resources
{
    public class Localization
    {
        private readonly string _filename;
        private IDictionary<string, string> _localizations;

        public Localization(string filename)
        {
            _filename = filename;
        }

        public string this[string key]
        {
            get
            {
                if (_localizations == null)
                {
                    using (var resource = Assembly.GetEntryAssembly()
                        .GetManifestResourceStream(_filename))
                    using (var reader = new StreamReader(resource))
                    {
                        var text = reader.ReadToEnd();
                        _localizations = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                    }
                }

                return _localizations[key];
            }
        }
    }
}
