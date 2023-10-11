terraform {
  required_providers {
    google = {
      source = "hashicorp/google"
    }
  }
}


resource "google_cloud_run_v2_service" "service" {
  name     = var.service_name
  location = var.gcp_region
  ingress  = "INGRESS_TRAFFIC_ALL"

  template {
    scaling {
      max_instance_count = var.max_instances
    }
    containers {
      image = var.image
      env {
        name  = "TMDB_API_KEY"
        value = var.tmdb_api_key
      }

      env {
        name  = "GCP_SERVICE_ACCOUNT_KEY_JSON"
        value = var.gcp_service_account_key_json
      }

      env {
        name  = "GCP_SERVICE_ACCOUNT_KEY"
        value = var.gcp_service_account_key_path
      }
      ports {
        container_port = var.port
      }
    }
  }
}

// NOTE: (mibui 2023-05-19) We are using a no auth policy since the API will exposed as public APIs
data "google_iam_policy" "no_auth_policy" {
  binding {
    role = "roles/run.invoker"
    members = [
      "allUsers"
    ]
  }
}

resource "google_cloud_run_v2_service_iam_policy" "service_iam_policy" {
  name     = google_cloud_run_v2_service.service.name
  project  = google_cloud_run_v2_service.service.project
  location = google_cloud_run_v2_service.service.location

  policy_data = data.google_iam_policy.no_auth_policy.policy_data
}
