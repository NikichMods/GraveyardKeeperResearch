# Perk Mechanics

Target: **Graveyard Keeper 1.407 (PC)**.

## Status and evidence

**Status:** verified static/current-data result, supported where noted by accepted 1.407 runtime dumps.

Primary evidence:

- pinned host source: `Kupie/GYK_DECOMP@6abf79199d92482af1c7573870dd9a20ec2270b9`;
- accepted 1.407 balance inputs used by NikichMods research:
  - `resources.assets` SHA-256 `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`;
  - `Assembly-CSharp.dll` SHA-256 `e72e4270e4b88dd0a87ca23c9cf1750aec4c4a0fedb40b6d2dae7902fc9c7fd8`;
- accepted Technology Work/Perk audit for current perk definitions and native consumers;
- accepted balance/runtime dumps for craft, item, drop, surgery and buff definitions.

External wiki/community material is useful only as corroboration or evidence of player confusion. It is not the canonical mechanics source because several current pages preserve older or inconsistent values.

## Native multi-quality model

`PerkDefinition.stars` is not a percentage.

For a multi-quality `CraftDefinition`:

1. `CraftDefinition.GetPerkValue(perk_id)` returns 0 if the perk is not unlocked; otherwise it returns that perk's `stars` value.
2. `GetMultiqualityResult()` sums the active `linked_perks` star values (plus any linked buff craft-quality contribution) into `value_perks`.
3. Ingredient-quality contribution and perk contribution are combined with craft difficulty into the result-quality calculation.
4. `CraftComponent.DropMultiqualityOutput()` rolls the authored/resulting quality probabilities and emits the selected quality tier.

The native Craft UI already exposes this contribution in its expanded quality presentation as a **star score with one decimal place**. Therefore wording such as `+★0.3` is a native-facing quality metric, not an invented percentage.

### Verified quality perks

| Perk | Stars | Current 1.407 linked quality consumers / scope |
| --- | ---: | --- |
| Writer (`p_writer`) | +0.3 | writing/prayer multi-quality family, including stories/notes/chapters and related writing outputs |
| Playwright (`p_good_writer`) | +0.5 | the writing/prayer multi-quality family, including story/chapter/book-related outputs |
| Industriousness (`p_industriousness`) | +0.2 | broad affected-craft family: writing, food, wine, books/prayers and selected material-quality crafts |
| Engineer (`p_engineer`) | +0.3 | `carved_wood`, steel-chisel crafts `chisel_2b` / `chisel_2_2b`, `marble_plate_3` |
| Jeweler (`p_jevelery`) | +0.7 | hardcover/book crafts `book_hard`, `book_hard_2`, `book_hard_souls` |
| Wine Master (`p_wine_master`) | +0.8 | red-wine craft `bottle_red_vine` |
| Blacksmith (`p_blacksmith`) | +0.1 | steel-chisel quality crafts `chisel_2b` / `chisel_2_2b` |
| Mason (`p_mason`) | +0.5 | carved-marble quality craft `marble_plate_3` |
| Woodworker (`p_woodworker`) | +0.5 | carved-wood quality craft `carved_wood` |

These values can stack when the same craft authors several linked perks. For example, the native calculation adds Writer, Playwright and/or Industriousness contributions when that craft links them and the player owns them.

Do not re-express `+★0.3` as “+30% quality”: the final output probability depends on ingredient quality, difficulty, other perks and buffs.

## Jeweler

Vanilla Russian description says the player finds precious materials more often and can make high-quality jewelry.

Current 1.407 consumers show two concrete effects:

### Quality

`p_jevelery` is a +★0.7 linked quality perk on:

- `book_hard`;
- `book_hard_2`;
- `book_hard_souls`.

In the current data this is a **book/hardcover-book quality bonus**, despite the flavor wording about jewelry.

### Dungeon-source yield

Current source expressions add one unit when `p_jevelery=1`:

- diamond source: minimum 4 -> 5 faceted diamonds;
- gold source: minimum 8 -> 9 gold nuggets;
- silver source: minimum 8 -> 9 silver nuggets.

Product implication: a concise factual presentation can say that book crafting quality gains +★0.7 and dungeon precious-material yield gains +1. Do not replace this with a guessed generic “jewelry quality” claim.

## Wine Master

