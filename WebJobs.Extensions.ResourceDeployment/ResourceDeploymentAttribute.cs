using Microsoft.Azure.WebJobs.Description;
using System;

namespace WebJobs.Extensions.ResourceDeployment
{
    /// <summary>
    /// Attribute used to bind an ARM template deployment client
    /// <remarks>
    /// The method parameter type can be following.
    /// <list type="bullet">
    /// <item><description><see cref="IAsyncCollector"/></description></item>
    /// </list>
    /// </remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class ResourceDeploymentAttribute : Attribute
    {
        public string ClientID { get; set; }
        public string ClientPassword { get; set; }
        public string TenantId { get; set; }
        public bool DeferMode { get; set; }

        /// <summary>
        /// Constructs a new instance. Set the Application attribute with ServicePrincipal.
        /// </summary>
        /// <param name="clientId">Application Id</param>
        /// <param name="clientPassword">Application Password</param>
        /// <param name="tenantId">Tenant Id</param>
        public ResourceDeploymentAttribute(string clientId, string clientPassword, string tenantId)
        {
            ClientID = clientId;
            ClientPassword = clientPassword;
            TenantId = tenantId;
            DeferMode = false;
        }
    }
}
