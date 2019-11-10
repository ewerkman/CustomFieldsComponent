namespace Sitecore.Commerce.Plugin.ExternalSettings.Pipelines.Blocks
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.ExternalSettings.Components;
    using Sitecore.Framework.Pipelines;
    using System.Threading.Tasks;

    [PipelineDisplayName(nameof(LoadExternalSettingsBlock))]
    public class LoadExternalSettingsBlock : PipelineBlock<CommerceEntity, CommerceEntity, CommercePipelineExecutionContext>
    {
        protected CommerceCommander Commander { get; set; }

        public LoadExternalSettingsBlock(CommerceCommander commander)
            : base(null)
        {
            this.Commander = commander;
        }

        public override Task<CommerceEntity> Run(CommerceEntity arg, CommercePipelineExecutionContext context)
        {
            if(arg is null)
            {
                return Task.FromResult(arg);
            }

            if(arg is SellableItem && arg.HasComponent<CustomFieldsComponent>())
            {
                var sellableItem = (SellableItem)arg;
                var externalSettingsComponent = arg.GetComponent<ExternalSettingsComponent>();
                var customFieldsComponent = arg.GetComponent<CustomFieldsComponent>();

                customFieldsComponent.UpdateExternalSettings(externalSettingsComponent, sellableItem);                
            }

            return Task.FromResult(arg);
        }
    }
}
