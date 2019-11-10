// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Plugin.ExternalSettings
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ExternalSettings.Pipelines.Blocks;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config =>
                    config.ConfigurePipeline<IFindEntityPipeline>(c =>
                       c.Add<LoadExternalSettingsBlock>().After<LoadEntityLocalizedValuesBlock>()
                    ));

            services.Sitecore().Pipelines(config =>
                    config.ConfigurePipeline<IPersistEntityPipeline>(c =>
                       c.Add<SaveExternalSettingsBlock>().After<LocalizeEntityBlock>()
                    ));

            services.RegisterAllCommands(assembly);
        }
    }
}