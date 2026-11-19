# Credo of the Tables -DnDGen.Infrastructure Theme-

![Credo of the Tables album art](cover.png)

The DnDGen.Infrastructure theme song: Gregorian chant, sung in English, with Latin answers, the scripture everything is built on.
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
   Names are sung as words, never as code. A Latin answer follows the English it answers, sung as ordinary lines by the
   other voice with a bracketed tag, and is added to the table under the Lyrics and to the Sources row of the English line it answers.
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
A cappella Gregorian chant, an unaccompanied male monastic choir singing a single plainchant melody, slow and solemn, holding the same unhurried pace from the first note to the last, A cantor sings alone in plain, level chant, gentle and clear, and the full choir answers in unison, in the verses the cantor sings the English and the choir answers in Latin, in the antiphons the choir sings the English and the cantor answers in Latin, Soft, rounded vowels and a steady legato, sung in a stone nave with natural reverb, ending on a hushed amen fading into the echo of the nave, Reverent, warm, candlelit, and ancient
```

## Lyrics

```
[Introitus - the cantor intones alone, a cappella plainchant, slow and level]
Common, core objects for the generators
[Choir - answers in Latin, in unison, a cappella plainchant, slow and soft]
Res communes et fundamentales generatoribus

[Versus I - the cantor alone, a cappella plainchant, slow and level]
Their data in tables, embedded in their assemblies
Collection tables, named lists of entries
Percentile tables, a result for every roll from one to one hundred
[Choir - answers in Latin, in unison, a cappella plainchant, slow and soft]
Eventus pro omni iactu

[Antiphona - the full choir sings in unison, a cappella plainchant, slow and steady]
Ye shall read the tables
[Cantor - answers alone in Latin, a cappella plainchant, slow and level]
Tabulas legetis
[Full choir]
Ye shall fetch the data
[Cantor]
Data afferetis

[Versus II - the cantor alone, a cappella plainchant, slow and level]
One to sixty picks from the common
[Choir - answers in Latin, in unison, a cappella plainchant, slow and soft]
Commune
[Cantor]
Sixty-one to ninety, the uncommon
[Choir]
Haud commune
[Cantor]
Ninety-one to ninety-nine, the rare
[Choir]
Rarum
[Cantor]
And at one hundred, the very rare
[Choir]
Rarissimum

[Antiphona - the full choir sings in unison, a cappella plainchant, slow and steady]
Ye shall read the tables
[Cantor - answers alone in Latin, a cappella plainchant, slow and level]
Tabulas legetis
[Full choir]
Ye shall fetch the data
[Cantor]
Data afferetis

[Versus III - the cantor alone, a cappella plainchant, slower still]
Together they must cover every roll
Exactly once
Table names must be unique within an assembly
Read once per kernel, and then cached
[Choir - answers in Latin, in unison, a cappella plainchant, slow and soft]
Semel lectum, deinde servatum

[Antiphona - the full choir sings in unison, a cappella plainchant, slow and steady]
Ye shall read the tables
[Cantor - answers alone in Latin, a cappella plainchant, slow and level]
Tabulas legetis
[Full choir]
Ye shall fetch the data
[Cantor]
Data afferetis

[Versus IV - the cantor alone, a cappella plainchant, slow and level]
Your own types, split into sections
Mappers, so you can build a selector of your own
[Choir - answers in Latin, in unison, a cappella plainchant, slow and soft]
Selectorem tuum aedifica

[Antiphona - the full choir sings in unison, a cappella plainchant, slow and steady]
Ye shall read the tables
[Cantor - answers alone in Latin, a cappella plainchant, slow and level]
Tabulas legetis
[Full choir]
Ye shall fetch the data
[Cantor]
Data afferetis

[Versus V - the cantor alone, a cappella plainchant, slow and level]
A new instance of something each time
Safe to call twice
Modules that wire all of it up
[Choir - answers in Latin, in unison, a cappella plainchant, slow and soft]
Omnia coniunge

[Antiphona - the full choir sings in unison, a cappella plainchant, slow and steady]
Ye shall read the tables
[Cantor - answers alone in Latin, a cappella plainchant, slow and level]
Tabulas legetis
[Full choir]
Ye shall fetch the data
[Cantor]
Data afferetis

