@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource openai 'Microsoft.CognitiveServices/accounts@2024-10-01' = {
  name: take('openai-${uniqueString(resourceGroup().id)}', 64)
  location: location
  kind: 'OpenAI'
  properties: {
    customSubDomainName: toLower(take(concat('openai', uniqueString(resourceGroup().id)), 24))
    publicNetworkAccess: 'Enabled'
    disableLocalAuth: true
  }
  sku: {
    name: 'S0'
  }
  tags: {
    'aspire-resource-name': 'openai'
  }
}

resource preview 'Microsoft.CognitiveServices/accounts/deployments@2024-10-01' = {
  name: 'preview'
  properties: {
    model: {
      format: 'OpenAI'
      name: 'gpt-4.5-preview'
      version: '2025-02-27'
    }
  }
  sku: {
    name: 'Standard'
    capacity: 8
  }
  parent: openai
}

output connectionString string = 'Endpoint=${openai.properties.endpoint}'

output name string = openai.name