Vanilla Russian description says all alcohol the player makes has higher quality.

Current 1.407 craft linkage is narrower:

- `p_wine_master` contributes **+★0.8** to `bottle_red_vine` (red wine);
- inspected beer and mead production does not link `p_wine_master` for craft quality.

Therefore the authored “all alcohol” quality wording is overbroad for current 1.407 craft-quality ownership.

### Consumption-energy side effect

The perk also affects consumption through `ItemDefinition.on_use_expressions`. `Item.UseItem()` applies the item's normal `params_on_use` and then evaluates those expressions, so the additions are real player-energy gains.

Verified examples while `p_wine_master=1`:

- red wine bronze: base 60 + 25 = 85 energy;
- red wine silver: 72 + 30 = 102;
- red wine gold: 84 + 30 = 114;
- beer bronze: 18 + 8 = 26;
- beer silver: 24 + 5 = 29;
- beer gold: 30 + 5 = 35;
- mead bronze: 12 + 5 = 17;
- mead silver: 18 + 8 = 26;
- mead gold: 24 + 10 = 34;
- apple ferment: base 15 + 7;
- berry ferment: base 24 + 10;
- additional alcoholic drink variants such as honey/hop-based drinks also carry `p_wine_master` energy expressions.

There is **no single verified universal percentage multiplier** behind these current item definitions. Do not summarize the 1.407 effect as “+20% energy” merely because an external guide/wiki does so.

Product implication: the quality effect is easy to state exactly (`Red wine crafting quality: +★0.8`). The consumption side effect is mechanically real but heterogeneous; a Technology tooltip should either use a qualitative clause (“Alcohol restores more energy”) or omit it rather than invent a fake universal percentage.

## Blacksmith

`p_blacksmith` has two kinds of current consumers.

### Output amount

When the perk is active, current craft expressions add:

- nails: **+3**;
- simple iron parts: **+1**;
- complex iron parts: **+1**;
- steel parts: **+2**.

### Quality

- steel-chisel crafting quality: **+★0.1**.

The vanilla “find time to make a couple of nails” wording therefore understates the current effect and does not mention the quality component.

## Industriousness

`p_industriousness` contributes **+★0.2** on a broad set of authored linked multi-quality crafts.

This is an additive craft-quality score, not a universal +20% probability. A safe concise semantic is “Craft quality: +★0.2 for affected crafts.”

Avoid claiming that literally every craft in the game receives the bonus; only authored linked consumers do.

## Engineer

`p_engineer` contributes **+★0.3** to the current linked quality consumers:

- carved wood;
- carved marble;
- steel chisels.

The vanilla phrase “all parts are of the highest quality” is not a literal description of the current consumer set.

## Writer / Playwright

Current 1.407 values:

- Writer `p_writer`: **+★0.3**;
- Playwright `p_good_writer`: **+★0.5**.

Both feed the same native multi-quality calculation on their authored writing-related consumers and can stack where the craft links both perks.

Current external documentation is inconsistent on these numbers; some pages preserve +0.5/+0.7 values. For 1.407, the current balance definitions and native calculation above are authoritative.

## Mason

`p_mason`:

- carved-marble quality: **+★0.5**;
- common stone-processing outputs inspected by the audit: **+2 stone**;
- higher carved-stone output path: **+1**.

The authored description is directionally correct (“more stone”) but omits the exact quantities and quality contribution.

## Woodworker

`p_woodworker`:

- carved-wood quality: **+★0.5**;
- saw-family productivity:
  - flitch: **+2**;
  - billets: **+1**;
  - planks: **+1**;
  - wooden beam: **+1**.

The vanilla “circular saws are more productive” description is directionally correct but not numerically explicit and does not mention the carved-wood quality contribution.

## Surgery: Butcher and Doctor

The surgery mistake is represented by an authored `surgeon_mistake` output chance. A surgeon mistake contributes +1 red skull and -1 white skull.

### Butcher (`p_butcher`)

For the common/basic extraction family, the current formula is:

```
0.25 - 0.25 * p_butcher - 0.15 * buff_cleancut
```

Ignoring the separate Clean Cut buff:

- without Butcher: **25%** mistake chance;
- with Butcher: **0%**.

A clear numerical statement is therefore **25% -> 0%**, not “-25%”.

