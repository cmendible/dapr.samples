#!/bin/bash

set -e

echo "Getting AKS credentials..."
az aks get-credentials -n $AKS_NAME -g $AKS_RG --overwrite-existing

echo "Installing dapr"

dapr init -k

echo "dapr has been installed."