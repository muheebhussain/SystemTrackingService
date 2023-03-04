#!/bin/bash

# Set the parameters for the script
resourceGroupName="<resource-group-name>"
storageAccountName="<storage-account-name>"
storageContainerName="<storage-container-name>"
bacpacFileName="<bacpac-file-name>"
serverName="<server-name>"
databaseName="<database-name>"
serverAdminLoginName="<admin-login-name>"
serverAdminPassword="<admin-login-password>"
databaseCollation="<database-collation>"

# Get the private endpoint connection ID for the storage account
storagePrivateEndpointConnectionId=$(az storage account show -n $storageAccountName --query "privateEndpointConnections[?privateLinkServiceConnectionState.status=='Approved'].id" --output tsv)

# Create a new SQL Server database
az sql db create --name $databaseName --server $serverName --resource-group $resourceGroupName --collation $databaseCollation

# Import the .bacpac file into the SQL Server database using the private endpoint
az sql db import --admin-password $serverAdminPassword --admin-user $serverAdminLoginName --connection-string-encrypted --storage-key-type SharedAccessKey --storage-endpoint https://$storageAccountName.blob.private.core.windows.net --storage-container $storageContainerName --storage-blob-name $bacpacFileName --connection-name $storagePrivateEndpointConnectionId --name $databaseName --resource-group $resourceGroupName --server $serverName
