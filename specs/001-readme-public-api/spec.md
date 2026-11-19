# Feature Specification: README Documents the Public API

**Feature Branch**: `001-readme-public-api`

**Created**: 2026-11-19

**Status**: Draft

**Input**: User description: "The README is one sentence and doesn't meet the DnDGen constitution's
README as contract. Document every public contract in the README and test that it stays
documented, following DnDGen.Stress's spec 002 and DnDGen.RollGen's spec 001. This comes before the
theme song."

## Clarifications

### Session 2026-11-19

These follow DnDGen.Stress's and DnDGen.RollGen's answers to the same questions, since nothing in
Infrastructure calls for different ones. The maintainer hasn't reviewed them yet, except where
marked.

- Q: Does this feature also set up Spec Kit? → A: Yes, as its own commits ahead of the spec:
  `specify init --here --integration claude --script ps`, the DnDGen constitution as a template
  override, then Infrastructure's constitution, as DnDGen.Stress and DnDGen.RollGen did.
- Q: Which public members must the README document? → A: Every public member of every public type
  in `DnDGen.Infrastructure.dll`: constructors, methods, properties, fields, and constants, as the
  DnDGen constitution's README as contract requires. Today that is the interfaces and abstract
  class consumers call or implement, the extension and helper classes, and the module loader.
  Infrastructure's constitution names no types, so it doesn't change whenever the public API does;
  the tests find them by reflection. Stress's and RollGen's constitutions list theirs, and should
  follow this later (outside this feature).
- Q: Is documenting members enough? → A: No. Members are what the tests check, but "every public
  contract" also means how a consumer uses them: loading the Ninject modules, where a table lives
  (an embedded XML resource) and what shape it has, how a data selection's sections are split, and
  which .NET and dependency versions each major version needs. These are in the README, written
  with the members, but are judged by hand, not by the tests.
- Q: Should the tests pin the README's overall structure? → A: No. The tests check only what the
  contract requires, so restructuring the README stays free.
- Q: How are generic members written? → A: By their full signature, type parameters included, such
  as `SelectRandomFrom<T>(IEnumerable<T> collection)`, as RollGen writes them. Constraints are
  documented in the row's description, not the signature, since they don't tell overloads apart.
- Q: Which parameter names does a member use? → A: Interface members use the interface's names, which
  are what a consumer sees in named arguments, as in RollGen.

## User Scenarios & Testing *(mandatory)*

The README is packed into the NuGet package and is the only documentation a consumer sees. Two
people rely on it: the **consumer**, who reads it to wire Infrastructure into a generator, and the
**maintainer**, who changes Infrastructure and needs to know when the README has fallen behind.

### User Story 1 - A consumer can use the package from the README alone (Priority: P1)

As a generator author, I expect the README to tell me what Infrastructure provides, how to load it
into my Ninject kernel, how to put my tables in my assembly and read them with the selectors, and
how to define my own data selections, without reading the source.

**Why this priority**: Today the README is one sentence. A consumer has nothing to go on, and the
constitution requires better.

**Independent Test**: Someone who has not seen the source follows the README to load the modules,
add a collection table to an assembly, and select from it.

**Acceptance Scenarios**:

1. **Given** the README, **When** a consumer reads it, **Then** it says what the package is for,
   which .NET and dependency versions each major version needs, and how to install it.
2. **Given** the README, **When** a consumer wants to select from a collection or percentile table,
   **Then** a code sample shows loading the modules and calling a selector, and says where the table
   lives and what shape it has.
3. **Given** the README, **When** a consumer wants a typed table of their own, **Then** a code sample
   defines a data selection and binds it.

---

### User Story 2 - A new or changed member can't go undocumented (Priority: P1)

