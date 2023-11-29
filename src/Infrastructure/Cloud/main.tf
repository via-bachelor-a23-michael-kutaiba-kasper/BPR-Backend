variable "DOCKER_USERNAME" {
  type        = string
  sensitive   = false
  description = "Name of the docker hub account that hosts the container images"
}

variable "GCP_SERVICE_ACCOUNT_KEY_JSON" {
  type        = string
  sensitive   = true
  description = "Contents of the service_account_key.json file to be passed to container images"
}

variable "GCP_SERVICE_ACCOUNT_KEY_FIREBASE_JSON" {
  type        = string
  sensitive   = true
  description = "Contents of the service_account_key_firebase.json file to be passed to container images"
}

variable "GOOGLE_API_KEY" {
  type        = string
  sensitive   = true
  description = "Google Maps Platform API Key"
}

module "api_gateway" {
  source                       = "./modules/container-service"
  service_name                 = "api-gateway"
  image                        = "docker.io/${var.DOCKER_USERNAME}/vibeverse-gateway"
  port                         = 4242
  gcp_service_account_key_json = var.GCP_SERVICE_ACCOUNT_KEY_JSON
  max_instances                = 1
  container_envs = {
    "QUERY_EVENTS_HOST"          = module.eventmanagement_service.service_url
    "QUERY_EVENT_HOST"           = module.eventmanagement_service.service_url
    "QUERY_ALLPUBLICEVENTS_HOST" = module.eventmanagement_service.service_url
    "QUERY_JOINEVENT_HOST"       = module.eventmanagement_service.service_url
    "QUERY_CREATEEVENT_HOST"     = module.eventmanagement_service.service_url
    "QUERY_CATEGORIES_HOST"      = module.eventmanagement_service.service_url
    "QUERY_KEYWORDS_HOST"        = module.eventmanagement_service.service_url
  }
  cloud_sql_instance = google_sql_database_instance.main.connection_name
}

module "eventmanagement_service" {
  source                       = "./modules/container-service"
  service_name                 = "event-management"
  image                        = "docker.io/${var.DOCKER_USERNAME}/vibeverse-eventmanagementservice"
  port                         = 80
  gcp_service_account_key_json = var.GCP_SERVICE_ACCOUNT_KEY_JSON
  max_instances                = 1
  cloud_sql_instance           = google_sql_database_instance.main.connection_name

  container_envs = {
    "GOOGLE_API_KEY"                    = var.GOOGLE_API_KEY
    "SERVICE_ACCOUNT_KEY_FIREBASE_JSON" = var.GCP_SERVICE_ACCOUNT_KEY_FIREBASE_JSON
    "DEPLOYMENT_ENVIRONMENT"            = "PRODUCTION"
  }
}

module "scraper_service" {
  source                       = "./modules/container-service"
  service_name                 = "scraper"
  image                        = "docker.io/${var.DOCKER_USERNAME}/vibeverse-scraperservice"
  port                         = 80
  gcp_service_account_key_json = var.GCP_SERVICE_ACCOUNT_KEY_JSON
  max_instances                = 1
  cloud_sql_instance           = google_sql_database_instance.main.connection_name

  container_envs = {
    "GCP_PROJECT"    = var.gcp_project_id
    "GCP_TOPIC_NAME" = "vibeverse_events_scraped"
  }
}
