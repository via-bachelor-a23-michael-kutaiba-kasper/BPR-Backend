#!/bin/bash

# Clean previous binaries

echo "CLeaning binaries..."

MAC_FILE=../../couch-cli-mac
WINDOWS_FILE=../../couch-cli.exe

if [[ -f "$MAC_FILE" ]]; then
    rm -f "$MAC_FILE"
fi

if [[ -f "$WINDOWS_FILE" ]]; then
    rm -f "$WINDOWS_FILE"
fi

# Building binary for Mac M2 (arm64)
echo "Building Mac OSX target"
export GOOS=darwin
export GOARCH=arm64

go build -o ../../couch-cli-mac ./main.go
chmod +x "$MAC_FILE"


# Building binary for Windows (amd64)
echo "Building Windows target"
export GOOS=windows
export GOARCH=amd64

go build -o ../../couch-cli.exe ./main.go

echo "Binaries are now available at the project root directory."