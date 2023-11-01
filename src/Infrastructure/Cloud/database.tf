resource "google_sql_database_instance" "main" {
  name             = "vibeverse-postgres"
  database_version = "POSTGRES_15"
  region           = "europe-west3"

  settings {
    # Second-generation instance tiers are based on the machine
    # type. See argument reference below.
    tier = "db-f1-micro"
  }
  deletion_protection = false
}
resource "google_sql_database_instance" "test" {
  name             = "vibeverse-test-postgres"
  database_version = "POSTGRES_15"
  region           = "europe-west3"

  settings {
    # Second-generation instance tiers are based on the machine
    # type. See argument reference below.
    tier = "db-f1-micro"
  }
  deletion_protection = false
}

output "db_test_url" {
  value = google_sql_database_instance.test.public_ip_address
}


output "db_url" {
  value = google_sql_database_instance.main.public_ip_address
}
