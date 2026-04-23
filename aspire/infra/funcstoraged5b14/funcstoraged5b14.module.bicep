@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource funcstoraged5b14 'Microsoft.Storage/storageAccounts@2024-01-01' = {
  name: take('funcstoraged5b14${uniqueString(resourceGroup().id)}', 24)
  kind: 'StorageV2'
  location: location
  sku: {
    name: 'Standard_GRS'
  }
  properties: {
    accessTier: 'Hot'
    allowSharedKeyAccess: false
    isHnsEnabled: false
    minimumTlsVersion: 'TLS1_2'
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
  tags: {
    'aspire-resource-name': 'funcstoraged5b14'
  }
}

output blobEndpoint string = funcstoraged5b14.properties.primaryEndpoints.blob

output dataLakeEndpoint string = funcstoraged5b14.properties.primaryEndpoints.dfs

output queueEndpoint string = funcstoraged5b14.properties.primaryEndpoints.queue

output tableEndpoint string = funcstoraged5b14.properties.primaryEndpoints.table

output name string = funcstoraged5b14.name

output id string = funcstoraged5b14.id