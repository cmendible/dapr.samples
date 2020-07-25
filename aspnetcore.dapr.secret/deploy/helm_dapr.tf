
resource "kubernetes_namespace" "dapr" {
  metadata {
    name = "dapr-system"
  }
}

data "helm_repository" "dapr" {
  name = "dapr"
  url  = "https://daprio.azurecr.io/helm/v1/repo"
}

resource "helm_release" "dapr" {
  name       = "dapr"
  chart      = "dapr/dapr"
  namespace  = "dapr-system"
  repository = data.helm_repository.dapr.metadata[0].name
}
