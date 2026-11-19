# Implementation Plan: DnDGen.Infrastructure Theme Song

**Branch**: `001-readme-public-api` (shared with feature 001, as in DnDGen.RollGen's pull request #113) | **Date**: 2026-11-19 | **Spec**: [spec.md](spec.md)

## Summary

Record Infrastructure's theme song in `docs/theme-song`, the way DnDGen.Stress and DnDGen.RollGen
record theirs: a README with the Updating section, Title, Style, Lyrics, Sources, Suno Notes, and
Album Art; album art recoloured from the main theme's cover; and the Cinzel typeface. The README
gets a badge beside the build badge and a Theme Song section at its end. The song is Gregorian
chant in English, drawn from the README that feature 001 wrote, with no leitmotif. DnDGen.RollGen's
record and README tests come across unchanged but for names.

## Technical Context

**Language/Version**: C# on `net10.0`; Markdown and HTML for the record and the art.

**Testing**: `DnDGen.Infrastructure.Tests.Integration`, which already copies `README.md` (feature
001) and now every Markdown and PNG file under `docs/` to its output.

**Constraints**: Suno's limits: 80 characters for the title, 1,000 for the style, 5,000 for the
lyrics; no Replace Section on the free plan. No network access in tests. Every README link is
absolute.

## Constitution Check

| Label | Check | Result |
|---|---|---|
| I.1 | Record, art, update procedure, and tests follow RollGen's, which follow Stress's; the genre and first-draft Sample follow DnDGen/.github's theme-song README. | Pass |
| I.2 | Nothing new: this repeats the established pattern. | Pass |
| I.3 | No code comments added. `cover.html` keeps the main theme's credit comment. | Pass |
| II | The record and README tests are written before the README section and the record, and fail until they and the Suno link are in place. | Pass |
| II.1 | The tests are the approved list from Stress and RollGen, named as consumer outcomes. | Pass |
| II.2 | Single-document tests: sampling doesn't apply beyond Suno's limits, which are tested at their limit. | Pass |
| II.2.c (Infrastructure) | No package code changes, so coverage of `DnDGen.Infrastructure.dll` doesn't decrease. | Pass |
| II.3.a | Source and link tests list every missing phrase or link at once. | Pass |
| II.3.c | The tests read only files in the build output. | Pass |
| README as contract | The song is a document derived from the README, says how it keeps in step, and is updated in the same pull request as the README. | Pass |
| Versioning | PATCH, 8.0.1: documentation and tests only. It ships in the same pull request as feature 001, so one release covers both and its notes say so. | Pass |

## Decisions

- **D1: Language and sound.** English lyrics, with each echo, which repeats the line before it, in Latin, sung as chant: a unison male choir, a single melodic
  line, a long, reverberant stone-nave sound, and no instruments, so Suno doesn't turn it into a
  concert-hall piece. The style names the sound, not the tradition, per the main theme's Suno
  Notes, and names the singers, per Stress's and RollGen's.
- **D2: Material.** The lyrics sing the README's ideas: the scripture everything is built on,
  tables embedded in an assembly, the Percentile and Collection selectors, rolling for rarity,
  data selections split into sections, and mappers anyone may build a selector on. They are sung
  as words: no type names, method calls, or code. A chant repeats, so lines return as refrains.
- **D3: Length.** A chant is quieter and slower than a tavern song. The lyrics aim for roughly 1,200
  to 1,800 characters, measured, for a song of 2:30 to 3:30.
- **D4: Sources.** Every distinct lyric line has a row, in the order first sung, quoting a phrase
  the README contains, or marked "Not from the README:". The record's tests check that every
  quoted phrase is still in the README.
- **D5: Album art.** The main theme's d20 and Cinzel title, recoloured for a monastery's scriptorium:
  warm parchment, ink black, and gold leaf, the d20 as an illuminated roundel with its ghost copies
  as a page of old marginalia. The d20 is the dndgen.com loading spinner, credited and linked as
  in the other repositories. `cover.png` is rendered from `cover.html` at 1500 × 1500.
- **D6: The README.** The badge goes on the build badge's line, as in Stress and RollGen. The Theme
  Song section goes after Reference, so it is the last `##` section, and links to the song and the
  record. The member tests don't see it: they read only tables.
- **D7: The DnDGen/.github change.** A patch file, `dndgen-github.patch`, holds the maintainer's
  change: Infrastructure's row in the organization profile's Soundtrack table, and its listen and
  record links in the Soundtrack Plan row. The leitmotif cell stays empty, since Infrastructure has
  none. The maintainer applies it, with the Suno link filled in, in their own pull request.
- **D8: One pull request.** Spec Kit, feature 001, and this feature ship together, titled
  `8.0.1: Spec Kit, README API tests, theme song`, as RollGen's did.

## Project Structure

```text
README.md                                                      # badge and Theme Song section
DnDGen.Infrastructure/DnDGen.Infrastructure.csproj             # release notes mention the theme song
docs/theme-song/
├── README.md                                                  # the song's record
├── cover.html, cover.png                                      # album art
└── Cinzel.ttf, Cinzel-OFL.txt                                 # typeface, unchanged from DnDGen/.github
DnDGen.Infrastructure.Tests.Integration/
├── DnDGen.Infrastructure.Tests.Integration.csproj             # copies docs/**/*.md and *.png
├── ReadmeTests.cs                                             # badge, section, and link tests added
└── Docs/ThemeSong/ReadmeTests.cs                              # the record's tests
specs/002-infrastructure-theme-song/dndgen-github.patch        # the maintainer's DnDGen/.github change
```

## Complexity Tracking

None.
