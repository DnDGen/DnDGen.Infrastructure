# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade the DnDGen.Infrastructure solution from .NET 8 to .NET 10 while preserving the existing SDK-style project structure.
**Scope**: Six SDK-style projects with a straightforward dependency graph and no incompatible packages or API blockers identified during assessment.

## Upgrade Options

| Option | Selected | Why |
|--------|----------|-----|
| Upgrade Strategy | All-at-Once | All projects are already SDK-style and share a single dependency chain, so retargeting the solution together is the lowest-risk path. |
| Target Framework | net10.0 | This matches the requested production target and the repository's current .NET 8 baseline. |

## Tasks

### 01-upgrade-target-framework: Retarget the solution to .NET 10

The six projects in this repository are already on the SDK-style project format and the assessment found no incompatible NuGet packages or source/API migration blockers. The work is therefore confined to updating the project target frameworks together and validating the full solution in one pass.

This task updates the shared project graph from net8.0 to net10.0, including the library projects and both unit/integration test projects that reference them. After the retarget, the solution will be rebuilt and the relevant tests will be run to confirm there are no regressions from the framework change.

**Done when**: all project files in the solution target net10.0, the solution builds successfully, and the unit/integration tests pass without errors.
