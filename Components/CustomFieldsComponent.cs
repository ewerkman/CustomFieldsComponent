using Newtonsoft.Json;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.ExternalSettings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Commerce.Plugin.ExternalSettings.Components
{
    public class CustomFieldsComponent : Component
    {
        private const string ExternalSettingsSharedFieldName = "shared";

        public CustomFieldsComponent()
        {
            CustomFields = new Dictionary<string, string>();
        }

        public Dictionary<string,string> CustomFields { get; }
        public string OriginalSettings { get; private set; }    // Copy of the Settings property of the ExternalSettingsComponent (used to check for changes)

        /// <summary>
        ///     Updates the ExternalSettingsComponent from the CustomFieldsComponent
        /// </summary>
        /// <param name="externalSettingsComponent"></param>
        /// <param name="sellableItem"></param>
        /// <returns></returns>
        public CustomFieldsComponent UpdateExternalSettings(ExternalSettingsComponent externalSettingsComponent, SellableItem sellableItem)
        {
            var settingsCollection = ParseExternalSettings(externalSettingsComponent.Settings);

            string[] allParents = GetAllParents(sellableItem);

            var newValues = this.CustomFields;

            var languages = new List<string>();
            languages.Add(ExternalSettingsSharedFieldName);

            settingsCollection = settingsCollection.UpdateSettings(sellableItem.SitecoreId, allParents, languages.ToArray(), this.CustomFields);

            var serializedSettings = JsonConvert.SerializeObject(settingsCollection);
            externalSettingsComponent.Settings = serializedSettings;

            OriginalSettings = serializedSettings;

            return this;
        }

        /// <summary>
        ///     Updates the CustomFieldsComponent from the ExternalSettingsComponent
        /// </summary>
        /// <param name="externalSettingsComponent"></param>
        /// <param name="sellableItem"></param>
        /// <returns></returns>
        public CustomFieldsComponent UpdateFromExternalSettingsComponent(ExternalSettingsComponent externalSettingsComponent, SellableItem sellableItem)
        {
            var updatedSettings = ParseExternalSettings(externalSettingsComponent.Settings);
            var originalSettings = ParseExternalSettings(this.OriginalSettings);

            string[] allParents = GetAllParents(sellableItem);

            var newValues = this.CustomFields;

            if (allParents != null)
            {
                foreach (var parent in allParents)
                {
                    Guid parentId;
                    if (!string.IsNullOrEmpty(parent) && Guid.TryParse(parent, out parentId))
                    {
                        var deterministicId = GuidUtils.GetDeterministicGuid($"{sellableItem.SitecoreId}|{parent}");

                        if (!updatedSettings.ContainsKey(deterministicId))
                        {
                            updatedSettings[deterministicId] = new Dictionary<string, Dictionary<string, string>>();
                        }

                        // Check which parent was updated
                        if (originalSettings.ContainsKey(deterministicId) && HaveSettingsBeenUpdated(updatedSettings[deterministicId], originalSettings[deterministicId]))
                        {
                            var settings = updatedSettings[deterministicId];

                            if (!settings.ContainsKey(ExternalSettingsSharedFieldName))
                            {   // Create the language dictionary
                                settings[ExternalSettingsSharedFieldName] = new Dictionary<string, string>();
                            }

                            // Assign new values only checking the keys we have in the custom fields component
                            var currentDictionary = new Dictionary<string, string>(this.CustomFields);
                            foreach (var keyValuePair in currentDictionary)
                            {
                                if (settings[ExternalSettingsSharedFieldName].ContainsKey(keyValuePair.Key))
                                {
                                    if (string.CompareOrdinal(settings[ExternalSettingsSharedFieldName][keyValuePair.Key], keyValuePair.Value) != 0)
                                    {
                                        this.CustomFields[keyValuePair.Key] = settings[ExternalSettingsSharedFieldName][keyValuePair.Key];
                                    }
                                }
                            }
                        }                        
                    }
                }
            }

            this.OriginalSettings = externalSettingsComponent.Settings;

            return this;
        }

        /// <summary>
        ///     Returns true when the revision of one of the languages of newSettings is different from originalSettings
        /// </summary>
        /// <param name="newSettings"></param>
        /// <param name="originalSettings"></param>
        /// <returns></returns>
        private bool HaveSettingsBeenUpdated(Dictionary<string, Dictionary<string, string>> newSettings, Dictionary<string, Dictionary<string, string>> originalSettings)
        {
            bool hasChanged = false;

            foreach (var language in newSettings.Keys)
            {
                if (language == ExternalSettingsSharedFieldName)
                {
                    continue;
                }
                var newRevision = newSettings[language]["__Revision"];
                if (originalSettings.ContainsKey(language))
                { 
                    var oldRevision = originalSettings[language]["__Revision"];
                    if (newRevision != oldRevision)
                    {
                        hasChanged = true;
                        break;
                    }
                }
                else
                {
                    hasChanged = true;
                    break;
                }
            }

            return hasChanged;
        }

        /// <summary>
        ///     Parses the external settings
        /// </summary>
        /// <param name="externalSettings"></param>
        /// <returns></returns>
        private static Dictionary<Guid, Dictionary<string, Dictionary<string, string>>> ParseExternalSettings(string externalSettings)
        {
            var settingsCollection = new Dictionary<Guid, Dictionary<string, Dictionary<string, string>>>();

            if (!string.IsNullOrEmpty(externalSettings))
            {
                try
                {
                    settingsCollection = JsonConvert.DeserializeObject<Dictionary<Guid, Dictionary<string, Dictionary<string, string>>>>(externalSettings);
                }
                catch
                {
                }
            }

            return settingsCollection;
        }

        /// <summary>
        ///     Returns an array of all the parent ids (category + catalog) of a SellableItem
        /// </summary>
        /// <param name="sellableItem"></param>
        /// <returns></returns>
        private static string[] GetAllParents(SellableItem sellableItem)
        {
            var parentCatalogs = sellableItem.ParentCatalogList?.Split('|');
            var parentCategories = sellableItem.ParentCategoryList?.Split('|');

            return parentCatalogs == null ? parentCategories : parentCategories == null ? parentCatalogs : parentCatalogs.Union(parentCategories).ToArray();
        }


    }
}
