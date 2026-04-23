targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for all deployed resources')
param location string

@description('Id of the user or app to assign application roles')
param principalId string = ''

@metadata({azd: {
  type: 'generate'
  config: {length:22}
  }
})
@secure()
param postgres_password string

var tags = {
  'azd-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}
module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
    location: location
    tags: tags
    principalId: principalId
  }
}

module funcstoraged5b14 'funcstoraged5b14/funcstoraged5b14.module.bicep' = {
  name: 'funcstoraged5b14'
  scope: rg
  params: {
    location: location
  }
}
module funcstoraged5b14_roles 'funcstoraged5b14-roles/funcstoraged5b14-roles.module.bicep' = {
  name: 'funcstoraged5b14-roles'
  scope: rg
  params: {
    funcstoraged5b14_outputs_name: funcstoraged5b14.outputs.name
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalType: 'ServicePrincipal'
  }
}
module serviceBus 'serviceBus/serviceBus.module.bicep' = {
  name: 'serviceBus'
  scope: rg
  params: {
    location: location
  }
}
module serviceBus_roles 'serviceBus-roles/serviceBus-roles.module.bicep' = {
  name: 'serviceBus-roles'
  scope: rg
  params: {
    location: location
    principalId: resources.outputs.MANAGED_IDENTITY_PRINCIPAL_ID
    principalType: 'ServicePrincipal'
    servicebus_outputs_name: serviceBus.outputs.name
  }
}

output MANAGED_IDENTITY_CLIENT_ID string = resources.outputs.MANAGED_IDENTITY_CLIENT_ID
output MANAGED_IDENTITY_NAME string = resources.outputs.MANAGED_IDENTITY_NAME
output AZURE_LOG_ANALYTICS_WORKSPACE_NAME string = resources.outputs.AZURE_LOG_ANALYTICS_WORKSPACE_NAME
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = resources.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID
output AZURE_CONTAINER_REGISTRY_NAME string = resources.outputs.AZURE_CONTAINER_REGISTRY_NAME
output AZURE_CONTAINER_APPS_ENVIRONMENT_NAME string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_NAME
output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_ID
output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = resources.outputs.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN
output FUNCSTORAGED5B14_BLOBENDPOINT string = funcstoraged5b14.outputs.blobEndpoint
output FUNCSTORAGED5B14_DATALAKEENDPOINT string = funcstoraged5b14.outputs.dataLakeEndpoint
output FUNCSTORAGED5B14_QUEUEENDPOINT string = funcstoraged5b14.outputs.queueEndpoint
output FUNCSTORAGED5B14_TABLEENDPOINT string = funcstoraged5b14.outputs.tableEndpoint
output SERVICEBUS_SERVICEBUSENDPOINT string = serviceBus.outputs.serviceBusEndpoint
output SERVICEBUS_SERVICEBUSHOSTNAME string = serviceBus.outputs.serviceBusHostName
