# Pipeline: Execution Plans & Steps

In BuildHub, a build run is represented as an **Execution Plan** made of ordered **Execution Steps**.

These models live in:
- `CommandBuilder/Models/Execution/ExecutionPlan.cs`
- `CommandBuilder/Models/Execution/ExecutionStep.cs`
- `CommandBuilder/Models/Execution/ExecutionCommand.cs`

## ExecutionPlan

`ExecutionPlan` represents a reusable plan definition containing ordered steps.

Fields (from code):
- `Id: Guid`
- `Name: string`
- `Version: BuildVersion` (defaults to `1.1.1`)
- `Steps: List<ExecutionStep>`

## ExecutionStep

Each step represents a single unit of work in the plan.

Fields (from code):
- `Id: Guid`
- `Order: int` (lower executes first)
- `Name: string`
- `StepType: string` (plugin identifier, e.g. `Git`, `TFS`, `MSBuild`)
- `Commands: IReadOnlyList<ExecutionCommand>` (executed sequentially)
- `DependsOn: IReadOnlyList<Guid>` (future DAG support)
- `Metadata: object?` (typed plugin-specific info; currently placeholder classes exist)
- `RetryPolicy: RetryPolicy?`
- `TimeoutPolicy: TimeoutPolicy?`
- `EnvironmentVariables: IReadOnlyDictionary<string,string>?`

## ExecutionCommand

An execution step contains one or more commands:

- `Executable: string` (e.g. `dotnet`, `msbuild`, `git`)
- `Arguments: string`
- `WorkingDirectory: string?`

> Note: some builders currently populate only `Arguments` and rely on the runner to choose a default shell/executable.

## Policies

### RetryPolicy

- `MaxAttempts` (default `1`)
- `Delay`
- `RetryOn` (`FailuresOnly` / other values in `RetryOn` enum)

### TimeoutPolicy

- `Timeout`
- `KillProcessTree` (default `true`)

## Step types

There is an enum `StepType` with values:
- `Build`, `Script`, `File`, `Command`, `Source`

Today, `ExecutionStep.StepType` is a **string** used as a plugin identifier (builder name).
The enum can be used as a higher-level categorization.

## Example JSON

This is an example shape matching the current models:

```json
{
  "id": "b3f4d9b2-0c0b-4f7c-93ef-4a12d6f6c9b0",
  "name": "Windows Release Build",
  "version": { "major": 1, "minor": 2, "lastBuild": 45 },
  "steps": [
    {
      "id": "9e5c2f2a-8ce2-4b6b-a6d0-1c0c52a6b67d",
      "order": 1,
      "name": "Git",
      "stepType": "Git",
      "commands": [
        { "executable": "git", "arguments": "clone \"https://example/repo.git\" \"C:\\work\\repo\"" }
      ],
      "dependsOn": [],
      "retryPolicy": { "maxAttempts": 2, "delay": "00:00:05", "retryOn": "FailuresOnly" },
      "timeoutPolicy": { "timeout": "00:10:00", "killProcessTree": true },
      "environmentVariables": { "CI": "true" }
    }
  ]
}
```

## Planned: DAG execution

`DependsOn` exists already, so the plan can evolve from “ordered list” to “DAG scheduler” later:
- keep `Order` for simple pipelines
- use `DependsOn` for parallelism and fan-in/fan-out graphs


## Communication and orchestration (planned)

The **ExecutionPlan/Step/Command** models define *what to run*.
The communication pieces define *how it is executed and observed*.

- Plan creation: API compiles a plan using CommandBuilder.
- Dispatching: Scheduler/Dispatcher assigns the run to an agent.
- Delivery: Agent Gateway sends the plan/commands over WebSocket.
- Updates: agent streams step events + log chunks back to the gateway.
- UI updates: API forwards realtime events to UI via WebSocket (or UI polls).

See also:
- [Communication](./communication.md)
- [Planned services](./services.md)