### Doctor (`p_doctor`)

For important organs at Preparation Place I, current formula:

```
0.50 - 0.25 * p_doctor - 0.15 * buff_cleancut
```

Ignoring Clean Cut:

- without Doctor: **50%**;
- with Doctor: **25%**.

For important organs at Preparation Place II:

```
0.25 * (1 - p_doctor) - 0.15 * buff_cleancut
```

Ignoring Clean Cut:

- without Doctor: **25%**;
- with Doctor: **0%**.

Thus there is no single context-free “Doctor gives X% fewer mistakes” sentence that preserves the current mechanics. The station-qualified before/after values are exact.

## Big Guy / Martial Skills

Perk `p_big_guy` writes:

- `add_damage=2`;
- `add_armor=2`.

This is not merely an internal half-scaled representation:

- `GameSave.UnlockPerk` adds the PerkDefinition `output_res` directly to player parameters;
- `WorldGameObject.GetDamage()` adds player `add_damage` directly to weapon damage;
- `HPActionComponent.DecHP()` subtracts player `add_armor` directly from incoming damage after equipment armor, clamped to a minimum of zero damage.

Therefore the current 1.407 effect is:

- **+2 weapon damage**;
- **2 less incoming damage**.

The Russian authored description says “+1 damage, +1 defense”, so its numeric values are stale/wrong for current 1.407.

### Sword Master

`p_sword_master` writes `add_damage=5` and reaches the same final damage path.

Current semantic: **+5 weapon damage**.

## Persistence

Technology/Perk `Persistence` sets `p_persistence=1`.

The hidden conditional buff `buff_dlc_refugee_persistence` has:

- condition: `p_persistence=1`;
- tick expression: `AddPpar("energy", 1)`;
- tick period: `1`;
- hidden presentation.

The buff lifecycle keeps the conditional effect active while its condition is satisfied, and `PlayerBuff.CustomUpdate(Time.deltaTime)` executes its tick only while game time is running.

Current semantic: **passively restores 1 energy per running second**. It is not a sleep-only or post-work recovery bonus.

## Miner

`p_miner` modifies several native drop expressions, so no one universal “valuable ore chance” percentage exists.

For iron mining:

- gold nugget chance, once the gold-nugget unlock is active: **5% -> 10%**;
- silver nugget chance, once the silver-nugget unlock is active: **10% -> 20%**.

For coal-source secondary drops, when their corresponding unlock is active:

- limestone/lifestone: chance 40% -> 70%, authored amount range 1–4 -> 2–7;
- sulfur: chance 20% -> 40%, range 1–2 -> 2–3;
- pyrite expression would change 30% -> 60%, range 1–2 -> 2–4, but the current known `p_t_pyrite` / `p_t_pirit` unlock-data mismatch means an informational mod must **not** confidently advertise the pyrite path as an enabled result.

Product implication: if exact Miner numbers are shown, prefer explicit before/after pairs such as “Gold nugget chance from iron: 5% -> 10%; silver: 10% -> 20%.” This avoids the ambiguity between relative percentage increase and percentage points.

## Cultist

`p_cultist` is a visibility/unmasking perk, not a skull-stat bonus.

Current body-part definitions expose their `q_plus` / `q_minus` values through `show_q_hint` expressions gated by `Ppar("p_cultist") > 0`.

`Item.GetBodySkulls()` sums each installed body's/part's red and white skull values into the corpse total.

Therefore the displayed Cultist skull values mean:

> **the contribution of that part while it is installed in the corpse.**

They are not “what will happen when you remove this organ.” Removing the organ removes that contribution, so the visible corpse total changes in the opposite direction.

The vanilla description is literally compatible with this mechanic, but player confusion can arise because the UI does not state the direction explicitly.

## Presentation implications

Numbers should only be surfaced in Technology tooltips when they remain intelligible without a second explanation.

Good compact forms:

- quality-score perks: `Writing quality: +★0.3`;
- direct stat perks: `Weapon damage: +5`;
- probability changes: `25% -> 0%`, not “-25%”;
- heterogeneous chance bonuses: explicit named before/after values rather than an aggregate percentage;
- broad authored effects with multiple different consumers should not be collapsed into a mathematically false universal number.

No production-mod wording or behavior is approved by this research document. It establishes current 1.407 mechanics only.
