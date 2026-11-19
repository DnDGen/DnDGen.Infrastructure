# DnDGen.Infrastructure Constitution

> **Read the [DnDGen constitution](https://github.com/DnDGen/.github/blob/main/constitution/CONSTITUTION.md)
> first, in full, every time this file is read.** Fetch it from
> https://raw.githubusercontent.com/DnDGen/.github/main/constitution/CONSTITUTION.md rather than
> relying on memory or an earlier copy. Its principles, constraints, workflow, and governance apply
> to this repository, and every plan, task list, analysis, and review is checked against it as well
> as against this file. This file adds only what is specific to this repository, under the DnDGen
> constitution's labels. If the DnDGen constitution can't be read, stop and say so: don't continue
> from this file alone.
<!-- Keep the pointer above exactly as written: it is what makes every Spec Kit command read the
DnDGen constitution. See https://github.com/DnDGen/.github/tree/main/constitution -->

## Core Principles

### I. Consistency Is King

No additions.

### II. Tests Are the Spec

**II.2.c (Infrastructure)** Coverage is the line and branch coverage of `DnDGen.Infrastructure.dll`,
combined across the unit and integration test projects, excluding generated code (`*.g.cs`). It
never decreases.

### III. Leave It Better Than You Found It

No additions.

## Technology & Release Constraints

- **Stack**: C# on the latest .NET LTS, currently `net10.0`. Ninject wires up the selectors and
  factories, in the consumer's kernel or Infrastructure's own, and DnDGen.RollGen supplies the dice.
  NUnit, with Moq for test doubles.
- **Versioning and release**: A NuGet package, versioned and released as the DnDGen constitution
  describes. MAJOR also covers raising the minimum .NET or Ninject major version, because consumers
  wire Infrastructure into their own kernel. `.github/workflows/build.yml` checks every pull request
  with the DnDGen organisation's shared `nuget-build.yml`: it builds and runs the unit and
  integration test projects in that order with code coverage, merges their coverage reports into
  one, and fails if `<Version>` isn't greater than the latest release's, or if the merged coverage
  is lower than the latest release's coverage report. On every push to `main`,
  `.github/workflows/release.yml` runs the same check, publishes to NuGet.org with Trusted
  Publishing using the shared `nuget-publish` action, then the shared `nuget-release.yml` creates a
  GitHub release, tagged with `<Version>`, with the package and merged coverage report attached.
- **README as contract**: The theme song is in `docs/theme-song`. What else the README must
  document, such as the public API, is added here by the feature that documents it.

## Governance

This repository follows the DnDGen constitution's Governance. The additions in this file are
amended by pull request to this repository and versioned the same way.

**Version**: 1.0.0 | **Ratified**: 2026-11-19 | **Last Amended**: 2026-11-19