[Conclusio - the cantor alone, a cappella plainchant, slow and level]
Praise the random number gods
[Choir - answers in Latin, hushed, in unison, fading into the echo of the nave]
Laudate deos numerorum fortuitorum
Amen
```

Each Latin line answers the English before it, sung by the other voice (the choir after a verse, the cantor after an antiphon), so nothing is lost in the translation:

| Latin | English |
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
| Common, core objects for the generators<br>Res communes et fundamentales generatoribus | Introduction: "common, core objects for the generators" |
| Their data in tables, embedded in their assemblies | What It Is: "keep their data in XML tables embedded in their assemblies" |
| Collection tables, named lists of entries | What It Is: "Collection tables" and "named lists of entries" |
| Percentile tables, a result for every roll from one to one hundred<br>Eventus pro omni iactu | What It Is: "Percentile tables" and "a result for every roll from 1 to 100" |
| Ye shall read the tables<br>Tabulas legetis | What It Is: "reads those tables" |
| Ye shall fetch the data<br>Data afferetis | What It Is: "does the fetching" |
| One to sixty picks from the common<br>Commune | Collection tables: "1 to 60 picks from" |
| Sixty-one to ninety, the uncommon<br>Haud commune | Collection tables: "61 to 90 from" |
| Ninety-one to ninety-nine, the rare<br>Rarum | Collection tables: "91 to 99 from" |
| And at one hundred, the very rare<br>Rarissimum | Collection tables: "and 100 from" |
| Together they must cover every roll<br>Exactly once | Percentile tables: "Together they must cover every roll" and "exactly once" |
| Table names must be unique within an assembly | Where tables live: "table names need to be unique within an assembly" |
| Read once per kernel, and then cached<br>Semel lectum, deinde servatum | Where tables live: "read once per kernel and then cached" |
| Your own types, split into sections | What It Is: "your own types" and "split into sections" |
| Mappers, so you can build a selector of your own<br>Selectorem tuum aedifica | Writing your own selector: "so you can build a selector of your own" |
| A new instance of something each time | Building things as you need them: "a new instance of something each time" |
| Safe to call twice | Loading the modules: "safe to call twice" |
| Modules that wire all of it up<br>Omnia coniunge | What It Is: "that wire all of it up" |
| Praise the random number gods<br>Laudate deos numerorum fortuitorum | Not from the README: a nod to the gods of chance, in keeping with the chant's amen |
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
- **Latin is sung as ordinary lines, answered by the other voice.** In the verses the cantor sings the English and the
  choir answers in Latin; in the antiphons the choir sings the English and the cantor answers in Latin. Each answer
  has a bracketed voice tag: the first spells out the delivery, and later ones are short (`[Cantor]`, `[Choir]`, `[Full choir]`). Parentheses carried the Latin in the first four drafts, and Suno dropped it almost every
  time, singing choral oohs and aahs over the Sample where the Latin could have gone. An older chant song (a Grinch
  parody on Suno v3.5) sang all-Latin lyrics correctly with ordinary lines, Latin section names (`[Introitus]`,
  `[Versus I]`, `[Antiphona]`, `[Conclusio]`), and a short Style: "A cappella Gregorian chant. Unaccompanied male
  monastic choir, slow and ominous... No instruments." This song borrows the ordinary lines, the Latin section
  names, and the short a cappella Style.
- **Keep each verse and antiphon whole.** The first restructure gave every Latin answer its own full tag and split the  verses into segments, and the antiphons lost their shape. The answers now alternate line by line inside the section,  and only the first tag in a section spells out the delivery. Whether a plain em dash before each answer would carry  the response without any tag, as the Grinch song's did, is untried.
- **Four drafts so far.** Every one dropped the intro's Latin, "Res communes et fundamentales generatoribus".
  - **v0.1 and v0.2** (four verses, a different last antiphon, a Style that named a "string drone"): they dropped the
    Latin, v0.1 left chant for an orchestra in verse 2, and v0.2 fell into the cinematic well by the third antiphon and
    let the cantor soar in verse 4. The Sample gave a nice string drone and made the choir sound mostly female, which
    is fine.
  - **v0.3 and v0.4** (five verses, identical antiphons, the Latin in parentheses, the Style asking for the drone and the
    Latin twice): they still dropped the intro and verse 1 Latin and the antiphon Latin, the antiphon brought in plucked
    strings, and in v0.4 plucked strings were faintly there by verse 4 and grew more cinematic as the song went on.
  - **The voices came out slightly digitized in every draft.** Unintended, not unwelcome. "A sober, perfectly in-tune
    choir" may have asked for it; the old song's Style said nothing about tuning.
- **Don't name an instrument you hope to allow.** The Style named a "single low bowed-string drone" to cap the
  accompaniment, and plucked strings arrived anyway: naming one probably told Suno it was already leaning toward
  instruments and offered a starting point to grow. The Sample pulls that way regardless. The Style now says only
  that the choir is a cappella and unaccompanied, and any instrument that creeps in from the Sample can stay small.
- **Say it twice: in the Style, naming the section, and in that section's tag.** The echoes, the slow chant pace, and
  the plain, level cantor are in both, and the tags no longer use words like "rising" or "soaring" (verse 4's old tag
  said "rising", which probably invited the soaring solo). The tags now all say "a cappella plainchant".
- **Antiphons are the same every time.** Church antiphons repeat word for word, so all of them share one set of lyrics and
  one tag pair, and the IoC material has its own fifth verse. Only the last is marked hushed.
- **The Style describes the delivery in positives** (soft, even, legato, rounded vowels) instead of forbidding shouting
  or growling, since Web learned that a "never" can pull Suno toward the thing it names. Not yet tried in a generation
  that kept the instruments out.
- *To fill in by the maintainer once the song is generated:* what the next drafts dropped, garbled, or added.

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
