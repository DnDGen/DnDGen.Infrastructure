# .NET Version Upgrade

## Strategy
**Selected**: All-at-Once
**Rationale**: All six projects are already SDK-style, target modern .NET 8, and the assessment shows no incompatible packages or API blockers, so a single retarget is the lowest-risk path.

### Execution Constraints
- Upgrade all projects in one pass to keep dependency graph consistent.
- Build the full solution after retargeting before running the test projects.
- Validate the solution with unit/integration tests after the framework change is in place.

## Preferences
- **Flow Mode**: Automatic
- **Target Framework**: net10.0

## Source Control
- **Source Branch**: main
- **Working Branch**: upgrade-dotnet-10
- **Commit Strategy**: After Each Task
- **Branch Sync**: Auto (Merge)
