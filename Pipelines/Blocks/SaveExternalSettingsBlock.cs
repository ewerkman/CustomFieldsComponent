namespace Sitecore.Commerce.Plugin.ExternalSettings.Pipelines.Blocks
{
    using Newtonsoft.Json;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.ExternalSettings.Components;
    using Sitecore.Commerce.Plugin.Shops;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [PipelineDisplayName("Change to <Project>Constants.Pipelines.Blocks.<Block Name>")]
    public class SaveExternalSettingsBlock : PipelineBlock<PersistEntityArgument, PersistEntityArgument, CommercePipelineExecutionContext>
    {
        protected CommerceCommander Commander { get; set; }

        public SaveExternalSettingsBlock(CommerceCommander commander)
            : base(null)
        {
            this.Commander = commander;
        }

        public override Task<PersistEntityArgument> Run(PersistEntityArgument arg, CommercePipelineExecutionContext context)
        {
            if (arg is null)
            {
                return Task.FromResult(arg);
            }

            if (arg.Entity is SellableItem && arg.Entity.HasComponent<CustomFieldsComponent>())
            {
                var sellableItem = (SellableItem)arg.Entity;
                var externalSettingsComponent = arg.Entity.GetComponent<ExternalSettingsComponent>();
                var customFieldsComponent = arg.Entity.GetComponent<CustomFieldsComponent>();

                customFieldsComponent.UpdateFromExternalSettingsComponent(externalSettingsComponent, sellableItem);
            }

            return Task.FromResult(arg);
        }
    }
}
