terraform {
  backend "gcs" {
    bucket = "vibeverse-tfstate-bucket"
    prefix = "terraform/state"
  }
}

# REMOTE STATE BACKEND  -------------------------------
resource "google_storage_bucket" "default" {
  name          = "vibeverse-tfstate-bucket"
  force_destroy = false
  location      = "EU"
  storage_class = "STANDARD"

  lifecycle {
    prevent_destroy = true
  }
  versioning {
    enabled = true
  }
}
