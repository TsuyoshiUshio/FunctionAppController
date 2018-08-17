# Resource Deployment bindings 

This is Azure Functions output bindings for deploying ARM template with dynamic parameter.

# Usage 

## local.settings.json 

Copy `local.settings.json.example` to `local.settings.json` 

Edit `local.settings.json`. You need to get Service Principal. See more detail 

* [reate an Azure service principal with Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/create-an-azure-service-principal-azure-cli?view=azure-cli-latest)

```
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "CLIENT_ID": "YOUR_CLIENT_ID_HERE",
    "CLIENT_PASS": "YOUR_CLIENT_PASS_HERE",
    "TENANT_ID": "YOUR_TENANT_ID_HERE"
  }
}
```

## Bindings 

You can use the output bindings like this. ResourceDeployment attributes has one optional parameter called `DeferMode` this boolean value enable 
you to defer the execution after executing Azure Functions. By default, it is `false` which means, the deployment is executed when you call `AddAsync` method. 
It helps you to debug the ARM deployment. Other bindings doesn't have this feature always `deferMode = true` .

Also, we provide, `StringParameter` and `IntParameter` class for support ARM template parameter. 

```
        [JsonProperty(PropertyName = "appName")]
        public StringParameter AppName { get; set; }

        public TemplateParameter(string appName)
        {
            AppName = new StringParameter(appName);
        }

        [FunctionName("Sample")]
        public static async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req,
            [ResourceDeployment("CLIENT_ID", "CLIENT_PASS", "TENANT_ID")] IAsyncCollector<ResourceDeploymentContext> collector,
            TraceWriter log) {

			var parameter = JsonConvert.SerializeObject(new TemplateParameter("resourceBindingSample5"));
            var deployment = new ResourceDeploymentContext("testResource5", "https://raw.githubusercontent.com/azure/azure-quickstart-templates/master/101-function-app-create-dynamic/azuredeploy.json", parameter, "testResource5", "westus");

            await collector.AddAsync(deployment); // This execute deployment
			}


```

## Testing

If you want to try this bindings, Please start `FunctionsSample` project with configure `local.settings.json` I recommend to change the name of `testResource5` to something else. 
It will deploy one FunctionApp for you. 

Enjoy

