# Couch CLI

A small convenience CLI tool for development purposes.

# Compile a new version of the tool

Go must be installed to compile this tool

To build binary for your own platform run:

```bash
$ go build -o ./couch-cli.exe ./main.go
```

It will produce an artifact in ./couch-cli.exe

To build binaries for both Mac OSX and Windows, run the `create-binaries.sh` script.
If you are on windows, use WSL to run it, since it requires bash.

```
$ ./create-binaries.sh
```

It will produce 2 artifacts at the project root dir:

-   `BPR-Backend/couch-cli.exe`
-   `BPR-Backend/couch-cli-mac`

# Usage

The tool itself is stored in the root of the the repository. The tool assumes that the repository root directory is called `BPR-Backend`, so please rename your directory if it is named something else.

## Help

List of commands can be seen by running:

```bash
$ couch-cli --help
```

## Create new service

```bash
$ couch-cli new service MyService
```

If your service requires a Postgres database, then use the `--with-dbup` flag

```console
$ couch-cli new Service MyService --with-dbup
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

-   ServiceName - API - Application - Domain - Infrastructure - .env
    In the `.env` file you may have specified `TMDB_API_KEY` or `GCP_SERVICE_ACCOUNT_KEY`

Now you can simply run

```bash
$ couch-cli start ServiceName
```

and it will detect the .env file and set all the environment variables for you.
