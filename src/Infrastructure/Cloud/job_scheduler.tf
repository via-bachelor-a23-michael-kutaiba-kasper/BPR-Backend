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

# Jobs
# resource "google_cloud_run_v2_job" "periodic_jobs" {
#   name     = "periodic-jobs"
#   location = var.gcp_region

#   template {
#     template {
#       containers {
#         image = "docker.io/michaelbui293886/vibeverse-httpjob"

#         env {
#           name = "JOBS"
#           value = jsonencode([
#             {
#               url    = "${module.scraper_service.service_url}/scrape"
#               method = "POST"
#               body   = { strategy = "faengslet" }
#             },
#             {
#               url    = "${module.eventmanagement_service.service_url}/api/v1/events/allPublicEvents"
#               method = "GET"
#             }
#           ])
#         }
#       }
#     }
#   }

#   lifecycle {
#     ignore_changes = [
#       launch_stage,
#     ]
#   }
# }

# # Scheduler
# resource "google_cloud_scheduler_job" "job" {
#   provider         = google
#   name             = "periodic-jobs-scheduler"
#   description      = "Periodically calls the periodic_jobs cloud run job"
#   schedule         = "*/15 * * * *"
#   attempt_deadline = "320s"
#   region           = var.gcp_region
#   project          = var.gcp_project_id

#   retry_config {
#     retry_count = 3
#   }
#   http_target {
#     http_method = "POST"
#     uri         = "https://${google_cloud_run_v2_job.periodic_jobs.location}-run.googleapis.com/apis/run.googleapis.com/v1/namespaces/${var.gcp_project_number}/jobs/${google_cloud_run_v2_job.periodic_jobs.name}:run"

#     oidc_token {
#       service_account_email = google_service_account.job_invoker.email
#     }
#   }

#   depends_on = [resource.google_project_service.cloudscheduler_api, resource.google_cloud_run_v2_job.periodic_jobs, google_cloud_run_v2_job_iam_binding.binding]
# }


# # IAM
# resource "google_service_account" "job_invoker" {
#   account_id   = "scheduler-sa"
#   description  = "Cloud Scheduler service account; used to trigger scheduled Cloud Run jobs."
#   display_name = "scheduler-sa"
#   project      = var.gcp_project_id
#   provider     = google
# }

# resource "google_project_iam_binding" "job_invoker_binding" {
#   project = var.gcp_project_id
#   role    = "roles/iam.serviceAccountTokenCreator"
#   members = ["serviceAccount:${google_service_account.job_invoker.email}"]
# }

# resource "google_cloud_run_v2_job_iam_binding" "binding" {
#   project    = var.gcp_project_id
#   location   = var.gcp_region
#   name       = google_cloud_run_v2_job.periodic_jobs.name
#   role       = "roles/viewer"
#   members    = ["serviceAccount:${google_service_account.job_invoker.email}"]
#   depends_on = [google_cloud_run_v2_job.periodic_jobs]
# }
