using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebJobs.Extensions.ResourceDeployment;

namespace FunctionAppController.model
{
    public class TemplateParameter
    {
        [JsonProperty(PropertyName = "appName")]
        public StringParameter AppName { get; set; }

        public TemplateParameter(string appName)
        {
            AppName = new StringParameter(appName);
        }
    }
}
