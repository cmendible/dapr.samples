data "helm_repository" "aad-pod-identity" {
  name = "aad-pod-identity"
  url  = "https://raw.githubusercontent.com/Azure/aad-pod-identity/master/charts"
}

locals {
  identities = [
    {
      name       = var.managed_identity_name
      type       = 0
      resourceID = azurerm_user_assigned_identity.mi.id
      clientID   = azurerm_user_assigned_identity.mi.client_id
      binding = {
        name     = "${var.managed_identity_name}-identity-binding"
        selector = var.managed_identity_selector
      }
    }
  ]
}

resource "helm_release" "aad-pod-identity" {
  name       = "aad-pod-identity"
  chart      = "aad-pod-identity/aad-pod-identity"
  repository = data.helm_repository.aad-pod-identity.metadata[0].name

  values = [
    <<-EOT
    azureIdentities:
      - name: ${var.managed_identity_name}
        type: 0
        resourceID: ${azurerm_user_assigned_identity.mi.id}
        clientID: ${azurerm_user_assigned_identity.mi.client_id}
        binding:
          name: ${var.managed_identity_name}-identity-binding
          selector: ${var.managed_identity_selector}
    EOT
  ]
}
