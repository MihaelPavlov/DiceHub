## graphify

This project has a knowledge graph at graphify-out/ with god nodes, community structure, and cross-file relationships.

Rules:
- For codebase questions, first run `graphify query "<question>"` when graphify-out/graph.json exists. Use `graphify path "<A>" "<B>"` for relationships and `graphify explain "<concept>"` for focused concepts. These return a scoped subgraph, usually much smaller than GRAPH_REPORT.md or raw grep output.
- If graphify-out/wiki/index.md exists, use it for broad navigation instead of raw source browsing.
- Read graphify-out/GRAPH_REPORT.md only for broad architecture review or when query/path/explain do not surface enough context.
- After modifying code, run `graphify update .` to keep the graph current (AST-only, no API cost).

## Known Android issues

- **"App state resets / form data or a picked image is lost" on a physical Android test device** - before assuming this is an app bug, check the device's Developer Options → **"Don't keep activities"**. If enabled, Android destroys and recreates the app's Activity (and reloads the WebView from scratch) the instant it leaves the foreground, even for a fraction of a second - e.g. while a native image/file picker is briefly on top. This is 100% reproducible, happens in ~1-2 seconds, produces no crash/ANR/process-death signal in `adb logcat`, and looks exactly like a real bug until you check this one setting. It's a developer-only testing toggle, off by default, and essentially never enabled by real users - not something to design around in app code. Confirmed root cause via: `adb logcat` (no crash), a temporary `pagehide`/`beforeunload`/`window.onerror` listener in `main.ts` (confirmed a genuine page reload with no preceding JS error), and finally the device's own Developer Options screen.
- `@capacitor/camera` 7.0.5's Android `CameraPlugin.java` has a real, unresolved upstream NullPointerException in `openPhotos()` (`ActivityResultLauncher.unregister()` on a null reference - `ActivityResultRegistry.register()` can invoke its callback synchronously before the assignment completes). Patched via `patch-package` (`DH.WebUI/patches/@capacitor+camera+7.0.5.patch`, wired through a `postinstall` script) - don't re-diagnose this from scratch if a picker-related crash shows the same stack trace; check the patch first.
