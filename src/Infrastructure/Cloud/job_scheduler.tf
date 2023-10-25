resource "google_project_service" "cloudscheduler_api" {
  service            = "cloudscheduler.googleapis.com"
  disable_on_destroy = false
  project            = var.gcp_project_id
}

resource "google_cloud_run_v2_job" "periodic_jobs" {
  name     = "periodic-jobs"
  location = var.gcp_region

  template {
    template {
      containers {
        image = "docker.io/michaelbui293886/vibeverse-httpjob"

        env {
          name  = "JOB_URL"
          value = module.scraper_service.service_url
        }

        env {
          name  = "JOB_METHOD"
          value = "POST"
        }

        env {
          name = "JOB_BODY"
          value = jsonencode({
            strategy = "faengslet"
          })
        }
      }
    }
  }

  lifecycle {
    ignore_changes = [
      launch_stage,
    ]
  }
}

resource "google_cloud_scheduler_job" "job" {
  provider         = google
  name             = "periodic-jobs-scheduler"
  description      = "Periodically calls the periodic_jobs cloud run job"
  schedule         = "*/15 * * * *"
  attempt_deadline = "320s"
  region           = var.gcp_region
  project          = var.gcp_project_id

  retry_config {
    retry_count = 3
  }
  http_target {
    http_method = "POST"
    uri         = "https://${google_cloud_run_v2_job.periodic_jobs.location}-run.googleapis.com/apis/run.googleapis.com/v1/namespaces/${var.gcp_project_number}/jobs/${google_cloud_run_v2_job.periodic_jobs.name}:run"
  }

  depends_on = [resource.google_project_service.cloudscheduler_api, resource.google_cloud_run_v2_job.periodic_jobs]
}

data "google_iam_policy" "no_auth_policy" {
  binding {
    role = "roles/run.invoker"
    members = [
      "allUsers"
    ]
  }
}

resource "google_cloud_run_v2_job_iam_policy" "periodic_jobs_iam_policy" {
  name     = google_cloud_run_v2_job.periodic_jobs.name
  project  = google_cloud_run_v2_job.periodic_jobs.project
  location = google_cloud_run_v2_job.periodic_jobs.location

  policy_data = data.google_iam_policy.no_auth_policy.policy_data
}
