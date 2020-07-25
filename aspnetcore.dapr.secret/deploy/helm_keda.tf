
resource "kubernetes_namespace" "keda" {
  metadata {
    name = "keda"
  }
}

data "helm_repository" "keda" {
  name = "kedacore"
  url  = "https://kedacore.github.io/charts"
}


resource "helm_release" "keda" {
  name       = "keda"
  chart      = "kedacore/keda"
  namespace  = "keda"
  repository = data.helm_repository.keda.metadata[0].name
}
