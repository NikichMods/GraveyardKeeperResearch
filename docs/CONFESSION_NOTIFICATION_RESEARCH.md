# Confession Notification — Feasibility and UX Research

**Target:** Graveyard Keeper 1.407  
**Status:** research/design only; no production mod exists yet.

## Expanded product direction

The concept is no longer limited to confessionals. The durable product goal is a small, vanilla-friendly **important waiting-state notification layer** for remote events that otherwise require the player to remember/check a location.

Current core scope:

1. **Corpse waiting** — preserve the stock donkey arrival sound/transient notification and add a persistent HUD reminder while an unprocessed/delivered corpse state still warrants player attention.
2. **Confession waiting** — add a thematically appropriate church/confession arrival sound plus a transient icon, then retain a persistent HUD reminder while at least one native confession is available.

Do not generalize this into a generic production-complete notifier. A future event should be added only when it matches the same product class: asynchronous/remote, discrete and actionable, reasonably important, locally invisible enough to create checking friction, and rare/bounded enough that a persistent alert is not HUD spam.

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

Follow-up inspection closes the native state owner/lifecycle:

- `Flow_AddInteractionEvent` calls `WorldGameObject.AddInteractionEvent(event)`;
- the exact event string is stored in `WorldGameObject.custom_interaction_events`;
- `Flow_RemoveInteractionEvent` removes it from that same list;
- the custom-interaction path in `WorldGameObject.Interact` consumes the queued event before firing it;
- `SerializableWGO.FromWGO` serializes the custom interaction-event list and `SerializableWGO.ToWGO` restores it;
- both `church_budka_1` and `church_budka_2` define `custom_interaction_icon="(pray_bubble)"`, which the generic interaction-bubble path can use while a custom interaction exists.

Therefore the canonical query for current confession availability is whether a live confessional WGO contains `"confession_available"` in `custom_interaction_events`. The game already persists that state; a notification mod should not save a parallel confession flag.

The verified stock visual semantic is `(pray_bubble)`.

## Corpse waiting-state ownership follow-up

The stock transient corpse-arrival UI is a dedicated path:

`Flow_BodyArrivedNotify -> NewBodyArrivedGUI.Display()`

Accepted PrayerClarity donkey-graph evidence also establishes ordinary delivery through `Flow_DropBody` in the `npc_donkey` graph. Depending on the morgue chute state, ordinary delivery can use an outside branch or a repaired-chute branch whose drop target is resolved by `Flow_FindWGO(custom_tag="morgue_throw_out")`.

The physical delivered body is host-owned loose-drop state:

- `Flow_DropBody` creates the body and delegates to a WGO's `DropItem`;
- `WorldGameObject.DropItem` delegates to `DropResGameObject.Drop`;
- the resulting Body drop is added to `DropsList.me.drops`;
- its native world zone is recorded in `DropResGameObject.zone_id` / `Item.drop_zone_id`;
- collection removes the loose drop through the normal `DropsList` lifecycle;
- `DropsList.ToGameSave` serializes loose-drop position, item/body data, and `zone_id`; `FromGameSave` recreates them.

Important negative finding: player parameter `cur_bodies_count` is general morgue occupancy, not "the newly delivered corpse is still waiting". Both ordinary delivery branches increment it and body disposal paths decrement it. It can remain nonzero because of unrelated bodies in the morgue, so it must not drive a delivery reminder.

Follow-up installed-runtime evidence from Keeper's Alerts Corpse State Probe 0.1.0 closes the physical save/load lifecycle on the repaired-chute path:

- a delivered Body appeared in `DropsList` with `zone_id="morgue"` / `Item.drop_zone_id="morgue"`;
- it settled to a stable native position near `morgue_throw_out`;
- save/load/world reconstruction replaced the Unity object instance but recreated the Body at the exact same saved position and zone;
- subsequent pickup removed that reconstructed Body from the loose-drop list.

Reusable implication: **Unity instance identity is ephemeral, but native loose-drop position/zone/body state is host-persistent.**

Static follow-up also confirms the evidence boundary: `Flow_DropBody` generates an ordinary Body Item, and `GameSave.SavedDropItem` stores the Item, position and zone but no dedicated donkey-delivery provenance field. Do not invent a hidden provenance ID.

For notification-style consumers, the host-native model should therefore be current **receiving-area occupancy by a loose Body**, if that semantic fits the product. Strict historical provenance after arbitrary manual movement would require separately justified mod-owned persistence.


## Confession final-consumer seam follow-up

Static GK 1.407 lifecycle inspection closes the normal-play mutation convergence for native confession availability.

Relevant paths:

- `WorldGameObject.AddInteractionEvent` adds the event and then calls `RedrawBubble`;
- `Flow_RemoveInteractionEvent` removes the event from `custom_interaction_events` and then calls `RedrawBubble`;
- `WorldGameObject.Interact` consumes the first queued custom interaction event, fires it, and then calls `RedrawBubble`;
- `WorldGameObject.RedrawBubble` delegates to `ComponentsManager.RefreshBubblesData`, which reads the already-mutated `custom_interaction_events` list and renders `custom_interaction_icon` while the list is nonempty.

