variable "image" {
  description = "Container image to run"
  type        = string
}

variable "service_name" {
  description = "Name of the service. Should be in the format movie_information"
  type        = string
}

variable "gcp_region" {
  description = "Region to deploy to"
  type        = string
  default     = "europe-west1"
}

variable "port" {
  description = "Port that the service listens on"
  type        = number
}

variable "max_instances" {
  type        = number
  description = "Max number of instances of the server that can be spun up. Defaults to 1"
  default     = 1
}

variable "gcp_service_account_key_json" {
  type        = string
  sensitive   = true
  description = "Contents of the service account key JSON file. This is needed such that services that requires Firebase access can restore the file in production. This is more secure since we pass it by environment variable instead of baking it in the image itself"
  default     = ""
}

variable "gcp_service_account_key_path" {
  type        = string
  sensitive   = false
  description = "Allows for overwriting path to the GCP service account key in our services"
  default     = "./service-account-key.json"
}

variable "container_envs" {
  type = map(string)

  description = "Optional list of environment variables to pass to the service"
  sensitive   = false
  default     = {}
}
