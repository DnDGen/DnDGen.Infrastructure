# Feature Specification: DnDGen.Infrastructure Theme Song

**Feature Branch**: `002-infrastructure-theme-song`

**Created**: 2026-11-19

**Status**: Draft

**Input**: User description: "Add a DnDGen.Infrastructure theme song, following the rules in DnDGen/.github,
with DnDGen.Stress and DnDGen.RollGen as the examples."

## Clarifications

### Session 2026-11-19

Answered from DnDGen/.github's theme-song README, DnDGen.Stress's spec 001, and DnDGen.RollGen's
spec 002, which settled the same questions. The maintainer hasn't reviewed them yet, except where
marked.

- Q: Which genre? → A: Gregorian chant, the genre the Soundtrack Plan gives Infrastructure, for
  "the scripture everything is built on".
- Q: What language are the lyrics in? → A: English, sung in chant style. *(Chosen by the
  maintainer.)* Latin is the genre's tradition, but the lyrics stay plain English from the README,
  so the record and its tests are DnDGen.Stress's and DnDGen.RollGen's unchanged but for names.
- Q: Are the echoes in English too? → A: No: an echo repeats the line before it, so it is sung in Latin, which costs the song no meaning, and is translated in the record. *(Chosen by the maintainer.)*
- Q: Does it carry a leitmotif? → A: No. Infrastructure is a support library, so its song describes
  its README as it is, as Stress's does.
- Q: Where do the lyrics come from? → A: Infrastructure's README, sung as words, never as code: no
  type names, method calls, commands, or formulas. Any line not from the README is marked so in the
  Sources table.
- Q: Where does the song live? → A: `docs/theme-song/README.md`, with its album art, sources, and
  update procedure, following DnDGen.Stress's record and tests.
- Q: How are the first drafts made? → A: Fresh generations that Sample the main theme's "Rogante
  Marendor" snippet, 3:00.0 to 3:17.1. The take that merges is v1.
- Q: Does this feature also set up Spec Kit? → A: No. Spec Kit setup lands with feature 001, which
  comes first (FR-014 moves there).
- Q: Why is this spec 002? → A: The README is too thin to sing and doesn't meet the constitution's
  README as contract. Feature 001 fixes that first, and this feature starts from its README.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A listener finds and hears the song (Priority: P1)

As someone reading Infrastructure's README, on GitHub or NuGet, I expect a Theme Song badge beside
the build badge and a Theme Song section at the end, both linking to the song on Suno, and the
section linking to the song's record.

**Why this priority**: The song is the deliverable. Nothing else here has anything to describe
until readers can reach it.

**Independent Test**: Open the README: the badge and section are there, and every link works from
NuGet, so none is relative.

**Acceptance Scenarios**:

1. **Given** the README, **When** a reader looks at the badges, **Then** a Theme Song badge sits
   beside the build badge, in the same style as the main theme's.
2. **Given** the badge or the section's link, **When** a reader clicks it, **Then** the published
   song opens.
3. **Given** the section, **When** a reader wants to know more, **Then** its other link opens the
   song's record.

---

### User Story 2 - The song stays in step with the README (Priority: P1)

As the maintainer, if a README change rewords or removes a phrase the song quotes, I expect the
build to fail and list each phrase, and the record to tell me how to update the song.

**Why this priority**: Without this the song drifts from the README with the first change after it
is written.

**Independent Test**: Reword a phrase the song quotes in the README: one test fails, naming that
phrase.

**Acceptance Scenarios**:

1. **Given** a README change that rewords or removes a quoted phrase, **When** the tests run,
   **Then** they fail, name each missing phrase, and point to the update procedure.
2. **Given** any README change, **When** the contributor reads the record's Updating section,
   **Then** they know whether the song needs updating and, if so, what to change and who remixes it.
3. **Given** a change that doesn't touch the README, or touches only the song's own badge, links, or
   mentions, **When** it is reviewed, **Then** no song change is expected.

---

### User Story 3 - The record is complete and fits Suno (Priority: P2)

As the maintainer making the song in Suno, I expect the record's Title, Style, and Lyrics to each
be one code block within Suno's limits (80, 1,000, and 5,000 characters), every lyric line to have
a source, and the album art to exist and link to the same song.

**Why this priority**: The record is what lets someone other than its author update or remix the
song.

**Independent Test**: Paste each block into Suno without trimming; match every lyric line to the
README using only the record.

**Acceptance Scenarios**:

1. **Given** the record, **When** a reader looks up any lyric line, **Then** the Sources table
   gives the README phrase it came from, or marks it as not from the README.
2. **Given** the record, **When** a reader views it, **Then** it shows the album art, the title,
   style, and lyrics, and the lessons learned while generating the song.

### Edge Cases

- **Before the song is generated**: There is no Suno link yet, so the link tests fail until the
  maintainer generates the song and adds it. That is the signal that the feature isn't done.
