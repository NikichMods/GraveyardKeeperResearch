# Confession Notification — Feasibility and UX Research

**Target:** Graveyard Keeper 1.407  
**Status:** research/design only; no production mod exists yet.

## Product problem

A confession can become available in a church confessional without any remote indication to the player. The stock game exposes the state locally through the confessional interaction / prayer icon, so a player who is elsewhere in the world must revisit the church simply to discover whether there is anything to collect.

The desired outcome is informational only:

- preserve the game's confession RNG, rewards, timing, and availability rules;
- tell the player when at least one confession is actually available;
- keep the indication truthful until all available confessions have been handled or the native state is cleared;
- use Graveyard Keeper's existing visual language where practical rather than introducing a foreign-looking notification system.

## Existing verified mechanics

PrayerClarity research already establishes the relevant stock owner path:

- the once-per-game-day logic is church_budka_roll;
- stock logic resets player parameter confession_probability to 0.15 before the roll;
- the graph loops over the two confessionals;
- on each iteration it removes the previous confession_available interaction, obtains a 0..1 random float, reads confession_probability, and evaluates the result before making confession available;
- PrayerClarity: Rebalanced changes the effective probability for Prayer of Repentance while deliberately preserving the surrounding native daily scheduler / roll semantics.

Implication for a notification mod: **do not duplicate the probability calculation or run a parallel roll.** The notification should observe the native resulting confession-availability state.

The exact mutation seam that adds/removes confession_available, its persistence behavior across save/load, and the exact consumer that drives the in-world prayer icon still need to be traced before production code.

## Community signal

This is a real player-facing friction point, although the available evidence is not enough to call it a broad community consensus.

Examples:

- Steam discussion, 2018: players note that nobody is visibly seen arriving/leaving and the only indication is the icon inside the church; one participant explicitly calls the need to keep checking the church poor design.
  - https://steamcommunity.com/app/599140/discussions/0/1733213724900982012/
- Steam discussion, 2019: a player describes confessionals as weak partly because they must be checked repeatedly to know whether someone is there.
  - https://steamcommunity.com/app/599140/discussions/0/1742231705662858611/
- Steam discussion, 2018: a player asks when they should check confessionals because the available information only says a confession may happen on any day.
  - https://steamcommunity.com/app/599140/discussions/0/2727382174629534889/
- The community wiki documents the existing local feedback: a prayer icon appears above a confessional when someone is waiting.
  - https://graveyardkeeper.fandom.com/wiki/Confessional

Treat these as **community signals / UX evidence**, not mechanics proof. The discussions are old and should not be generalized into a claim that most current players want the feature.

A targeted search did not surface an obvious dedicated modern mod whose primary purpose is remote confession notification. This is not proof that none exists.

## UX gap

The mechanic creates a mismatch between where the event is resolved and where the player is expected to notice it:

- the event can become relevant while the player is anywhere in the world;
- the state is actionable for a meaningful reward;
- the game communicates it only at the destination;
- checking the destination has no value on most failed-roll days.

That makes repeated church visits an information tax rather than a gameplay decision.

## Solution-space comparison

### 1. One-shot native-style arrival bubble

When native state changes from no available confession to at least one available confession, show a short stock-style notification near the player, potentially using the same prayer icon semantics.

**Advantages:** low visual footprint; can reuse the game's existing transient-notification grammar.  
**Weakness:** easy to miss; provides no persistent reminder.

### 2. Persistent HUD prayer icon

While at least one confession remains available, show a small persistent prayer icon in a verified native HUD context. Remove it immediately when no confession remains.

**Advantages:** directly solves the information problem; no text is required after the player learns the icon.  
**Weakness:** adds persistent HUD state and therefore needs careful native anchoring / lifecycle research.

### 3. Hybrid: one-shot arrival cue + persistent prayer icon

On the empty -> occupied transition, show a short native-style cue. Keep a small prayer icon visible until the native state returns to zero available confessions.

**Current preferred product direction.**

It separates two different jobs cleanly:

- the transient cue says **something just happened**;
- the persistent icon says **it is still waiting for you**.

The icon should preferably reuse the same prayer/confessional symbol already used in the church, preserving semantic continuity.

An initial version does not need a numeric count. The product requirement is "there is something to collect", not necessarily "which of two confessionals is occupied". A count can be reconsidered only if runtime behavior shows it adds meaningful value.

### 4. Diegetic church-bell cue

A church bell when a confession becomes available is thematically strong and could complement option 3.

However, no reusable church-bell sound seam/resource has yet been verified in current research. Audio is therefore a **design hypothesis**, not an implementation assumption. A sound-only solution would also be missable and is weaker than persistent state.

### 5. Map / quest / day-wheel marker

These are technically plausible presentation families but currently less attractive:

- a map marker still requires opening the map and overstates a small recurring event;
- a journal/task entry is too heavy for frequent stochastic activity;
- a day-wheel marker risks semantic collision with weekday/time information and existing day-wheel quest-marker work.

Do not pursue these unless simpler notification families fail a concrete acceptance need.

## Proposed presentation

Current visual concept:

- **arrival:** a brief native-style prayer-icon cue, optionally with localized text such as "Someone is waiting for confession" / equivalent;
- **persistent state:** the same prayer icon, small and unobtrusive, in a verified HUD anchor;
- **clear:** disappear as soon as the last native confession_available state is gone;
- **optional later:** one verified diegetic sound on the transition, not repeated polling audio.

Avoid a large toast, quest log mutation, custom parchment panel, or permanent text label unless testing shows the icon alone is not understandable.

## Technical feasibility

The concept appears feasible because the game already has a discrete native confession-availability state produced by a known daily graph. The mod should be an observer/presenter of that state rather than a second mechanics implementation.

Candidate native UI family already present in the game:

- EffectBubblesManager.ShowImmediately(...) is used by stock code for short task/resource/relation/status feedback and is a credible transient-notification surface;
- persistent UI should attach to a verified existing HUD/NGUI parent and follow native screen/language lifecycle rather than raw Screen.width/height positioning.

These are candidate presentation seams only. The exact notification owner and persistent anchor are not yet selected.

## Production evidence gate

**BLOCKED — research only.**

- **Observable property:** a truthful remote indication exists exactly while one or more native confession interactions are available.
- **Canonical owner:** native confession state produced by church_budka_roll; the exact add/remove mutation seam still needs to be established.
- **Final writer / consumer:** not yet closed for the in-world icon / interaction lifecycle.
- **Blast radius:** not yet established for any proposed hook.
- **Preserved invariants:** confession RNG, PrayerClarity probability modifications, rewards, daily reset, interaction behavior, save/load behavior, and unrelated HUD behavior must remain unchanged.
- **Acceptance evidence:** should include real daily-state creation, one- and two-confessional states if reachable, collection/clear, next-day reset, save/load while active, and compatibility with stock 15% plus PrayerClarity Rebalanced effective probabilities.

No production source should be created or mutated until the owner/final-consumer questions above are closed.

## Next research

Trace, from the exact GK 1.407 graph/runtime:

1. the node/method that actually adds and removes confession_available;
2. whether that interaction/state is serialized or reconstructed on load;
3. the exact path that produces the visible prayer icon above the confessional;
4. whether there is a clean event-driven seam for empty -> occupied and occupied -> empty;
5. the best native HUD anchor/prefab to reuse for a persistent indicator;
6. only if audio remains desirable, inspect existing game sound resources for a suitable church/bell cue.

Prefer direct static/graph inspection first. Build a probe only if those owners cannot be established cleanly from existing evidence.
