#!/bin/bash

VERSION="latest.$(date +%s)" # unique tag using timestamp

echo "Building..."
docker build -t dh-api -f DH.Api/Dockerfile .

echo "Tagging..."
docker tag dh-api mpavlov9905/dh-api:$VERSION

echo "Pushing..."
docker push mpavlov9905/dh-api:$VERSION

echo "Latest tag: $VERSION"

# Powershell

$timestamp = [int][double]::Parse((Get-Date -UFormat %s))
$version = "latest.$timestamp"

Write-Host "Building..."
docker build -t dh-api -f DH.Api/Dockerfile .

Write-Host "Tagging..."
docker tag dh-api mpavlov9905/dh-api:$version

Write-Host "Pushing..."
docker push mpavlov9905/dh-api:$version

Write-Host "✅ Latest tag pushed: $version"
