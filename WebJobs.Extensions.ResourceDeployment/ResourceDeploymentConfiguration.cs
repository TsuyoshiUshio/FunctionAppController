using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.WebJobs.Host.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WebJobs.Extensions.ResourceDeployment
{
    public class ResourceDeploymentConfiguration : IExtensionConfigProvider
    {
        internal readonly ConcurrentDictionary<string, IAzure> ClientCache = new ConcurrentDictionary<string, IAzure>();
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<ResourceDeploymentAttribute>().BindToCollector(attr => new ResourceDeploymentCollector(attr, this));
        }

        internal IAzure GetClient(string clientId, string clientPassword, string tenantId)
        {
            return ClientCache.GetOrAdd(clientId, (c) => {
                var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(c, clientPassword, tenantId, AzureEnvironment.AzureGlobalCloud);
                var client = Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(credentials)
                    .WithDefaultSubscription();
                return client;
            });

        }
    }
}
