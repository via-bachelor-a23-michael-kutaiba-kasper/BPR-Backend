# Couch CLI
A small convenience CLI tool for development purposes. 

# Compile a new version of the tool
Go must be installed to compile this tool

To build run: 
```bash
$ go build
```

It will produce an artifact in ./couch-cli.exe

# Usage
The tool itself is stored in the root of the Couch Potates repository.

## Help
List of commands can be seen by running:
```bash
$ couch-cli --help
```

## Create new service
```bash
$ couch-cli new service MyService
```

## Start containers based on all docker compose files
The flags '-d' (detached) and '-b' (build) are optional. Consider always using '-d' and only using '-b' after making changes to a service.
```bash
$ couch-cli compose up -d -b
```

## Stop containers based on all docker compose files
```bash
$ couch-cli compose down
```

## Start service 
This is a convenient way to start a microservice with the necessary environment variables

Image this scenario. You have the following folder structure: 

- ServiceName
    - API
    - Application
    - Domain
    - Infrastructure
    - .env
In the `.env` file you may have specified `TMDB_API_KEY` or `GCP_SERVICE_ACCOUNT_KEY`

Now you can simply run
```bash
$ couch-cli start ServiceName
```

and it will detect the .env file and set all the environment variables for you.

