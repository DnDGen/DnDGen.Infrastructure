# Implementation Plan: README Documents the Public API

**Branch**: `001-readme-public-api` | **Date**: 2026-11-19 | **Spec**: [spec.md](spec.md)

## Summary

Rewrite the README so it meets the constitution's README as contract: what the package is, which
versions it needs, how to install it, how to load it, where tables live and their shape, how to
select from them, how to define a data selection, and a Reference section with a table per public
type. Add tests to the integration `ReadmeTests` that compare, by reflection, every public member of
every public type in `DnDGen.Infrastructure.dll` with the README's tables, and that exercise every
code sample. The tests follow DnDGen.RollGen's `ReadmeTests` (001), with the changes Infrastructure's
generics, extension methods, and abstract class need. Spec Kit setup is already committed.

## Technical Context

**Language/Version**: C# on `net10.0`

**Primary Dependencies**: NUnit 5 (tests only); Ninject and DnDGen.RollGen, which the samples use.

**Testing**: `DnDGen.Infrastructure.Tests.Integration`, which gains a copy of `README.md` in its
output, as RollGen's does. The tests run in the existing pipeline step.

**Project Type**: NuGet class library. The README is packed into the package by the existing
`<None Include="..\README.md">` item, so it must read well on NuGet as well as GitHub.

**Constraints**: No package code changes. No network. Samples run against the real package.

**Scale/Scope**: 16 public types today, found by reflection: `InfrastructureModuleLoader`,
`BindingExtensions`, `CollectionHelper`, `DataHelper`, `DataSelection<T>`,
`TypeAndAmountDataSelection`, `JustInTimeFactory`, `AssemblyLoader`, `CollectionMapper`,
`PercentileMapper`, and the six selector interfaces.

## Constitution Check

| Label | Check | Result |
|---|---|---|
| I.1 | The tests copy RollGen's `ReadmeTests` helpers (`GetLinesOutsideCodeBlocks`, `GetTableRows`, `WriteMember`) and table kinds (Constructor, Method, Property); the README's badge line and Installing section follow RollGen's and Stress's. | Pass |
| I.2 | Generic and `this` signatures are needed by Infrastructure's own members; they extend RollGen's signature writer rather than replace it. The embedded-sample check (D4) is new, justified by D4, and available to the other repositories. | Pass, with justification |
| I.3 | No comments added. | Pass |
| II | Tests are written first and seen to fail against today's README (T003, T005), before the README changes. | Pass |
| II.1.c | Constructors, methods, properties, and each sample get their own case. | Pass |
| II.2.a | The whole public API is checked, not a sample. | Pass |
| II.2.c (Infrastructure) | No package code changes, so coverage of `DnDGen.Infrastructure.dll` doesn't decrease. | Pass |
| II.3.a | Each member test compares the whole documented list with the whole public list, so a failure shows every missing and extra item at once. Samples compare whole results. | Pass |
| II.3.b | Everything a sample relies on, its tables and data selection, is in the test fixture, not implied. | Pass |
| II.3.c | The tests read only files in the build output and embedded resources; nothing depends on the machine. | Pass |
| III.1.c | The README's stale one-line description is replaced. | Pass |
| README as contract | This feature is the contract's test, and the constitution's README contract already describes it. | Pass |
| Versioning | PATCH, 8.0.1: README and tests only; release notes say 8.0.0 was the last version with code changes. | Pass |

## Decisions

- **D1: Signatures.** `WriteMember` writes `Name<T>(type name = default, ...)`, as RollGen's does,
  and adds what Infrastructure needs: `this` on an extension method's first parameter, generic
  types as C# writes them (`IEnumerable<T>`, `Dictionary<string, IEnumerable<string>>`,
  `Func<string[], T>`), arrays, `params`, and keyword type names. Generic constraints
  (`where T : DataSelection<T>, new()`) stay in the row's description (spec clarification).
- **D2: Found types.** `typeof(InfrastructureModuleLoader).Assembly.GetExportedTypes()` gives the
  public types, so a new type is checked without editing the tests.
- **D3: Grouped rows and generic types.** A generic interface's members are listed under its
  open type (`ICollectionDataSelector<T>`). As in RollGen, every code span in a row's first cell is
  a documented item, so overloads may share a row.
- **D4: Table samples are the real files.** The README shows a collection table and a percentile
  table as XML. Each is also embedded in the integration project, and a test compares the README's
  block, whole, with the embedded file, so the documented shape can't drift from a table the real
  mapper reads. The selection samples then read those tables through the real selectors.
- **D5: Table lookup is documented as it works.** A table is the embedded `.xml` resource whose
  name ends in `.<tableName>.xml` in the named assembly, wherever its folder; the README says so,
  since a consumer's folder layout is theirs.
- **D6: Constants are properties.** As in Stress and RollGen, fields and constants are listed in
  Property tables beside read-only properties. Infrastructure has few; the tables are omitted for
  types with none.
- **D7: Mappers and the assembly loader are extension points.** `CollectionMapper`,
  `PercentileMapper`, and `AssemblyLoader` are public on purpose, so a consumer who hand-rolls a
  selector can reuse or replace how tables are mapped and found. The README documents them as
  extension points, not as plumbing.

## Project Structure

### Documentation (this feature)

```text
specs/001-readme-public-api/
├── spec.md              # Feature specification
├── plan.md              # This file
├── checklists/
│   └── requirements.md  # Spec quality checklist
└── tasks.md             # Next: /speckit-tasks
```

### Repository changes

```text
README.md                                                      # Rewritten: overview, versions,
│                                                              # install, loading, tables,
│                                                              # selecting, data selections,
│                                                              # Reference tables per public type
DnDGen.Infrastructure/DnDGen.Infrastructure.csproj             # Version 8.0.1 and release notes
.specify/memory/constitution.md                                # Already updated
DnDGen.Infrastructure.Tests.Integration/
├── DnDGen.Infrastructure.Tests.Integration.csproj             # Copies README.md to the output;
│                                                              # embeds the README's two tables
├── ReadmeTests.cs                                             # Member tests, sample tests, table
│                                                              # shape tests
└── Tables/
    ├── Collections/ReadmeCollectionTable.xml                  # The README's collection table
    └── Percentiles/ReadmePercentileTable.xml                  # The README's percentile table
```

## Complexity Tracking

None. D4 is the one new pattern, justified under I.2.
