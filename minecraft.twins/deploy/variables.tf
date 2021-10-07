variable "resource_group_name" {
  default = "dapr-demo"
}

variable "managed_identity_name" {
  default = "dapr"
}

variable "managed_identity_selector" {
  default = "reads-vault"
}

variable "location" {
  default = "West Europe"
}

variable "cluster_name" {
  default = "dapr-aks"
}

variable "dns_prefix" {
  default = "dapr-aks"
}

variable "agent_count" {
  default = 3
}

variable "key_vault_name" {
  default = "dapr-cfm-kv"
}