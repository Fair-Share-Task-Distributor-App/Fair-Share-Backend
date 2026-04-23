@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param sku string = 'Standard'

resource serviceBus 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: take('serviceBus-${uniqueString(resourceGroup().id)}', 50)
  location: location
  properties: {
    disableLocalAuth: true
    publicNetworkAccess: 'Enabled'
  }
  sku: {
    name: sku
  }
  tags: {
    'aspire-resource-name': 'serviceBus'
  }
}

resource tasksQueue 'Microsoft.ServiceBus/namespaces/queues@2024-01-01' = {
  name: 'tasksQueue'
  parent: serviceBus
}

output serviceBusEndpoint string = serviceBus.properties.serviceBusEndpoint

output serviceBusHostName string = split(replace(serviceBus.properties.serviceBusEndpoint, 'https://', ''), ':')[0]

output name string = serviceBus.name

output id string = serviceBus.id