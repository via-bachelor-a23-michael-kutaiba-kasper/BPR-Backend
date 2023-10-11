// NOTE: (mibui 2023-05-19) These will be outputted after provisioning a new service.
//                          We can pipe this value into other scripts, e.g. update our API Gateway 
//                          with the new service.
output "service_url" {
  value = google_cloud_run_v2_service.service.uri
}

output "service_name" {
  value = google_cloud_run_v2_service.service.name
}
