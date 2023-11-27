# GCP Infrastructure for VibeVerse backend

## Requirements

-   Terraform CLI
-   gcloud CLI

## Before deployment

1. Download GCP Service Account Key JSON file and store it somewhere, e.g. `~/Tools/gcp_vibeverse.json`
2. Authenticate to GCP by running:
   ` gcloud auth application-default login`
3. Set docker username environment variable: `export TF_VAR_DOCKER_USERNAME=michaelbui293886`
4. Set Service Account Key JSON env. var: `export TF_VAR_GCP_SERVICE_ACCOUNT_KEY_JSON=$(cat ~/Tools/gcp_vibeverse.json)`
5. Set Google API Key environment variable: `export TF_VAR_GOOGLE_API_KEY=YOUR_API_KEY`
6. Set Firestore service account key JSON environment variable: `export TF_VAR_SERVICE_ACCOUNT_KEY_FIREBASE_JSON=$(cat ~/Tools/gcp_vibeverse_firebase.json)`
7. Initialize backend + install custom modules by running `terraform init`
8. Check changes with `terraform plan`
9. Deploy changes with `terraform apply`
