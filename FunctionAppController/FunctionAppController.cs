
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using FunctionAppController.model;
using System.Threading.Tasks;
using System;
using WebJobs.Extensions.ResourceDeployment;
using System.Collections.Generic;

namespace FunctionAppController
{
    public static class FunctionAppController
    {
        /// <summary>
        /// Create a Function App for multi-tenant service backed with Durable Functions 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateFunctionApp")]
        public static async Task<IActionResult> CreateFunctionApp(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "functionapp")]HttpRequest req,
            [CosmosDB(
                databaseName: "FunctionAppDB",
                collectionName: "FunctionApp",
                CreateIfNotExists = true,
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<FunctionApp> functionApp,
            [ResourceDeployment("CLIENT_ID", "CLIENT_PASS", "TENANT_ID")] IAsyncCollector<ResourceDeploymentContext> deployment,
            ILogger log)
        {

            try
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                FunctionApp input = JsonConvert.DeserializeObject<FunctionApp>(requestBody);
                if (input.Id == null)
                {
                    input.Id = Guid.NewGuid().ToString();
                }

                if (input.TemplateURL == null)
                {
                    input.TemplateURL = "https://raw.githubusercontent.com/azure/azure-quickstart-templates/master/101-function-app-create-dynamic/azuredeploy.json";
                }

                input.CreatedDate = DateTimeOffset.Now;

                var parameter = JsonConvert.SerializeObject(new TemplateParameter(input.Name));
                var deploymentContext = new ResourceDeploymentContext("functionAppDeploy", input.TemplateURL, parameter, input.ResourceGroup, input.Location);

                await deployment.AddAsync(deploymentContext); // This execute deployment

                log.LogInformation($"FunctionApp {input.Name} has been created.");

                // If success you write cosmos db record for managing functions apps. 

                await functionApp.AddAsync(input);

                return (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(input));
            } catch (Exception e)
            {
                log.LogInformation($"CreateFunctionApp: Error: {e.Message} : {e.StackTrace}");
                return (ActionResult)new System.Web.Http.ExceptionResult(e, true);
            }
        }

        [FunctionName("DeleteFunctionApp")]
        public static IActionResult DeleteFunctionApp([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "functionapp")]HttpRequest req, ILogger log)
        {
            // TODO implement delete feature
            return (ActionResult)new OkObjectResult($"Deleted.");
        }
        [FunctionName("RollingUpdateFunctionApp")]
        public static async Task<IActionResult> RollingUpdateFunctions(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "functions")]HttpRequest req,
            [CosmosDB(
                databaseName: "FunctionAppDB",
                collectionName: "FunctionApp",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "select * from c order by c._ts desc")] IEnumerable<FunctionApp> input,
            [CosmosDB(
                databaseName: "FunctionAppDB",
                collectionName: "FunctionApp",
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<FunctionApp> output,
            [ResourceDeployment("CLIENT_ID", "CLIENT_PASS", "TENANT_ID")] IAsyncCollector<ResourceDeploymentContext> collector,
            ILogger log)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            Artifact artifact = JsonConvert.DeserializeObject<Artifact>(requestBody);

            var azure = ((ResourceDeploymentCollector)collector).GetClient();
            foreach (var functionApp in input)
            {
                // Update the AppSetting
                azure.AppServices.FunctionApps
                    .GetByResourceGroup(functionApp.ResourceGroup, functionApp.Name)
                    .Update()
                    .WithAppSetting("WEBSITE_RUN_FROM_ZIP", artifact.RunFromZipURL)
                    .Apply();
                // output update the function version
                functionApp.FunctionVersion = artifact.FunctionVersion;
                functionApp.FunctionLastUpdate = DateTimeOffset.Now;
                await output.AddAsync(functionApp);
            }

            // Query
            return (ActionResult)new OkObjectResult($"Deleted.");
        }


    }
}
