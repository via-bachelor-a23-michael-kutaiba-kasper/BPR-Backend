# APIs to enable
resource "google_project_service" "cloudscheduler_api" {
  service            = "cloudscheduler.googleapis.com"
  disable_on_destroy = false
  project            = var.gcp_project_id
}
resource "google_project_service" "iam_api" {
  service            = "iam.googleapis.com"
  disable_on_destroy = false
}

# #Jobs handler
module "periodic_jobs" {
  source                       = "./modules/container-service"
  service_name                 = "periodic-jobs"
  image                        = "docker.io/${var.DOCKER_USERNAME}/vibeverse-httpjob"
  port                         = 4242
  gcp_service_account_key_json = var.GCP_SERVICE_ACCOUNT_KEY_JSON
  max_instances                = 1
}

# Scheduler
resource "google_cloud_scheduler_job" "job" {
  provider         = google
  name             = "periodic-jobs-scheduler"
  description      = "Periodically calls the periodic_jobs cloud run job"
  schedule         = "*/5 * * * *"
  attempt_deadline = "320s"
  region           = var.gcp_region
  project          = var.gcp_project_id

  retry_config {
    retry_count = 3
  }
  http_target {
    http_method = "POST"
    uri         = "${module.periodic_jobs.service_url}/trigger"
    # Add job endpoints here.
    body = base64encode(jsonencode([
      {
        url    = "${module.scraper_service.service_url}/scrape"
        method = "POST"
        body   = { strategy = "faengslet" }
      },
      {
        url    = "${module.eventmanagement_service.service_url}/api/v1/events/externalEvents"
        method = "GET"
      },
      {
        url    = "${module.eventmanagement_service.service_url}/api/v1/progress/exp"
        method = "POST"
      },
      {
        url    = "${module.eventmanagement_service.service_url}/api/v1/progress/achievements/processUserAchievements"
        method = "POST"
      }
    ]))
    headers = {
      "Content-Type" = "application/json"
    }
  }

  depends_on = [resource.google_project_service.cloudscheduler_api, google_project_iam_binding.binding]
}


# IAM
resource "google_service_account" "job_invoker" {
  account_id   = "scheduler-sa"
  description  = "Cloud Scheduler service account; used to trigger scheduled Cloud Run jobs."
  display_name = "scheduler-sa"
  project      = var.gcp_project_id
  provider     = google
}

resource "google_project_iam_binding" "binding" {
  project = var.gcp_project_id
  role    = "roles/run.invoker"
  members = ["serviceAccount:${google_service_account.job_invoker.email}"]
}
