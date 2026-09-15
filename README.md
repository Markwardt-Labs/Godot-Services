# Godot Services

[![NuGet](https://img.shields.io/nuget/v/Markwardt.GodotServices.svg?label=NuGet)](https://www.nuget.org/packages/Markwardt.GodotServices)
[![License: MIT](https://img.shields.io/github/license/Markwardt-Labs/Godot-Services.svg)](LICENSE)
[![Build](https://github.com/Markwardt-Labs/Godot-Services/actions/workflows/build.yml/badge.svg)](https://github.com/Markwardt-Labs/Godot-Services/actions/workflows/build.yml)
[![Coverage](https://raw.githubusercontent.com/Markwardt-Labs/Godot-Services/main/.github/badges/badge_linecoverage.svg)](https://github.com/Markwardt-Labs/Godot-Services/actions/workflows/build.yml)

Generalized dependency injection, asset-loading, and engine-adapter infrastructure for Godot
Engine C# projects, built on `Microsoft.Extensions.DependencyInjection` and `Godot.NET.Sdk`.

## Installing

```sh
dotnet add package Markwardt.GodotServices
```

## Getting started

```csharp
// Marks this assembly so ServiceManager finds its convention-named services automatically.
[assembly: ConventionScannable]

// Set this as the script on an autoload node - ServiceManager handles the rest. Override
// Configure for anything convention scanning can't cover, like a keyed asset registration.
public partial class GameServices : ServiceManager
{
    protected override void Configure(IServiceCollection services) =>
        services.AddScene(SceneKeys.MainMenu, "res://Scenes/MainMenu.tscn");
}

// Any convention-named service (Thing implementing IThing) is now registered automatically.
public partial class Player : CharacterBody2D
{
    [Inject]
    public required IScoreKeeper ScoreKeeper { get; init; }
}
```

## Documentation

| File | Covers |
|------|--------|
| [Docs/Api.md](Docs/Api.md) | The public API's design and flow |
| [Docs/Usage.md](Docs/Usage.md) | Usage examples for common scenarios |
| [Docs/Architecture.md](Docs/Architecture.md) | The high-level design decisions behind the library |
| [Docs/Components/](Docs/Components/) | Design/implementation detail for individual complex components, one file each |
| [Docs/Testing.md](Docs/Testing.md) | How this repository's own tests are written, and why |
| [Docs/Project.md](Docs/Project.md) | This repository's own tooling and workflow: `Scripts/`, publishing, CI |
