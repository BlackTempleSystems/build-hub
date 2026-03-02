# CommandBuilder

The `CommandBuilder` project generates `ExecutionStep` objects from domain-specific “contexts”.
It is intended to be used by the API (or a pipeline service) to produce the final **Execution Plan** dispatched to agents.

Path: `CommandBuilder/`

## Core API

### ICommandBuilder

`CommandBuilderBase<TBuilder, TContext>` exposes:

- `Validate()` — runs core + derived validation
- `GenerateCommand()` — returns `BuildCommandResult`
  - calls `Validate()` (warning logged on validation failures)
  - times generation
  - returns `Steps: IReadOnlyList<ExecutionStep>` or an error message

### BuildCommandResult

Contains:
- `Success: bool`
- `BuilderName: string`
- `Steps: IReadOnlyList<ExecutionStep>`
- `BuildTime: TimeSpan`
- `ErrorMessage: string`

## Implemented builders (current)

| Builder | Status | Notes |
|---|---:|---|
| `GitCommandBuilder` | ✅ implemented | Generates Windows shell commands for clone/fetch/checkout/pull |
| `TfsCommandBuilder` | ✅ implemented | Builds `tf get ...` commands |
| `MSBuildCommandBuilder` | ✅ implemented | Generates `msbuild` args including configuration/platform/version |
| `IncrediBuildCommandBuilder` | ✅ implemented | Generates `BuildConsole` args; optional MSBuild args |
| `VisualStudioCommandBuilder` | ✅ implemented | Generates `devenv` args (build/rebuild, cfg/platform, target) |
| `DotnetCommandBuilder` | 🧱 stub | Not implemented yet |
| `NpmCommandBuilder` | 🧱 stub | Not implemented yet |

## Contexts

Contexts live in `CommandBuilder/Models/Contexts` and carry parameters required to generate commands.

Examples:

- `GitContext`
  - `Repository`, `Branch`, `Version?`, `TargetDirectory`, `Clean`
- `TfsContext`
  - `WorkspacePath`, `Changeset?`, `Recursive`, `Force`
- `MsBuildBasedContextBase`
  - `BuildType`, `Flavor`, `Platform`, `Target`
- `MsBuildContext`
  - inherits MSBuild base + `Version`
- `IncrediBuildContext`
  - inherits MSBuild base + `OutputFile`, `Version`, `VisualStudioVersion`, `UseMsBuild`

## Example usage (C#)

```csharp
var git = new GitCommandBuilder()
    .SetRepository("https://github.com/BlackTempleSystems/build-hub.git")
    .SetBranch("development")
    .SetTargetDirectory(@"C:\work\build-hub")
    .SetClean(false);

BuildCommandResult result = git.GenerateCommand();
if (!result.Success) throw new Exception(result.ErrorMessage);

IReadOnlyList<ExecutionStep> steps = result.Steps;
```

## Known sharp edges (current)

- Some builders use `new Guid()` instead of `Guid.NewGuid()` for step IDs (produces `Guid.Empty`).
- `GitCommandBuilder` currently uses Windows `cmd`-style statements (`rmdir`, `if not exist ...`) — it will not work as-is on Linux agents.
- Some builders set only `Arguments` and not `Executable` — the runner should define the default shell/executable behavior.

## Related

- [Pipeline models](pipeline.md)
- [Architecture](architecture.md)
