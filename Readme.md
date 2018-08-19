# Function App Controller

Function App Controller enable us to deploy and manage Function App and Functions

This project using [ResourceDeployment bindings](WebJobs.Extensions.ResourceDeployment) which I developmed. 

# How to use 

## Configuration 

Go to FunctionAppController dir and configure lcal.settings.json. If you want to deploy this tool, you can setup App Settings. 
You need a cosmosdb instance and Service Principal which has priviledge to deploy funciton app. You can refer the [Create an Azure service principal with Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/create-an-azure-service-principal-azure-cli?view=azure-cli-latest)

Please fill the local.settings.json or AppSettings from CosmosDB Connection Strings and Service Principal values to fit this format. 

_local.settings.json_

```json
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "CosmosDBConnection": "YOUR_COSMOSDB_CONNECTION_HERE",
    "CLIENT_ID": "YOUR_SERVICE_PRINCIPAL_CLIENT_ID_HERE",
    "CLIENT_PASS": "YOUR_SERVICE_PRINCIPAL_CLIENT_PASS_HERE",
    "TENANT_ID": "YOUR_SERVICE_PRINCIPAL_TENANT_ID_HERE"
  }
}
```

## Create A Function App 

This REST API call will create a function app with inserting metadata into CosmosDB. 


### Request URL

If you deploy to Azure, please change the `localhost:7071` to fit your function app.

```
POST http://localhost:7071/api/functionapp
```

with `application/json`.

### Request Body

```
{
	"ResourceGroup": "RESOURCE_GROUP_NAME",
	"Name": "YOUR_FUNCTION_APP_NAME",
	"Location": "westus",
	"FunctionVersion": "0.0.0"
}
```

## Deploy Functions 

This REST-API will deploy your functions to all your function apps. This REST-API fetch the all function apps from cosmosdb then update AppSettings to deploy zip file using [RunFromZip](https://github.com/Azure/app-service-announcements/issues/84) deployment.

### Request URL

If you deploy to Azure, please change the `localhost:7071` to fit your function app.

```
POST http://localhost:7071/api/functions
```

with `application/json`.

### Request Body

`RunFromZipURL` is a url which you put a zip file that you want to deploy. Usually, we use blob storage for this purpose.
Also, you can see the version. 

```
{
	"RunFromZipURL": "THE_URL_THAT_YOU_STORE_THE_ZIP_FILE",
	"FunctionVersion": "1.0.0"
}
```

# Database 

## CosmosDB Schema

This Controller create a database `FunctionAppDB` and Collection `FunctionApp`. CosmosDB manage these information related FunctionApps.

```
{
    "id": "26adbd74-3294-4eb8-bdf4-3b7cd328c43d",
    "ResourceGroup": "MultiTenantResource",
    "Name": "sasaasapp02",
    "Location": "westus",
    "TemplateURL": "https://raw.githubusercontent.com/azure/azure-quickstart-templates/master/101-function-app-create-dynamic/azuredeploy.json",
    "FunctionVersion": "1.0.0",
    "FunctionLastUpdate": "2018-08-17T12:49:18.7897068-07:00",
    "CreatedDate": "2018-08-17T12:38:09.9437661-07:00",
    "_rid": "cX0gAPeU7vwDAAAAAAAAAA==",
    "_self": "dbs/cX0gAA==/colls/cX0gAPeU7vw=/docs/cX0gAPeU7vwDAAAAAAAAAA==/",
    "_etag": "\"6a01498f-0000-0000-0000-5b7726bf0000\"",
    "_attachments": "attachments/",
    "_ts": 1534535359
}
```

# TODO 

Currently I have some issue to fix. 

* When I update the function, it doubles: The root cause is I use Id instead of id. 
* KeyVault Integration: I should use KeyVault instead directory use Service Prncipal on the AppSettings. 

