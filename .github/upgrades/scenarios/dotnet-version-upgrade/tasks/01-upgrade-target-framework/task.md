# 01-upgrade-target-framework: Retarget the solution to .NET 10

The six projects in this repository are already on the SDK-style project format and the assessment found no incompatible NuGet packages or source/API migration blockers. The work is therefore confined to updating the project target frameworks together and validating the full solution in one pass.

This task updates the shared project graph from net8.0 to net10.0, including the library projects and both unit/integration test projects that reference them. After the retarget, the solution will be rebuilt and the relevant tests will be run to confirm there are no regressions from the framework change.

**Done when**: all project files in the solution target net10.0, the solution builds successfully, and the unit/integration tests pass without errors.
