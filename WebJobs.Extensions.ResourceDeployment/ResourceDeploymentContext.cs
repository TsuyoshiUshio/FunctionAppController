using System;
using System.Collections.Generic;
using System.Text;

namespace WebJobs.Extensions.ResourceDeployment
{
    public class ResourceDeploymentContext
    {
        public string Name { get; set; }
        public string TemplateUrl { get; set; }
        public string TemplateVersion { get; set; }
        public string Parameter { get; set; }
        public string ResourceGroup { get; set; }
        public string Location { get; set; }
        public Microsoft.Azure.Management.ResourceManager.Fluent.Models.DeploymentMode DeploymentMode { get; set; }

        public ResourceDeploymentContext(string name, string templateUrl, string parameter, string resourceGroup, string location)
        {
            this.Name = name;
            this.TemplateUrl = templateUrl;
            this.Parameter = parameter;
            this.ResourceGroup = resourceGroup;
            this.Location = location;
            SetDefault();
        }

        private void SetDefault()
        {
            this.TemplateVersion = "1.0.0.0";
            this.DeploymentMode = Microsoft.Azure.Management.ResourceManager.Fluent.Models.DeploymentMode.Incremental;
        }
    }
}
