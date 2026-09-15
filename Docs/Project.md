# Project

Documentation for this repository's own tooling and workflow.

## Targeting Godot

`Core/Core.csproj` targets `Godot.NET.Sdk` rather than `Microsoft.NET.Sdk` - the library's public
surface is built directly on Godot engine types (`Node`, `Resource`, `SceneTree`, ...), which only
`Godot.NET.Sdk` resolves. Building it produces a `.godot/` cache directory (gitignored, like
`bin/`/`obj/`) alongside the usual output.

`Godot.NET.Sdk` references `GodotSharpEditor` only when `Configuration == Debug` - `packages.lock.json`
(recorded from a Debug restore) therefore never matches a `Release` restore's package set, a known
NuGet lock file limitation with configuration-conditional package references. `build.yml`'s `Pack`
step (the only place that restores as `Release`) deliberately omits `-p:RestoreLockedMode=true` for
this reason; every other step restores as the default `Debug` and keeps locked-mode restore as
normal.

## Scripts

`Scripts/` holds file-based C# apps (`dotnet run Scripts/<Name>.cs`) for automated repository
actions.

- `Scripts/Test.cs` - runs the test suite with coverage collection and prints a summary.
- `Scripts/Verify.cs` - applies formatting fixes and regenerates the coverage badge.
- `Scripts/Publish.cs` - cuts a release (see Publishing below). A manual, human-only action.

## Publishing

Run `Scripts/Publish.cs` locally to cut a release:

1. It prompts for the version to publish (e.g. `1.2.3`) - `Core/Core.csproj` carries no `<Version>` of its
   own, so this is what actually gets built and published.
2. It creates the GitHub Release (and its underlying tag) for that version locally via `gh release
   create` - done locally because a repo ruleset blocks the default `GITHUB_TOKEN` from creating tags.
3. It dispatches `build.yml`'s `workflow_dispatch` trigger with the version as input. The workflow
   verifies the dispatcher has Admin permission on the repo, refuses to run from anything but `main`,
   packs `Core/Core.csproj`, pushes the package to GitHub Packages and nuget.org, and uploads the
   `.nupkg`/`.snupkg` as release assets.
4. It waits for that run to finish, rolling the release/tag back if the workflow fails, so a failed
   publish never leaves one behind. If it can't confirm the run happened at all, it leaves the release
   in place instead, rather than risk deleting one that's still running.

## Workflows

- `.github/workflows/build.yml` - builds, verifies formatting (`dotnet format --verify-no-changes`,
  never applies fixes), and runs tests on every push/PR to `main`; its `publish` job (see Publishing
  above) only runs on `workflow_dispatch`, gated on the dispatcher having Admin permission on the repo
  and the run being on `main`, and pushes the package to both GitHub Packages and nuget.org.
- `.github/workflows/codeql.yml` - CodeQL security analysis on push/PR to `main` and a weekly schedule.
