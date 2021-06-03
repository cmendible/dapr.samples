# Deploy Storage Class and sample PVC
resource "null_resource" "secretstore" {
  provisioner "local-exec" {
    interpreter = ["PowerShell", "-Command"]
    command = "(Get-Content ./components/secretstore.yaml) -Replace 'your_keyvault_name', '${azurerm_key_vault.kv.name}' -replace 'your_managed_identity_client_id', '${azurerm_user_assigned_identity.mi.client_id}' | kubectl apply -f -"
  }

  depends_on = [
    helm_release.aad-pod-identity
  ]
}

resource "null_resource" "k8s" {
  provisioner "local-exec" {
    interpreter = ["PowerShell", "-Command"]
    command = "kubectl apply -f ./components/k8s.yaml"
  }

  depends_on = [
    helm_release.aad-pod-identity
  ]
}

resource "null_resource" "messagebus" {
  provisioner "local-exec" {
    interpreter = ["PowerShell", "-Command"]
    command = "kubectl apply -f ./components/messagebus.yaml"
  }

  depends_on = [
    helm_release.aad-pod-identity
  ]
}

resource "null_resource" "twitter" {
  provisioner "local-exec" {
    interpreter = ["PowerShell", "-Command"]
    command = "kubectl apply -f ./components/twitter.yaml"
  }

  depends_on = [
    helm_release.aad-pod-identity
  ]
}

