---
name: run-dev-windows
description: Start the DiceHub backend (DH.Api) and frontend (DH.WebUI) dev servers, each in its own separate GUI terminal window, so the user can watch both live logs. Use whenever the user says "run be and fe in separate windows", "run backend and frontend in separate windows", "start the dev servers in windows/tabs", "run fe and be in separate terminals", or similar.
---

# Run BE + FE in separate terminal windows

Goal: two independent `gnome-terminal` windows on the user's desktop, one running
the API, one running `ng serve`, detached from this Claude session so they keep
running and the user watches each window's live output directly.

## 1. Preconditions (check first, one command)

```bash
echo "DISPLAY=$DISPLAY  DBUS=$DBUS_SESSION_BUS_ADDRESS"
command -v gnome-terminal || command -v x-terminal-emulator
ls DH.DiceHub/DH.Api/firebase-adminsdk.local.json          # backend needs this (or Firebase__CredentialsJson env)
(timeout 2 bash -c '</dev/tcp/localhost/5432' && echo PG_UP || echo PG_DOWN)   # local Postgres for the API
ss -ltnp 2>/dev/null | grep -E ':4200|:5000|:5001' || echo "ports free"
```

- No `$DISPLAY` / no terminal emulator → **cannot open windows**. Fall back:
  start both with `run_in_background: true` and give the user
  `tail -f <task-output-file>` commands for their own two tabs. Tell them why.
- `PG_DOWN` → the API will fail on boot (it migrates + seeds on startup). Tell
  the user to start Postgres; don't launch the backend window.
- Ports already in use → something's already running. Kill it (step 2) or, if the
  user just wants windows attached to the *existing* servers, skip launching that
  side.

## 2. Free the ports (only if 4200 / 5000 / 5001 are taken)

```bash
# find + kill whatever holds them, plus any stragglers this skill started before
ss -ltnp 2>/dev/null | grep -E ':4200|:5000|:5001'
pkill -f "dotnet run --no-launch-profile"
pkill -f "bin/Debug/net8.0/DH.Api"
pkill -f "ng serve"
sleep 2
ss -ltnp 2>/dev/null | grep -E ':4200|:5000|:5001' || echo "ports free"
```

If a previous invocation of this skill started them as **background tasks** of
this session, stop those tasks too (TaskStop / the /tasks list) so they don't
respawn or duplicate.

## 3. Launch the two windows

```bash
setsid gnome-terminal --title="DiceHub BACKEND (:5000)" \
  --working-directory=/home/mpavlov/repos/DiceHub/DH.DiceHub/DH.Api \
  -- bash -lc 'ASPNETCORE_ENVIRONMENT=Development dotnet run --no-launch-profile; echo; echo "=== backend exited ($?). Press Enter to close ==="; read' >/dev/null 2>&1 &

sleep 1

setsid gnome-terminal --title="DiceHub FRONTEND (:4200)" \
  --working-directory=/home/mpavlov/repos/DiceHub/DH.WebUI \
  -- bash -lc 'npm start; echo; echo "=== frontend exited ($?). Press Enter to close ==="; read' >/dev/null 2>&1 &

sleep 3
ps -eo pid,cmd | grep -E "gnome-terminal-server|dotnet run|npm start|ng serve" | grep -v grep
```

- `setsid ... &` detaches the windows from this session — they must outlive the
  turn. Do **not** use `run_in_background` here; that keeps them tied to the
  session and defeats the point.
- The trailing `; read` keeps each window open after its process dies so the
  user can read the crash.
- `--working-directory` + a `bash -lc` one-liner: the login shell loads the
  user's `nvm`/`dotnet` PATH the same way their real terminal does.

## 4. Report

- Backend ready when its window prints `Now listening on: http://localhost:5000`
  (only :5000 binds in Development; it migrates + seeds first, ~10–30 s).
- Frontend ready when its window prints `➜  Local:   http://localhost:4200/`
  (first compile ~30–60 s).
- Tell the user the two window titles and the two URLs. Nothing more to poll —
  the windows are theirs to watch.

## Notes

- Commands, verbatim: BE = `ASPNETCORE_ENVIRONMENT=Development dotnet run --no-launch-profile`
  in `DH.DiceHub/DH.Api`; FE = `npm start` (which is `ng serve`) in `DH.WebUI`.
- `npm run cap:sync` / `prod-build` are for device builds, not this — always use
  `npm start` for the live-reload dev window.
- If `gnome-terminal` is missing but `x-terminal-emulator` exists, swap the
  binary; the `--title` / `--working-directory` / `-- bash -lc` form is
  Debian-compatible via `x-terminal-emulator`.
