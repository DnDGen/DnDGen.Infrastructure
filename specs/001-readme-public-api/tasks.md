---

description: "Task list for the README documenting the public API"
---

# Tasks: README Documents the Public API

**Input**: Design documents from `specs/001-readme-public-api/`

**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md)

**Tests**: The tests are the feature. They are written first and seen to fail for the expected
reason before the README changes that make them pass (II).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- **(maintainer)**: Only the maintainer can do this task. `/speckit-implement` stops and hands it
  over.

## Phase 1: Setup

- [X] T001 Copy `README.md` to the output of `DnDGen.Infrastructure.Tests.Integration`, as
  DnDGen.RollGen's test project does
- [X] T002 [P] Add `Tables/Collections/ReadmeCollectionTable.xml` and
  `Tables/Percentiles/ReadmePercentileTable.xml` to the integration project as embedded resources:
  the two small tables the README will show (plan D4)

## Phase 2: Tests

- [X] T003 (maintainer) Approve the test list, written as the consumer's outcomes:
  1. **Members** (US2), each comparing the whole documented list with the whole public list, so one
     failure shows every missing and extra item:
     - `Constructors_AreAllDocumented`: if I add, remove, or change a public constructor, I expect
       the README's Constructor tables to list exactly the constructors there are.
     - `Methods_AreAllDocumented`: the same for every public method, by full signature: type
       parameters, parameter types, names, `this` and `params`, and defaults.
     - `Properties_AreAllDocumented`: the same for public properties, fields, and constants.
  2. **Samples** (US3), each running the README's code against the real package:
     - `LoadModules`: loading the modules into a kernel, twice, gives me a selector.
     - `CollectionSelection`: `SelectFrom`, `SelectAllFrom`, `IsCollection`, `FindCollectionOf`, and
       both `SelectRandomFrom` overloads over the README's collection table give what the README
       says.
     - `PercentileSelection`: `SelectFrom`, `SelectAllFrom`, and `SelectRandomFrom` over the
       README's percentile table do the same.
     - `TypeAndAmountSelection`: the typed-and-amount selectors over the README's tables do the same.
     - `DataSelectionSelection`: defining a data selection, binding it, and selecting it from a
       table gives my type back.
     - `DataHelperParsing`: `DataHelper`'s parse overloads round-trip the README's example,
       including a custom separator.
     - `JustInTimeFactory`: building by type and by name gives me what the README says.
     - `HandRolledSelector`: a selector built on the README's mapper sample reads the table.
  3. **Tables** (D4):
     - `CollectionTable_ReadmeMatchesEmbeddedFile` and `PercentileTable_ReadmeMatchesEmbeddedFile`:
       the XML the README shows is, whole, the embedded file the samples read.
- [X] T004 [US2] Write the three member tests in
  `DnDGen.Infrastructure.Tests.Integration/ReadmeTests.cs`, with the `WriteMember` and table
  helpers extended for generics, `this`, and arrays (plan D1-D3), and see them fail against
  today's one-sentence README
- [X] T005 [US3] Write the sample and table tests in the same file, and see them fail against
  today's README (it has no samples, so they can't find them)

## Phase 3: User Story 1 - A consumer can use the package from the README alone (Priority: P1) 🎯 MVP

- [X] T006 [US1] Write the README's first half: what the package is for, which .NET and dependency
  versions each major version requires, how to install it, how to load the modules, where a table
  lives and its shape (D4, D5), and how to select from collection and percentile tables, with the
  samples T005 tests. Keep the build badge and the existing description
- [X] T007 [US1] Write the data selections section: defining a `DataSelection<T>`, its
  `Separator`, binding it, and the helpers; and the extension points section: the mappers and
  assembly loader, for hand-rolled selectors (D7)
- [X] T008 [US2] Add the Reference section: a Constructor, Method, or Property table per public
  type, signatures in full, defaults included, generic constraints and what a consumer must or may
  implement in the descriptions, until the member tests pass
- [ ] T009 (maintainer) Read the README as someone who hasn't seen the source: can you load the
  modules, add a table, and select from it? (SC-006)

## Phase 4: Release

- [ ] T010 Set `<Version>` to 8.0.1 in `DnDGen.Infrastructure.csproj`, and `<PackageReleaseNotes>`
  to say the package changes only in its README, that 8.0.0 was the last version with code changes,
  and to see the GitHub releases page for anything earlier

## Phase 5: Polish

- [ ] T011 Run the unit and integration test projects; confirm coverage of
  `DnDGen.Infrastructure.dll` hasn't decreased
- [ ] T012 Temporarily change one default (`DataHelper`'s separator) and remove one README row, and
  confirm each fails only the expected test, naming the member; then revert (SC-002, SC-003)
