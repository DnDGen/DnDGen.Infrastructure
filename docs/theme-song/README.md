# Credo of the Tables -DnDGen.Infrastructure Theme-

![Credo of the Tables album art](cover.png)

The DnDGen.Infrastructure theme song: Gregorian chant, sung in English with Latin echoes, the scripture everything is built on.
Its lyrics come from the [DnDGen.Infrastructure README](https://github.com/DnDGen/DnDGen.Infrastructure/blob/main/README.md), sung in plain English.
Infrastructure is a support library, so its song has no leitmotif: it describes its README as it is.

**Not on Suno yet.** The maintainer adds the song's Suno link here, on the cover and as *Listen on Suno*, once the take is picked.

## Updating

This song follows the [DnDGen.Infrastructure README](https://github.com/DnDGen/DnDGen.Infrastructure/blob/main/README.md).
Every pull request that changes the README gets a song check:

1. If the change rewords or removes a phrase in the Sources table below, the song needs updating.
   The tests catch this one: they fail and list every quoted phrase that is no longer in the README.
2. Otherwise, if the change alters the README's high-level ideas or vibe, the song needs updating.
3. Otherwise, it doesn't. Changes to the README's Theme Song badge or section never need a song update.

When the song needs updating, it happens in the same pull request:

1. The contributor updates the Lyrics and Sources here to match the new README.
   Names are sung as words, never as code. An echo in parentheses repeats the line before it, in Latin,
   and is added to the echo table under the Lyrics; it needs no source.
2. The maintainer makes the new song in Suno:
   - **Cover the current song**, so the theme stays recognizably the same song as it evolves.
   - Use the Title, Style, and Lyrics from this page. **Leave the Style as it is**: only the lyrics change.
   - **Turn Variety all the way down**, so Suno keeps to the Style instead of straying from it.
   - Pick the take. The new take replaces the old one.
   - **Publish it**, so anyone with the link can find and hear it.
3. The maintainer updates the Suno link here and in the README, and adds anything learned to the Suno Notes.

## Title

```
Credo of the Tables -DnDGen.Infrastructure Theme-
```

## Style

```
Gregorian chant, a unison choir of monks singing a single plainchant melody in a slow, free, flowing rhythm, Solemn, serene and unhurried, sung in a vast stone cathedral nave with long natural reverb, Voices only and no instruments, with a deep, steady drone held by the lowest voices, A calm cantor sings each verse alone, clearly and gently, then the whole choir answers in unison on the refrains, every line sung softly and evenly in a gentle, steady legato, A sober, perfectly in-tune choir with crisp diction and soft, rounded vowels, Antiphon refrains sung by the full choir, with each line echoed softly in Latin, ending on a hushed amen fading into the echo of the stone nave, Reverent, warm, candlelit, and ancient
```

## Lyrics

```
[Intro - a low, steady drone; the cantor intones alone]
Common, core objects for the generators
(Res communes et fundamentales generatoribus)

[Verse 1 - the cantor alone, gentle and clear]
Their data in tables, embedded in their assemblies
Collection tables, named lists of entries
Percentile tables, a result for every roll from one to one hundred
(Eventus pro omni iactu)

[Antiphon - the full choir in unison]
Ye shall read the tables
(Tabulas legetis)
Ye shall fetch the data
(Data afferetis)

[Verse 2 - the cantor alone, steady]
One to sixty picks from the common
(Commune)
Sixty-one to ninety, the uncommon
(Haud commune)
Ninety-one to ninety-nine, the rare
(Rarum)
And at one hundred, the very rare
(Rarissimum)

[Antiphon - the full choir in unison]
Ye shall read the tables
(Tabulas legetis)
Ye shall fetch the data
(Data afferetis)

[Verse 3 - the cantor alone, slower]
Together they must cover every roll
Exactly once
Table names must be unique within an assembly
Read once per kernel, and then cached
(Semel lectum, deinde servatum)

[Antiphon - the full choir in unison]
Ye shall read the tables
(Tabulas legetis)
Ye shall fetch the data
(Data afferetis)

[Verse 4 - the cantor alone, rising]
Your own types, split into sections
Mappers, so you can build a selector of your own
(Selectorem tuum aedifica)
A new instance of something each time
Safe to call twice

[Final Antiphon - the full choir, hushed and unhurried]
Modules that wire all of it up
(Omnia coniunge)
Ye shall read the tables
Ye shall fetch the data

[Outro - the cantor alone, then the choir fades into the echo of the nave]
Praise the random number gods
(Laudate deos numerorum fortuitorum)
Amen
[End]
```

An echo in parentheses repeats the line before it, in Latin, so nothing is lost in the translation:

| Echo | English |
|---|---|
| Res communes et fundamentales generatoribus | Common and fundamental things for the generators |
| Eventus pro omni iactu | A result for every roll |
| Tabulas legetis | Ye shall read the tables |
| Data afferetis | Ye shall fetch the data |
| Commune | Common |
| Haud commune | Not common |
| Rarum | Rare |
| Rarissimum | Very rare |
| Semel lectum, deinde servatum | Read once, then kept |
| Selectorem tuum aedifica | Build a selector of your own |
| Omnia coniunge | Join it all together |
| Laudate deos numerorum fortuitorum | Praise the gods of random numbers |

The Latin is approximate.

## Sources

| Lyric | From the README |
|---|---|
| Common, core objects for the generators | Introduction: "common, core objects for the generators" |
| Their data in tables, embedded in their assemblies | What It Is: "keep their data in XML tables embedded in their assemblies" |
| Collection tables, named lists of entries | What It Is: "Collection tables" and "named lists of entries" |
| Percentile tables, a result for every roll from one to one hundred | What It Is: "Percentile tables" and "a result for every roll from 1 to 100" |
| Ye shall read the tables | What It Is: "reads those tables" |
| Ye shall fetch the data | What It Is: "does the fetching" |
| One to sixty picks from the common | Collection tables: "1 to 60 picks from" |
| Sixty-one to ninety, the uncommon | Collection tables: "61 to 90 from" |
| Ninety-one to ninety-nine, the rare | Collection tables: "91 to 99 from" |
| And at one hundred, the very rare | Collection tables: "and 100 from" |
| Together they must cover every roll<br>Exactly once | Percentile tables: "Together they must cover every roll" and "exactly once" |
| Table names must be unique within an assembly | Where tables live: "table names need to be unique within an assembly" |
| Read once per kernel, and then cached | Where tables live: "read once per kernel and then cached" |
| Your own types, split into sections | What It Is: "your own types" and "split into sections" |
| Mappers, so you can build a selector of your own | Writing your own selector: "so you can build a selector of your own" |
| A new instance of something each time | Building things as you need them: "a new instance of something each time" |
| Safe to call twice | Loading the modules: "safe to call twice" |
| Modules that wire all of it up | What It Is: "that wire all of it up" |
| Praise the random number gods | Not from the README: a nod to the gods of chance, in keeping with the chant's amen |
| Amen | Not from the README: the chant's closing |

## Suno Notes

Lessons from generating this song, for the next update:

- **Drafts and the Sample.** Every draft before this song first merges is a fresh generation, Sampling the main theme's
  "Rogante Marendor" snippet (3:00.0 to 3:17.1) with Variety all the way down, per the main theme's
  [New Theme Songs](https://github.com/DnDGen/.github/blob/main/theme-song/README.md#new-theme-songs).
  The take that merges is v1; every update from here Covers it.
- **Starting point, before any generation.** The Style names the sound, not the tradition, and names the singers (a calm
  cantor, a unison choir), because the other songs' drafts came out growly or shouty until they did. It describes the delivery in positives (soft, even, legato, rounded vowels) instead of
  forbidding shouting or growling, since Web learned that a "never" can pull Suno toward the thing it names, and it asks for voices only.
- **No code in the lyrics**, as DnDGen.Stress learned. The rarity ranges are sung as words.
- **Echoes are Latin.** Each echo repeats the line before it, so singing it in Latin costs the song no meaning. Parentheses
  make the second voice, per the main theme's notes, so keep them. If Suno drops an echo, that is the song as it is.
- *To fill in by the maintainer once the song is generated:* what Suno dropped, garbled, or added.

## Album Art

[cover.png](cover.png) is rendered from [cover.html](cover.html), which you can open in any browser.
To render it again at 1500 × 1500, from this folder:

```
msedge --headless --hide-scrollbars --allow-file-access-from-files --window-size=1500,1500 --screenshot=cover.png cover.html
```

If Edge reads `cover.html` as a web address, or doesn't write the file, pass both as full paths instead: `--screenshot=` the full path of `cover.png`, and `file:///` plus the full path of `cover.html`.

- It is the main theme's [cover](https://github.com/DnDGen/.github/blob/main/theme-song/cover.html), recoloured for the scriptorium.
  The d20 is an illuminated initial in gold leaf, the ghost copies behind it are its marginalia in rubric red,
  and a four-line chant staff of square notes runs beneath it.
- The d20 is the dndgen.com loading spinner, from [DnDGen.Web](https://github.com/DnDGen/DnDGen.Web/blob/main/dndgen-web/src/assets/loadingDice.svg), originally from [a gist by hperantunes](https://gist.github.com/hperantunes/f4c2fd407c2ea74c6c9d).
- The title is set in [Cinzel](https://github.com/NDISCOVER/Cinzel), included as [Cinzel.ttf](Cinzel.ttf) under the [SIL Open Font License](Cinzel-OFL.txt).
