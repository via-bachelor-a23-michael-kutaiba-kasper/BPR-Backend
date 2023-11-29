# GCP Infrastructure for VibeVerse backend

## Requirements

-   Terraform CLI
-   gcloud CLI

## Before deployment

1. Download GCP Service Account Key JSON and Service account key firebase JSON file and store it somewhere, e.g. `~/Tools/gcp_vibeverse.json`
2. Authenticate to GCP by running:
   ` gcloud auth application-default login`
3. Set docker username environment variable: `export TF_VAR_DOCKER_USERNAME=michaelbui293886` for windows use $env: instead of export
4. Set Service Account Key JSON env. var: `export TF_VAR_GCP_SERVICE_ACCOUNT_KEY_JSON=$(cat ~/Tools/gcp_vibeverse.json)` $env: instead of export
5. Set Google API Key environment variable: `export TF_VAR_GCP_GOOGLE_API_KEY=YOUR_API_KEY` $env: instead of export
6. Set Firestore service account key JSON environment variable: `export TF_VAR_GCP_SERVICE_ACCOUNT_KEY_FIREBASE_JSON=$(cat ~/Tools/gcp_vibeverse_firebase.json)` $env: instead of export
7. Initialize backend + install custom modules by running `terraform init`
8. out comment this code from Cloud -> main.tf:

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
9. Check changes with `terraform plan`
10. Deploy changes with `terraform apply`
11. remove comments from main.tf
12. Check changes with `terraform plan`
13. Deploy changes with `terraform apply`
