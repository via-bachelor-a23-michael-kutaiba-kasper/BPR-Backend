# GCP Infrastructure for VibeVerse backend

## Requirements

-   Terraform CLI
-   gcloud CLI

## Before deployment

1. Download GCP Service Account Key JSON file and store it somewhere, e.g. `~/Tools/gcp_vibeverse.json`
2. Authenticate to GCP by running:
   ` gcloud auth application-default login`
3. Set docker username environment variable: `export TF_VAR_docker_username=michaelbui293886`
4. Set Service Account Key JSON env. var: `export TF_VAR_gcp_service_account_key_json=$(cat ~/Tools/gcp_vibeverse.json)`
5. Initialize backend + install custom modules by running `terraform init`
6. Check changes with `terraform plan`
7. Deploy changes with `terraform apply`
