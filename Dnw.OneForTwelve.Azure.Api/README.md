# Intro

Microsoft Azure Functions implementation of the game api.

For running / debugging functions locally in Rider and to be able to publish them to azure using a .NET Publish profile you first need to install the "Azure Toolkit for Rider" plugin.  

# Run locally

Using the azure CLI or Rider are both possible.

## CLI

In a terminal window type the following:

```
func host start --port 5001 --pause-on-error --useHttps --useDefaultCert
```

# Deployment

Deployment to Azure can be done with either the CLI or with a Rider publish profile.

## Rider

Create a new run profile under .NET -> Azure Functions Host.  

## CLI

After initial setup (see below) you can create / update the Azure Function using the commands (example on mac):

```
rm -rf ./publish
dotnet publish -c Release -o ./publish
zip -r ./publish/publish.zip ./publish
az functionapp deployment source config-zip -g rg-dnw -n Dnw-OneForTwelve-Azure-Api --src ./publish/publish.zip
```

More info here:

https://learn.microsoft.com/en-us/azure/azure-functions/deployment-zip-push  
https://markheath.net/post/deploying-azure-functions-with-azure-cli  

## Rider

Create a new .NET Publish profile under .NET Publish -> Azure Publish Function App. 

# Initial setup

General instructions on how to get started with Azure Functions (out-of-process) can be found here:  

https://learn.microsoft.com/en-us/azure/azure-functions/create-first-function-cli-csharp?tabs=azure-cli%2Cisolated-process
https://www.cazzulino.com/net6functions.html
https://andrewlock.net/exploring-dotnet-6-part-2-comparing-webapplicationbuilder-to-the-generic-host/

## Install Azure Functions Core Tools

See: 

https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#v2

```
brew tap azure/functions
brew install azure-functions-core-tools@4
# if upgrading on a machine that has 2.x or 3.x installed:
brew link --overwrite azure-functions-core-tools@4
```

## Install 

See: 

https://learn.microsoft.com/en-us/cli/azure/install-azure-cli

```
brew update && brew install azure-cli
```

And for Rider you need the Azure Toolkit: https://plugins.jetbrains.com/plugin/11220-azure-toolkit-for-rider

## Create Azure Function

```
func init Dnw.OneForTwelve.Azure.Api --worker-runtime dotnet-isolated --target-framework net6.0
func new --name HttpExample --template "HTTP trigger" --authlevel "anonymous"
```

# Authentication

Its a bit of a mess at the moment. Compare that to the aws lambda code, which is very straightforward because you can use all the normal minimal api features.

The best thing I have found so far is how to do it yourself:

https://joonasw.net/view/azure-ad-jwt-authentication-in-net-isolated-process-azure-functions

# Https

```
host start --port 5001 --pause-on-error --useHttps --useDefaultCert
```

# Issues

Certificates:

https://stackoverflow.com/questions/62453695/how-to-enable-azure-function-https-easily-when-do-local-test

Json serialization:

https://github.com/Azure/azure-functions-host/issues/5469