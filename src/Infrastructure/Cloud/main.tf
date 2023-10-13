variable "docker_username" {
  type        = string
  sensitive   = false
  description = "Name of the docker hub account that hosts the container images"
}

variable "gcp_service_account_key_json" {
  type        = string
  sensitive   = true
  description = "Contents of the service_account_key.json file to be passed to container images"
}

module "api_gateway" {
  source                       = "./modules/container-service"
  service_name                 = "api-gateway"
  image                        = "docker.io/${var.docker_username}/vibeverse-gateway"
  port                         = 4242
  gcp_service_account_key_json = var.gcp_service_account_key_json
  max_instances                = 1
  container_envs = {
    "QUERY_EVENTS_URL" = "TEST"
    "QUERY_EVENT_URL"  = "TEST2"
  }
}