- **The README was one sentence.** Feature 001 (README documents the public API) comes first and
  gives the song its material: the selectors, mappers, tables, data selections, and IoC loading it
  documents. This spec is written against that README, and its Sources are drafted from it once it
  exists. The song captures the README's ideas and vibe, may repeat lines as a chant does, and
  doesn't invent content the README lacks.
- **Repeated lines and responses**: A line sung more than once needs one Sources row, where it is
  first sung. A line wholly in parentheses is a response, and needs none.
- **Not from the README**: A Sources row starting "Not from the README:" quotes nothing, and is
  allowed for any line the README can't supply, such as a chant's opening.
- **Suno drops, garbles, or growls lines in the take the maintainer picks.** The take is still the
  song; Suno Notes records what it changed.
- **The README is read on NuGet**, where links relative to the repository don't resolve, so every
  README link is absolute.
- **The two repositories merge at different times.** Neither change links to something the other
  has not yet published.

## Requirements *(mandatory)*

### Functional Requirements

**The song**

- **FR-001**: Infrastructure MUST have a theme song published publicly on Suno, with a title and a
  style prompt within Suno's limits.
- **FR-002**: The song MUST be Gregorian chant.
- **FR-003**: The lyrics MUST be in English and capture the README's high-level ideas and vibe as it
  is, sung as words, never as code. Every lyric MUST come from the README, except lines the Sources
  table marks as not from the README.
- **FR-004**: The song MUST NOT carry a leitmotif. Infrastructure is a support library.

**The record**

- **FR-005**: The record, `docs/theme-song/README.md`, MUST have, in order, the sections Updating,
  Title, Style, Lyrics, Sources, Suno Notes, and Album Art, none empty. It MUST show only the
  current song: a remix replaces the previous version.
- **FR-006**: Title, Style, and Lyrics MUST each be one code block within Suno's limits.
- **FR-007**: The Sources table MUST have one row per distinct lyric line, in the order first sung,
  and every phrase it quotes MUST be in the README.
- **FR-008**: The album art MUST be square, recognizably in the main theme's family, exist, link to
  the song, and credit and license everything it reuses.

**Keeping it in step with the README**

- **FR-009**: Every README change MUST get a song check. The song is updated only when the change
  alters the README's high-level ideas or vibe, or changes or removes a phrase the Sources table
  quotes. The update lands in the same pull request: the contributor updates the lyrics and
  Sources, and the maintainer remixes the song and updates its link before merging. A README change
  that touches only the song's own badge, links, or mentions needs no song check.

**Linking it in**

- **FR-010**: The record and the README MUST link to the same, single song on Suno.
- **FR-011**: The README MUST have a Theme Song badge beside the build badge and a Theme Song
  section at the end that links to the record; every README link MUST be absolute.
- **FR-012**: The organization's Soundtrack Plan MUST link to the song, by the maintainer's own pull
  request to DnDGen/.github.

**Checking it automatically**

- **FR-013**: Automated tests MUST check the record's and the README's mechanical rules above, name
  every failure with what to do, MUST NOT judge the song's ideas, vibe, or chosen take, and MUST
  NOT need the network.

### Key Entities

- **Theme song**: the published song, with a title, Suno link, genre, style prompt, and lyrics.
- **Source entry**: one lyric line and the README phrase it came from.
- **Song record**: the written page holding the current song's details, art, and update procedure.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Every new test passes once the maintainer adds the Suno link, and fails, naming the
  phrase, when a quoted README phrase is reworded.
- **SC-002**: From the README, on GitHub or NuGet, a reader gets to the song in one click and to its
  record in one click.
- **SC-003**: Every lyric line, 100%, traces to the README or is marked as not from it.
- **SC-004**: Line and branch coverage of `DnDGen.Infrastructure.dll` does not decrease
  (II.2.c (Infrastructure)).
- **SC-005**: The maintainer judges that the chosen take captures the README's ideas and vibe, and
  approves the album art.

## Assumptions

- The tests are DnDGen.Stress's record and README tests, as DnDGen.RollGen copied them, with
  Infrastructure's names (I.1). They live in `DnDGen.Infrastructure.Tests.Integration`, which gains
  copies of `README.md` and every Markdown and PNG file under `docs/` in its output.
- Suno is the given way to make and publish the song, on the free plan: no Replace Section.
- The album art reuses the main theme's ingredients, the dndgen.com d20 and the Cinzel typeface, and
  their licenses carry over, recoloured for a monastery's scriptorium.
- The README is the contract the constitution describes; the record is supporting documentation.
- Changes to DnDGen/.github are a separate pull request in that repository, made by the maintainer
  from their personal account.
- No package code changes. The README is packed into the package, so the change is a PATCH release,
  8.0.1, whose release notes say the package changes only in its README and that 8.0.0 was the last
  version with code changes.