As the maintainer, if I add, remove, or re-shape a public member of Infrastructure (a constructor,
method, property, field, or constant, including a parameter's type, name, or default) and don't
update the README, I expect the build to fail, and the failure to name exactly which member the
README is missing or has wrong.

**Why this priority**: An undocumented member is invisible to consumers, and a wrongly documented
default is believed. Nothing catches either today.

**Independent Test**: Change a default in `DataHelper.Parse` without touching the README: one test
fails, listing the changed signature as undocumented and the old one as documented but missing.

**Acceptance Scenarios**:

1. **Given** every public member is documented, **When** the tests run, **Then** they pass.
2. **Given** a public member or overload that the README doesn't document, **When** the tests run,
   **Then** a test fails and lists every undocumented one, methods by their full signature.
3. **Given** the README documents a member or overload that no longer exists, **When** the tests
   run, **Then** a test fails and lists every documented one that doesn't exist.
4. **Given** a public type is added, **When** the tests run, **Then** its members are checked too,
   without anyone adding the type to a list.

---

### User Story 3 - The README's code samples are tested (Priority: P2)

As the maintainer, I expect every code sample in the README to compile-and-run as written, so that a
change that breaks the documented way of using the package fails the build.

**Why this priority**: Stress and RollGen test their samples; a sample that no longer works is worse
than none.

**Independent Test**: Change a documented selector's behaviour: the test of the sample that shows it
fails.

**Acceptance Scenarios**:

1. **Given** the README's samples, **When** the tests run, **Then** each is exercised against the
   real package and its output matches what the README says.

### Edge Cases

- **Generic interfaces and methods**: `ICollectionDataSelector<T>` and
  `SelectRandomFrom<T>(IEnumerable<T> collection)` differ from their non-generic or differently
  generic overloads only by type parameters and parameter types. Each overload is its own documented
  item, told apart by its full signature.
- **Extension methods**: `BindingExtensions.BindDataSelection<T>(this IKernel kernel)` is documented
  as the consumer calls it, on a kernel.
- **Optional parameters**: Defaults, such as the separator `'@'` in `DataHelper`, are part of the
  signature, so a changed default fails like a changed parameter.
- **Abstract and virtual members**: `DataSelection<T>`'s abstract members, which a consumer must
  implement, and its virtual `Separator`, which they may override, are documented as such.
- **Grouped rows**: Related members may share one table row; every code span in the row's first cell
  counts as documented.
- **Code samples**: A name that appears only inside a code sample doesn't count as documented.
- **Inherited members**: Members every .NET object has aren't part of Infrastructure's API and
  aren't required.
- **Internal types**: Types that are `internal`, which includes the selectors', mappers', and
  factories' implementations, are not public API, even though the test projects can see them.
- **Restructuring**: Adding, renaming, or reordering README sections fails no new test, as long as
  every member is still documented.

## Requirements *(mandatory)*

### Functional Requirements

**The README**

- **FR-001**: The README MUST say what the package is for, which .NET and dependency versions each
  major version requires, and how to install it, and MUST be updated in the same pull request as any
  change to them.
- **FR-002**: The README MUST show, with code samples, how to load the modules into a Ninject
  kernel, where a collection or percentile table lives and its shape, how to select from them, and
  how to define and bind a data selection.
- **FR-003**: The README MUST document every public member, in a table per public type, with its
  default where it has one, and what a consumer must or may implement.

**The tests**

- **FR-004**: The tests MUST fail when a public member of a public Infrastructure type, including
  each overload, isn't documented in a README table outside a code sample, and MUST list every one
  that isn't.
- **FR-005**: The tests MUST fail when the README documents a member that doesn't exist, and MUST
  list every one that doesn't.
- **FR-006**: Methods and constructors MUST be documented by their full signature: generic type
  parameters, and each parameter's type, name, `params` and `this` modifiers, and default value.
- **FR-007**: "Public API" MUST mean every public constructor, method, property, field, and constant
  of every public type in `DnDGen.Infrastructure.dll`, found by reflection. Members inherited from
  `object` are excluded.
- **FR-008**: Every code sample in the README MUST be exercised by a test.
- **FR-009**: The tests MUST NOT pin the README's overall structure, and MUST NOT need the network.
- **FR-010**: The new tests MUST sit in the integration `ReadmeTests`, one test file per file under
  test.

**Setting up Spec Kit**

- **FR-011**: The repository MUST have Spec Kit, set up as in DnDGen.Stress and DnDGen.RollGen:
  Claude skills in `.claude/skills`, templates and PowerShell scripts in `.specify`, the DnDGen
  constitution installed as a template override, and a filled Infrastructure constitution that
  points to it, whose README as contract lists what the README documents. Machine-local Claude
  settings are ignored.

### Key Entities

- **Documented item**: A member as the README names it in the first column of a table. A method or
  constructor is written as its signature; any other member by its name.
- **Public API**: FR-007.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: With the README updated, every new test passes, and the README documents 100% of the
  public API.
- **SC-002**: Removing any single documented item from the README makes exactly the test for that
  kind of item fail, naming that item.
- **SC-003**: Changing any single default makes exactly one test fail, naming the method.
- **SC-004**: Line and branch coverage of `DnDGen.Infrastructure.dll` does not decrease
  (II.2.c (Infrastructure)).
- **SC-005**: A maintainer reading a failure knows what to change in the README without opening the
  test code (II.3).
- **SC-006**: Someone who has not seen the source can load the modules, add a table, and select from
  it by following the README (User Story 1), as judged by the maintainer.

## Assumptions

- Signatures carry parameter types and defaults, as RollGen's do, because Infrastructure's overloads
  share names (`DataHelper.Parse` has six). This is RollGen's pattern, not a new one (I.2).
- This is a tests-and-documentation change: no code in the package changes, so it ships as a PATCH,
  8.0.1, whose release notes say the package changes only in its README and that 8.0.0 was the last
  version with code changes. If a test shows a public type should be internal, that is a MAJOR
  change, decided separately, as RollGen did with `Utils`.
- The public API is as it is: the README documents it and doesn't change it. The mappers and the
  assembly loader are public on purpose, so a consumer who hand-rolls a selector can use them, and
  the README presents them that way.
- The README keeps its build badge, and gains the theme song's badge and section in feature 002.
- The tests and the csproj change that copies the README to the test output are plan-level detail.
