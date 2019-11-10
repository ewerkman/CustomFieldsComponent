

namespace Sitecore.Commerce.Plugin.ExternalSettings.Extensions
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Shops;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class ExternalSettingsExtensions
    {
        public static Dictionary<Guid, Dictionary<string, Dictionary<string, string>>> UpdateSettings(
                this Dictionary<Guid, Dictionary<string, Dictionary<string, string>>> settingsCollection,
                string sellableItemSitecoreId,
                string[] parents,
                string[] languages,
                Dictionary<string, string> keyValuePairs)
        {
            if (parents == null)
            {
                return settingsCollection;
            }

            foreach (var parent in parents)
            {
                Guid parentId;
                if (!string.IsNullOrEmpty(parent) && Guid.TryParse(parent, out parentId))
                {
                    var deterministicId = GuidUtils.GetDeterministicGuid($"{sellableItemSitecoreId}|{parent}");

                    if (!settingsCollection.ContainsKey(deterministicId))
                    {
                        settingsCollection[deterministicId] = new Dictionary<string, Dictionary<string, string>>();
                    }

                    var settings = settingsCollection[deterministicId];

                    foreach (var language in languages)
                    {                        
                        if (!settings.ContainsKey(language))
                        {   // Create the language dictionary
                            settings[language] = new Dictionary<string, string>();
                        }

                        // Assign new values
                        foreach(var keyValuePair in keyValuePairs)
                        {
                            settings[language][keyValuePair.Key] = keyValuePair.Value;
                        }
                        
                    }
                }
            }

            return settingsCollection;
        }
    }
}
