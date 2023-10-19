variable "docker_username" {
  type        = string
  sensitive   = false
  description = "Name of the docker hub account that hosts the container images"
}

variable "GCP_SERVICE_ACCOUNT_KEY_JSON" {
  type        = string
  sensitive   = true
  description = "Contents of the service_account_key.json file to be passed to container images"
}

# module "api_gateway" {
#   source                       = "./modules/container-service"
#   service_name                 = "api-gateway"
#   image                        = "docker.io/${var.docker_username}/vibeverse-gateway"
#   port                         = 4242
#   gcp_service_account_key_json = var.GCP_SERVICE_ACCOUNT_KEY_JSON
#   max_instances                = 1
#   container_envs = {
#     "QUERY_EVENTS_URL" = module.eventmanagement_service.service_url
#     "QUERY_EVENT_URL"  = module.eventmanagement_service.service_url
#   }
# }

# module "eventmanagement_service" {
#   source                       = "./modules/container-service"
#   service_name                 = "event-management"
#   image                        = "docker.io/${var.docker_username}/vibeverse-eventmanagementservice"
#   port                         = 80
#   gcp_service_account_key_json = var.GCP_SERVICE_ACCOUNT_KEY_JSON
#   max_instances                = 1
# }

# module "scraper_service" {
#   source                       = "./modules/container-service"
#   service_name                 = "scraper"
#   image                        = "docker.io/${var.docker_username}/vibeverse-scraperservice"
#   port                         = 80
#   gcp_service_account_key_json = var.GCP_SERVICE_ACCOUNT_KEY_JSON
#   max_instances                = 1

#   container_envs = {
#     "GCP_PROJECT"    = var.gcp_project_id
#     "GCP_TOPIC_NAME" = "test"
#   }
# }
