# GitTools

> A .NET CLI tool and Git abstraction library for streamlined Git workflows with GitHub and GitLab integration.

[![Build](https://github.com/CreativeCodersTeam/GitTools/actions/workflows/main.yml/badge.svg)](https://github.com/CreativeCodersTeam/GitTools/actions/workflows/main.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/CreativeCoders.GitTool)](https://www.nuget.org/packages/CreativeCoders.GitTool)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

[Features](#features) | [Getting started](#getting-started) | [Usage](#usage) | [Configuration](#configuration) | [Architecture](#architecture) | [Building from source](#building-from-source)

## Overview

GitTools provides the `gt` CLI tool and a set of .NET libraries that simplify everyday Git workflows. It wraps [LibGit2Sharp](https://github.com/libgit2/libgit2sharp) in a clean abstraction layer and adds high-level commands for managing feature branches, releases, tags, and more — with first-class support for both **GitHub** and **GitLab**.

## Features

- **Feature workflow** — start and finish feature branches following a GitFlow-style process
- **Branch management** — list, inspect, pull, push, and update all permanent branches at once
- **Release management** — create versioned releases via Git tags with optional auto-increment
- **Tag management** — create, delete, fetch, and list tags locally and remotely
- **GitHub integration** — automatic pull request creation via [Octokit](https://github.com/octokit/octokit.net)
- **GitLab integration** — automatic merge request creation via [GitLabApiClient](https://github.com/nmklotas/GitLabApiClient)
- **Credential support** — seamless authentication through [Git Credential Manager](https://github.com/git-ecosystem/git-credential-manager)
- **Git abstraction layer** — clean interfaces (`IGitRepository`, `IGitRepositoryFactory`, ...) for testable Git operations
- **Per-repository configuration** — JSON-based configuration with sensible defaults

## Getting started

1. Install the tool:
   ```bash
   dotnet tool install --global CreativeCoders.GitTool
   ```
2. Navigate to any Git repository:
   ```bash
   cd your-repo
   ```
3. Start your first feature:
   ```bash
   gt feature start my-feature
   ```

> [!IMPORTANT]
> Requires [.NET 10 SDK](https://dotnet.microsoft.com/download) or later and Git installed on your machine.
> [Git Credential Manager](https://github.com/git-ecosystem/git-credential-manager) is recommended for authentication.

## Usage

Run `gt --help` to see all available commands.

### Feature branches

```bash
# Start a new feature branch (branches off develop or main)
gt feature start <feature-name>

# Finish the feature: merge, push, and open a pull/merge request
gt feature finish [<feature-name>]
```

### Branch commands

```bash
gt branch list                # List all local and remote branches
gt branch info                # Show details about the current branch
gt branch pull                # Pull the current branch from remote
gt branch push                # Push the current branch to remote
gt branch update              # Pull all permanent local branches (main, develop, ...)
```

### Release commands

```bash
gt release create 1.2.0              # Create a release tag v1.2.0
gt release create --increment minor  # Auto-increment the minor version
gt release list-versions             # List all version tags
```

### Tag commands

```bash
gt tag create <name>          # Create a new tag
gt tag delete <name>          # Delete a tag locally (and optionally on remote)
gt tag fetch                  # Fetch all tags from remote
gt tag list                   # List all tags
```

### Other commands

```bash
gt showconfig                 # Show the effective configuration for the current repository
```

## Configuration

The tool reads its configuration from a JSON file located at:

| Platform | Path |
|----------|------|
| Windows  | `%LOCALAPPDATA%\CreativeCoders\GitTool\gt.json` |
| macOS    | `~/Library/Application Support/CreativeCoders/GitTool/gt.json` |
| Linux    | `~/.local/share/CreativeCoders/GitTool/gt.json` |

<details>
<summary>Example configuration</summary>

```json
{
  "tool": {
    "defaultGitServiceProviderName": "github"
  },
  "GitServiceProviders": {
    "GitHub": {
      "Hosts": ["github.com"]
    },
    "GitLab": {
      "Hosts": ["gitlab.com"]
    }
  }
}
```

Per-repository configuration files (prefix `repo_`) in the same folder allow repository-specific overrides such as a custom develop branch name, feature branch prefix, or disabling TLS certificate validation for self-hosted instances.

</details>

<details>
<summary>Repository configuration defaults</summary>

| Setting | Default |
|---------|---------|
| `featureBranchPrefix` | `feature/` |
| `developBranch` | `develop` |
| `hasDevelopBranch` | `true` |
| `gitServiceProviderName` | `github` |
| `disableCertificateValidation` | `false` |

</details>

## Git abstraction library

Beyond the CLI, GitTools ships a set of libraries that provide a clean, testable abstraction over [LibGit2Sharp](https://github.com/libgit2/libgit2sharp). You can reference these projects directly if you want to build your own Git tooling on top of them.

Register the services via dependency injection:

```csharp
services.AddGit();
```

Then inject `IGitRepositoryFactory` to open repositories:

```csharp
public class MyService(IGitRepositoryFactory repoFactory)
{
    public void DoWork()
    {
        using var repo = repoFactory.OpenRepository("/path/to/repo");
        var branches = repo.Branches;
        // ...
    }
}
```

The abstraction layer exposes interfaces like `IGitRepository`, `IGitRepositoryFactory`, and `IGitRepositoryUtils` — making it straightforward to mock Git operations in tests.

## Architecture

<details>
<summary>Project structure</summary>

```
source/
├── Git/
│   ├── CreativeCoders.Git.Abstractions                # Interfaces & models for Git operations
│   ├── CreativeCoders.Git                             # LibGit2Sharp-based implementation
│   └── CreativeCoders.Git.Auth.CredentialManagerCore  # GCM credential provider
└── GitTool/
    ├── CreativeCoders.GitTool.Base              # Shared base types, configuration, return codes
    ├── CreativeCoders.GitTool.Cli.Commands      # All CLI command implementations
    ├── CreativeCoders.GitTool.Cli.GtApp         # Entry point / host setup (the `gt` executable)
    ├── CreativeCoders.GitTool.GitHub            # GitHub service provider (Octokit)
    └── CreativeCoders.GitTool.GitLab            # GitLab service provider (GitLabApiClient)
```

</details>

## Building from source

```bash
# Restore and build
dotnet build GitTools.sln

# Run all tests
dotnet test GitTools.sln

# Pack the tool locally
dotnet pack source/GitTool/CreativeCoders.GitTool.Cli.GtApp/CreativeCoders.GitTool.Cli.GtApp.csproj
```

> [!TIP]
> Found a bug or have a question? [Open an issue](https://github.com/CreativeCodersTeam/GitTools/issues).
