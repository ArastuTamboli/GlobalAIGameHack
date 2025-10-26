# Copilot / AI Agent Quick Win Notes — GlobalAIGameHack

Short, actionable instructions to help an AI agent be productive in this Unity repo.

## Quick context
- Project type: Unity (2020–2022 style project layout)
- Main AI integration: Neocortex (voice, transcription, assistant behaviors)
- Key scripts: `Assets/Scripts/VoiceCommandController.cs`, `Assets/Scripts/TowerManager.cs`, `Assets/Scripts/CameraController.cs`

## SDK install (must-read)
- This repo expects the Neocortex Unity SDK to be installed via the Unity Package Manager.
- `Packages/manifest.json` already contains:
  `"link.neocortex.sdk": "https://github.com/neocortex-link/neocortex-unity-sdk.git"`

If Neocortex types (e.g. `using Neocortex;`) are unresolved, do the following:
1. Open the project in the Unity Editor (Package Manager resolves git packages on open).
2. If it does not auto-install, open Window → Package Manager → Click + → Add package from git URL...
   Paste: `https://github.com/neocortex-link/neocortex-unity-sdk.git`
3. If the repo is private or your network blocks git, use a local package fallback:
   - Clone the SDK next to this project or copy its folder into `Packages/link.neocortex.sdk`
   - Change the manifest entry to a local reference: `"link.neocortex.sdk": "file:Packages/link.neocortex.sdk"`

## Quick troubleshooting steps (Windows PowerShell)
- Check for package cache files mentioning neocortex:
```powershell
Get-ChildItem -Path . -Recurse -Force -ErrorAction SilentlyContinue |
  Where-Object { $_.Name -match 'neocortex' } | Select-Object FullName
```
- Inspect the manifest and lockfile for the neocortex entry:
```powershell
Get-Content .\Packages\manifest.json | Select-String 'neocortex'
Get-Content .\Packages\packages-lock.json | Select-String 'neocortex'
```
- If Package Manager failed, check the Unity Editor log: `%LOCALAPPDATA%\Unity\Editor\Editor.log`

## Project-specific patterns an agent should follow
- Voice flow: `AudioReceiver` → `NeocortexSmartAgent` → transcription/response events → `VoiceCommandController` handlers.
  See `Assets/Scripts/VoiceCommandController.cs` for the event wiring.
- Tower placement: `TowerManager.PlaceTower(string towerType, Vector3 position)` stores towers in a `Dictionary<Vector3, GameObject>` and tracks counts per type.
- WebGL support: there are WebGL template helpers under `Assets/WebGLTemplates/Neocortex` (e.g. `WebMicrophone.jslib`) — keep these in sync with SDK web shims if you modify audio glue.

## What to change in code vs what to ask a human
- You can add commands/command-matching logic inside `VoiceCommandController` safely.
- Do NOT change package resolution in `manifest.json` without confirming network/auth — prefer documenting the local fallback first.

## Files to reference when modifying AI/voice behavior
- `Assets/Scripts/VoiceCommandController.cs` — event handlers + action dispatch
- `Assets/Resources/Neocortex/NeocortexSettings.asset` — runtime config values the SDK reads
- `Packages/manifest.json` — package source (git URL) for the SDK
- `Assets/WebGLTemplates/Neocortex/TemplateData/WebMicrophone.js` — WebGL microphone glue

If anything is unclear about how the SDK should be installed in your environment (private repo, access tokens, local copy), tell me how you'd like to proceed and I can either add a local manifest change or expand these instructions.
