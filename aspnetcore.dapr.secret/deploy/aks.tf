# Create Application Registration. 
resource "azuread_application" "aks_app" {
  name     = var.cluster_name
  homepage = "http://${var.cluster_name}"
  identifier_uris = [
    "http://${var.cluster_name}"
  ]
  reply_urls                 = []
  available_to_other_tenants = false
  oauth2_allow_implicit_flow = false
}

# Create Service Principal
resource "azuread_service_principal" "aks_sp" {
  application_id               = azuread_application.aks_app.application_id
  app_role_assignment_required = false

  tags = []
}

resource "random_password" "aks_password" {
  length           = 16
  special          = true
  override_special = "_%@"
  keepers = {
    aks_app_id =  azuread_application.aks_app.id
  }
}

resource "azuread_application_password" "aks_password" {
  application_object_id = azuread_application.aks_app.id
  value                 = random_password.aks_password.result
  end_date_relative     = "87600h"
}

# Deploy Kubernetes
resource "azurerm_kubernetes_cluster" "k8s" {
  name                = var.cluster_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  dns_prefix          = var.dns_prefix

  default_node_pool {
    name       = "default"
    node_count = var.agent_count
    vm_size    = "Standard_D2_v2"
  }

  service_principal {
    client_id     = azuread_application.aks_app.application_id
    client_secret = azuread_application_password.aks_password.value
  }

  role_based_access_control {
    enabled = true
  }
}
