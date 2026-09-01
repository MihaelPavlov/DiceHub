---
name: deploy-api
description: Build and push the DH.Api backend Docker image using deploy.sh. Use whenever the user says "deploy.sh", "run deploy", "deploy the api", "deploy the backend", or asks for a new deploy tag.
---

# Deploy DH.Api (deploy.sh)

The deploy script builds the backend Docker image and pushes it to Docker Hub
with a unique timestamp tag. Its job is done when the pushed tag is reported
back to the user.

## Where it lives / where to run it

- The real script is **`DH.DiceHub/deploy.sh`** and it **must be run from the
  `DH.DiceHub/` directory** — it builds with `-f DH.Api/Dockerfile .`, so the
  build context is `DH.DiceHub/` and `DH.Api/Dockerfile` is resolved relative
  to it.
- There is an identical stray copy at `DH.WebUI/deploy.sh`. **Ignore it** — it
  references `DH.Api/Dockerfile`, which does not exist from `DH.WebUI/`.
- The `.sh` file has a second **PowerShell** block appended after the bash
  section. **Do not run `bash deploy.sh`** — bash chokes on the `$timestamp = ...`
  lines and re-runs `docker build`/`docker tag` with an empty version. Run the
  bash steps yourself instead (below).

## Steps

Run from `DH.DiceHub/`:

```bash
cd /home/mpavlov/repos/DiceHub/DH.DiceHub
VERSION="latest.$(date +%s)"
echo "TAG=$VERSION"
docker build -t dh-api -f DH.Api/Dockerfile .
docker tag dh-api mpavlov9905/dh-api:$VERSION
docker push mpavlov9905/dh-api:$VERSION
echo "Latest tag: $VERSION"
```

- Registry / repo: **`mpavlov9905/dh-api`** on Docker Hub. The host must already
  be `docker login`ed as `mpavlov9905` (check with `docker info | grep Username`).
- Tag format: `latest.<unix-timestamp>` (e.g. `latest.1787909846`). Each run
  produces a new one — never reuse a previous tag unless the same built image
  is still present locally and you are just retrying the push.
- The Dockerfile is a multi-stage .NET 8 build (`mcr.microsoft.com/dotnet/sdk:8.0`
  → `aspnet:8.0`), entrypoint `dotnet DH.Api.dll`. Build takes a few minutes.

## Notes

- `docker push` is an outbound publish and is gated by the permission
  classifier — it may need the user to approve it. If the build succeeded but
  the push was denied, keep the local `dh-api` image, give the user the
  `$VERSION` tag, and the exact `docker tag` + `docker push` commands so they
  can run it (or approve and retry — the built image is reused, same tag).
- Always end by reporting the pushed tag and image digest to the user.
- This deploys whatever is in the working tree. Confirm the intended backend
  changes are committed/present before building.
