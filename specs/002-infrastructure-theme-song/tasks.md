---

description: "Task list for the DnDGen.Infrastructure theme song"
---

# Tasks: DnDGen.Infrastructure Theme Song

**Input**: Design documents from `specs/002-infrastructure-theme-song/`

**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), and feature 001 (the README the song
draws from)

**Tests**: Written first, from the approved list in DnDGen.Stress and DnDGen.RollGen, and seen to
fail before the README and the record that make them pass (II).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- **(maintainer)**: Only the maintainer can do this task. `/speckit-implement` stops and hands it
  over.

## Phase 1: Setup

- [X] T001 Copy every Markdown and PNG file under `docs/` to the integration test output, as
  DnDGen.RollGen's test project does
- [X] T002 [P] Add `docs/theme-song/Cinzel.ttf` and `Cinzel-OFL.txt`, unchanged from DnDGen/.github

## Phase 2: Tests

- [X] T003 (maintainer) Approve the test list. It is DnDGen.RollGen's approved list, with
  Infrastructure's names:
  - Record: `HasCorrectSectionsInOrder`; `SunoField_FitsSunoLimit` for Title, Style, and Lyrics;
    `Sources_HasCorrectTable`; `Sources_EveryQuotedPhraseIsStillInTheReadme`;
    `AlbumArt_LinksToTheSong`; `AlbumArt_ImageExists`.
  - README: `ThemeSong_HasBadgeBesideTheBuildBadge`; `ThemeSong_HasSectionAtTheEnd`;
    `ThemeSong_LinksToTheRecord`; `ThemeSong_LinksToTheSameSongAsTheRecord`;
    `DocumentLinks_AreAbsolute`.
- [X] T004 Write the record's tests in
  `DnDGen.Infrastructure.Tests.Integration/Docs/ThemeSong/ReadmeTests.cs`, and see them fail: there
  is no record
- [X] T005 Write the README's theme song tests in
  `DnDGen.Infrastructure.Tests.Integration/ReadmeTests.cs`, and see them fail against today's
  README, which has no badge, section, or record link

## Phase 3: The Song

- [X] T006 [US3] Draft the Title, Style, and Lyrics from the README, and their Sources, as plan D1 to
  D4 describe: Gregorian chant, English, no leitmotif, no code in the lyrics
- [X] T007 (maintainer) Review the draft lyrics and style before anything is generated
- [X] T008 [US3] Draw the album art for a scriptorium, recoloured from the main theme's cover (plan
  D5), and render it to `cover.png` at 1500 × 1500
- [X] T009 (maintainer) Approve the album art
- [X] T010 (maintainer) Generate drafts in Suno, each Sampling the main theme's "Rogante Marendor"
  snippet (3:00.0 to 3:17.1) with Variety all the way down; pick and publish the take
- [X] T011 (maintainer) Add the Suno link to the record (the cover and Listen on Suno) and the
  README (the badge and the Theme Song section), and record what was learned in Suno Notes

## Phase 4: The README

- [X] T012 [US1] Add the Theme Song section to the end of the README, after Reference, linking to
  the record. The badge, and the section's link to the song, wait for the Suno link (T011)
- [X] T013 Write `dndgen-github.patch`: Infrastructure's row in the organization profile's
  Soundtrack table, and its listen and record links in the Soundtrack Plan row, with `SUNO_LINK`
  standing in until T011
- [ ] T014 (maintainer) Open the DnDGen/.github pull request from `dndgen-github.patch`, with the
  Suno link filled in

## Phase 5: Release

- [X] T015 Add the theme song to `<PackageReleaseNotes>` for 8.0.1, which ships in the same pull
  request as feature 001

## Phase 6: Polish

- [X] T016 Once T011 is done, run the unit and integration test projects and confirm every test
  passes
- [ ] T017 (maintainer) Open the pull request, titled
  `8.0.1: Spec Kit, README API tests, theme song`, with `cidthecoatrack` as the reviewer
