resource "azurerm_cognitive_account" "cognitive" {
  name                = "cognitive"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "CognitiveServices"

  sku_name = "S0"
}