Installed balance census also found exactly two ObjectDefinitions using `custom_interaction_icon="(pray_bubble)"`: `church_budka_1` and `church_budka_2`.

Reusable implication: a filtered **post-`WorldGameObject.RedrawBubble` resync** is a credible least-sufficient normal-play observation seam for confession availability. It observes the final native state after add/remove/consume instead of patching RNG or each writer separately. A one-time world/load resync is still required because no mutation necessarily occurs immediately after restoration.

The `(pray_bubble)` value is rendered through the normal bubble text path: `ComponentsManager.RefreshBubblesData` places the token into `BubbleWidgetTextData`, and `BubbleWidgetText.Draw` renders it with an NGUI `UILabel` / native `UIFont`. Treat it as a native label/font-symbol token until runtime evidence proves a standalone Sprite mapping; do not assume `EasySpritesCollection.GetSprite("(pray_bubble)")`.

## Community signal

This is a real player-facing friction point, although the available evidence is not enough to call it a broad community consensus.

Examples:

- Steam discussion, 2018: players note that nobody is visibly seen arriving/leaving and the only indication is the icon inside the church; one participant explicitly calls the need to keep checking the church poor design. Another proposes: "There should be a bell like with the corpses!", followed by a request for a visual indicator as well.
  - https://steamcommunity.com/app/599140/discussions/0/1733213724900982012/
- Steam discussion, updated in 2024: a player independently asks for a church bell when somebody is in a confessional so the player knows to return before missing it.
  - https://steamcommunity.com/app/599140/discussions/0/1694923613864163134/
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

- stock corpse arrival has its own `NewBodyArrivedGUI.Display()`, invoked by `Flow_BodyArrivedNotify`; this is now the strongest first presentation family to inspect if confession arrival should look literally like a sibling of corpse arrival;
- `EffectBubblesManager.ShowImmediately(...)` remains a secondary stock transient-feedback family, but should not be preferred merely because it is easy if the dedicated corpse-arrival presentation can be reused more faithfully;
- persistent UI should attach to a verified existing HUD/NGUI parent and follow native screen/language lifecycle rather than raw `Screen.width/height` positioning.

These are candidate presentation seams only. The exact notification owner and persistent anchor are not yet selected.

## Adjacent-event survey

A targeted first-pass survey did **not** identify a third event that currently belongs in the same core scope.

### Strong fit

- **Corpse delivery:** asynchronous, remote, discrete, actionable, important, already has a stock sound + transient visual notification but lacks the proposed persistent reminder.
- **Confession availability:** asynchronous, remote, discrete, short-lived/actionable, currently locally signaled only at the church, with repeated player requests for corpse-style audio/visual notification.

### Weaker / currently excluded

- **Merchant crate proceeds:** money waits in the merchant cashbox and is collected through the trading-result UI. This is a real remote waiting reward, and community questions show some discoverability friction, but the sale follows a known weekly cycle and behaves more like accumulated income than an arrival requiring immediate attention. Keep as a possible later candidate, not core scope.
- **Tavern cashbox:** continuous/accumulating revenue rather than a discrete arrival event.
- **Weekly NPC presence / quest availability:** already communicated structurally by the day cycle and belongs closer to the existing day-wheel/quest-marker problem domain.
- **Crops, furnaces, zombie production/logistics:** high-frequency production completion; notifying these would create a different automation/status-dashboard product and likely HUD spam.
- **Refugee/story NPC arrivals:** mostly one-off progression events/cutscenes rather than recurring remote waiting states.

Re-open this survey if player evidence reveals another event with the same checking-friction pattern. Do not broaden scope just because a state can technically be observed.

## Production evidence gate

**BLOCKED — research only.**

- **Observable property:** a truthful remote indication exists exactly while one or more native confession interactions are available.
- **Canonical owner:** live confessional `WorldGameObject.custom_interaction_events`; the exact `confession_available` event is added/removed by native interaction-event nodes and consumed by native interaction.
- **Final writer / consumer:** the interaction-event list is serialized/restored by `SerializableWGO`; the generic WGO bubble path consumes the object's verified `(pray_bubble)` custom interaction icon.
- **Blast radius:** not yet established for any proposed hook.
- **Preserved invariants:** confession RNG, PrayerClarity probability modifications, rewards, daily reset, interaction behavior, save/load behavior, and unrelated HUD behavior must remain unchanged.
- **Acceptance evidence:** should include real daily-state creation, one- and two-confessional states if reachable, collection/clear, next-day reset, save/load while active, and compatibility with stock 15% plus PrayerClarity Rebalanced effective probabilities.

No production source should be created or mutated until the owner/final-consumer questions above are closed.

## Next research

Trace, from the exact GK 1.407 graph/runtime:

1. verify the least-sufficient event-driven transition seam for confession empty -> occupied and occupied -> empty, including blast radius;
2. verify whether the donkey-delivered loose corpse can be reconstructed after save/load from native drop position/zone without meaningful provenance false positives;
3. verify the best native HUD anchor/prefab for persistent indicators;
4. inspect whether the dedicated corpse-arrival UI can be reused/mirrored cleanly for confession arrival;
5. inspect existing game sound resources for a suitable church/confession cue.

Prefer direct static/graph inspection first. Build a probe only if those owners cannot be established cleanly from existing evidence.
