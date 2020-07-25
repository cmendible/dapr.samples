variable resource_group_name {
  default = "dapr"
}

variable managed_identity_name {
  default = "dapr"
}

variable managed_identity_selector {
  default = "reads-vault"
}

variable location {
  default = "West Europe"
}

variable cluster_name {
  default = "dapr-vacd-aks"
}

variable "dns_prefix" {
  default = "dapr-vacd-aks"
}

variable "agent_count" {
  default = 3
}

variable key_vault_name {
  default = "dapr-vacd-kv"
}
