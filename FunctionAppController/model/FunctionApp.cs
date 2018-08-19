using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionAppController.model
{
    public class FunctionApp
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string ResourceGroup { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string TemplateURL { get; set; }
        public string FunctionVersion { get; set; }
        public DateTimeOffset FunctionLastUpdate { get; set; }
        public DateTimeOffset CreatedDate { get; set;}

    }
}
