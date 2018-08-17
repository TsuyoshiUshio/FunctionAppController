using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebJobs.Extensions.ResourceDeployment
{
    /// <summary>
    /// Defines the configuration options for the ARM template bindings
    /// </summary>
    public class ResourceDeploymentCollector : IAsyncCollector<ResourceDeploymentContext>
    {
        private IAzure client;
        private List<ResourceDeploymentContext> deployments = new List<ResourceDeploymentContext>();
        private ResourceDeploymentAttribute attribute;
        private ResourceDeploymentConfiguration config;
        public ResourceDeploymentCollector(ResourceDeploymentAttribute attribute, ResourceDeploymentConfiguration config)
        {
            this.attribute = attribute;
            this.config = config;
            ConfigureClient();
        }

        public async Task AddAsync(ResourceDeploymentContext item, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (attribute.DeferMode)
            {
                // Just add to the list and executed at the FlushAsync
                deployments.Add(item);

            }
            else
            {
                // Execute deployment immediately for getting error
                await CreateResourceGroupIfExistsAsync(item);
                await ExecuteDeploymentAsync(item);
            }
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (attribute.DeferMode)
            {
                // Execute deployments 
                foreach (var deployment in this.deployments)
                {
                    await CreateResourceGroupIfExistsAsync(deployment);
                    await ExecuteDeploymentAsync(deployment);
                    return;
                }
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// This method is for someone who wants to deploy manually.
        /// </summary>
        /// <returns></returns>
        public IAzure GetClient()
        {
            return this.client;
        }

        private void ConfigureClient()
        {
            var clientId = Environment.GetEnvironmentVariable(this.attribute.ClientID);
            var clientPassword = Environment.GetEnvironmentVariable(this.attribute.ClientPassword);
            var tenantId = Environment.GetEnvironmentVariable(this.attribute.TenantId);
            this.client = config.GetClient(clientId, clientPassword, tenantId);
        }

        private async Task<IResourceGroup> CreateResourceGroupIfExistsAsync(ResourceDeploymentContext deployment)
        {
            if (await client.ResourceGroups.ContainAsync(deployment.ResourceGroup))
            {
                return await client.ResourceGroups.GetByNameAsync(deployment.ResourceGroup);
            }
            else
            {
                return await client.ResourceGroups.Define(deployment.ResourceGroup)
                                    .WithRegion(deployment.Location)
                                    .CreateAsync();
            }
        }

        private async Task<IDeployment> ExecuteDeploymentAsync(ResourceDeploymentContext deployment)
        {
            return await client.Deployments.Define(deployment.Name)
                .WithExistingResourceGroup(deployment.ResourceGroup)
                .WithTemplateLink(deployment.TemplateUrl, deployment.TemplateVersion)
                .WithParameters(deployment.Parameter)
                .WithMode(Microsoft.Azure.Management.ResourceManager.Fluent.Models.DeploymentMode.Incremental)
                .CreateAsync();
        }

    }
}
