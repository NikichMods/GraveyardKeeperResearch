# Confession Notification — Feasibility and UX Research

**Target:** Graveyard Keeper 1.407  
**Status:** shared host/runtime research; production consumer exists at `NikichMods/KeepersAlerts`.

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

The `(pray_bubble)` value is rendered through the normal bubble text path: `ComponentsManager.RefreshBubblesData` places the token into `BubbleWidgetTextData`, and `BubbleWidgetText.Draw` renders it with an NGUI `UILabel` / native `UIFont`.

Accepted installed-runtime font/atlas evidence additionally closes the symbol mapping:

`(pray_bubble) -> icon_pray_bubble`

The installed icon atlas reports `icon_pray_bubble` as 17x20. The token and sprite ID are distinct identifiers; do not pass `"(pray_bubble)"` to `EasySpritesCollection.GetSprite`.


## Native notification/HUD presentation follow-up

Keeper's Alerts Presentation Probe 0.1.0 on GK 1.407 closes reusable host presentation facts.

### Stock corpse-arrival transient

The runtime owner is:

`UI Root/NewBodyArrivedPanel/BodyArrivedPanel`

with `NewBodyArrivedGUI` on the panel.

Installed serialized presentation:
- parent `UI Root/NewBodyArrivedPanel`: full-screen NGUI `UIPanel`;
- notification widget: 108x62;
- `Background`: `UI2DSprite("icon_frame_techno")`, 96x52;
- `BodyImage`: `UI2DSprite("body_01")`, 96x96;
- `PlusText`: `tiny_font` UILabel containing `+`;
- timings: 0.5 s appear, 1.0 s hold, 0.5 s hide;
- visible-point Y: 80.

Static `NewBodyArrivedGUI.Display()` owns activation plus slide-in/hold/slide-out behavior. Consumers wanting a sibling transient notification should prefer reusing/cloning this native family over recreating its motion by guess.

### HUD lifecycle and responsive anchor

The runtime HUD root is:

`UI Root/HUD`

and has its own full-screen `UIPanel`.

The stock `hud left` widget is anchored directly to:

`UI Root/Screen size panel/Screen size`

at the top-left.

Static `HUD.Open()` / `HUD.Hide()` toggles the HUD root during ordinary GUI-window lifecycle.

Reusable implication: a small persistent mod indicator parented under `UI Root/HUD` and anchored to the verified screen-size target can inherit native HUD visibility and responsive placement without raw `Screen.width/height` positioning.

### Corpse-delivery endpoint geometry

Installed runtime endpoint transforms:

- `morgue_throw_out=(10656,-10992,-2297.572)`;
- `morgue_throw_in=(3672,-1896,-374.235)`.

Accepted repaired-chute delivery samples settled approximately 35-72 world units from `morgue_throw_out`.

Reusable implication: current receiving-area state can be bounded relative to the native endpoint instead of treating all of `zone_id="morgue"` as equivalent.

### Church prayer sound boundary

Prior church-loaded evidence found:

`World/[wgo] church_pulpit/content/church_pulpit(Clone)/PrayFX/pray sound`

with `DarkTonic.MasterAudio.EventSounds`.

A later morgue-loaded snapshot did not contain that transform. Treat this as chunk/load-state dependence, not as evidence that the object is absent from the game.

The exact church-pulpit prayer sound group is now verified from direct `resources.assets` inspection as `chorus_short`.


## Direct asset closure for church prayer sound

A user-supplied GK `resources.assets` was inspected directly without redistributing the proprietary asset.

Source identity:
- size: `93,209,412 bytes`;
- SHA-256: `215c7981901a4b72d5db717666ba47ad3cc032527c95f58dc39d8af1293a69ca`;
- Unity engine string: `2020.3.17f1`.

Serialized Transform/GameObject linkage proves the prefab chain:

`church_pulpit -> PrayFX -> pray sound`

The `pray sound` GameObject contains a Transform plus one MonoBehaviour. Prior installed-runtime evidence identifies that component as `DarkTonic.MasterAudio.EventSounds`.

Its serialized AudioEvent payload contains:

`Your action name -> chorus_short -> [None]`

A matching MasterAudio `AudioEvent` layout serializes `actionName`, then `isExpanded`, then `soundType`; the payload therefore identifies the stock sound group as:

`chorus_short`

Reusable implication: mods wanting the same native church/prayer cue should invoke the existing MasterAudio group `chorus_short`; do not extract or ship the game's audio asset.

The accompanying `resources.assets.resS` stream is not required to identify/reuse the group. It would only be needed to extract the underlying streamed clip for offline inspection.

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

### 4. Diegetic audio cue

A short native audio cue complements the hybrid presentation.

Two stock sound groups were verified during Keeper's Alerts research:

- `chorus_short` — the native church-pulpit prayer sound group, established from the serialized `church_pulpit -> PrayFX -> pray sound` chain;
- `bell_single` — a stock short one-shot sound used by existing game flows.

The installed game was used for direct A/B auditioning, and `bell_single` was selected for Keeper's Alerts because it better fits the desired short notification role.

Reusable host fact: both sound IDs are verified native resources. The choice of `bell_single` for confession notification is a Keeper's Alerts product decision, not a general Graveyard Keeper semantic claim.

### 5. Map / quest / day-wheel marker

These are technically plausible presentation families but currently less attractive:

- a map marker still requires opening the map and overstates a small recurring event;
- a journal/task entry is too heavy for frequent stochastic activity;
- a day-wheel marker risks semantic collision with weekday/time information and existing day-wheel quest-marker work.

Do not pursue these unless simpler notification families fail a concrete acceptance need.

## Accepted Keeper's Alerts presentation resolution

The production consumer resolved the presentation as:

- **arrival audio:** one native `bell_single` cue on the aggregate false -> true confession transition;
- **arrival visual:** a private sibling clone of the stock `BodyArrivedPanel` / `NewBodyArrivedGUI.Display()` presentation, using the native prayer symbol while leaving the stock corpse-arrival instance untouched;
- **persistent state:** a small prayer indicator parented under the live `HUD.bar_energy` transform while at least one confession remains available;
- **clear:** remove the persistent indicator when the aggregate native confession state returns to false;
- **load:** restore persistent state silently without synthesizing a new-arrival cue.

A numeric count is intentionally unnecessary for the current product requirement. Exact visual calibration is project-specific and remains canonical in `NikichMods/KeepersAlerts`.

## Technical feasibility and accepted host seams

The production path is feasible without duplicating confession mechanics or introducing recurring polling.

Accepted host/runtime seams for the current consumer:

- canonical confession truth: `WorldGameObject.custom_interaction_events` containing `"confession_available"` on `church_budka_1` / `church_budka_2`;
- normal add/remove/consume convergence: filtered postfix on `WorldGameObject.RedrawBubble(bool?)`, after the native list mutation;
- load boundary: silent authoritative resync at `MainGame.OnGameStartedPlaying`;
- transient family: private clone of the stock `BodyArrivedPanel` / `NewBodyArrivedGUI.Display()` lifecycle;
- persistent HUD family: private children of the live `HUD.bar_energy` transform, so normal HUD visibility, anchoring and scaling remain host-owned;
- audio: invoke the verified stock `bell_single` group; do not extract or redistribute game audio.

The owning Keeper's Alerts repository is authoritative for product-specific patch composition, exact calibration, compatibility guards and release acceptance.

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

## Production evidence closure

**Status: CLOSED for the existing Keeper's Alerts consumer on Graveyard Keeper 1.407.**

- **Observable property:** a truthful remote notification appears when aggregate native confession availability changes from none to at least one, and persistent state remains visible exactly while at least one native confession interaction is available.
- **Canonical owner:** live confessional `WorldGameObject.custom_interaction_events`; native save serialization/restoration owns persistence.
- **Final writer / consumer:** native add/remove/consume paths mutate the list before `WorldGameObject.RedrawBubble`; the production consumer performs a filtered postfix resync there and a silent post-load resync at `MainGame.OnGameStartedPlaying`.
- **Blast radius:** the hook is globally placed but immediately filtered to the two verified confessionals before Keeper's Alerts state/presentation work; stock corpse presentation uses a separate untouched instance.
- **Preserved invariants:** confession RNG/probability, PrayerClarity probability changes, rewards, daily reset, interaction behavior, save/load semantics and unrelated HUD/audio behavior remain host-owned.
- **Acceptance evidence:** integrated runtime testing covered existing active state after load, false -> true notification, clear behavior, one/both persistent states, stock corpse notification coexistence and the selected audio/transient presentation.

The accepted reusable conclusion is that an observer/presenter can implement this feature event-driven from native state without reproducing the daily roll or storing a parallel confession flag.

## Research boundary

No additional host-internals research is required for the current Graveyard Keeper 1.407 Keeper's Alerts implementation. Re-open research only for a new game build, a materially different presentation/behavior requirement, or evidence that one of the verified owner/lifecycle assumptions changed.

## Native single-bell candidate

Direct search of the installed GK `resources.assets` found a stock sound id:

`bell_single`

It is explicitly used by three serialized `Flow_PlaySound` nodes in refugee-story flows; each use is followed by a 1.5-second wait. This proves `bell_single` is a real short one-shot game sound id, distinct from corpse `donkey_bell`.

The stock church preaching flow separately calls `Flow_PlaySound("chorus")`; therefore `bell_single` is not the normal sermon cue.

Current evidence does not tie `bell_single` to a church tower and does not prove its timbre from the identifier alone. For vanilla-friendly notification work it is nevertheless a stronger first audio candidate than the longer church-prayer chorus when a single bell-like cue is desired.


## Corpse receiving-corridor closure

Keepers Alerts follow-up research closes the native delivery envelope for both ordinary donkey branches.

Accepted host facts:
- pre-repair `Flow_DropBody`: source is the donkey WGO, direction `Up`;
- repaired `Flow_DropBody`: source is `morgue_throw_out`, direction `Down`;
- directional `Flow_DropBody` passes force `3f` and `check_walls=false`;
- `DropResGameObject` applies an immediate 28.800001-unit directional offset and then `KickComponent`;
- `DropResCurve.duration_factor` is authored in the range 0..1;
- `KickComponent` decays delta by 0.96 per fixed step and stops below 0.01;
- vertical motion uses the native 0.8 factor;
- GK sets `Time.fixedDeltaTime` to 1/60 in normal play and 1/12 during sleep/wait.

Direct installed `resources.assets` inspection identifies the native DockPoint component and verifies:
- donkey prefab has no DockPoint;
- repaired `morgue_throw_out` has no DockPoint.

Therefore both sources use their WGO transform as `GetDropPos()`.

Under the worst inspected timestep (1/12), force 3 and duration factor 1, maximum kick travel is approximately 459.22 world units; with the immediate directional offset, the derived maximum forward travel is approximately 488.02 units.

A conservative reusable receiving corridor is:
- lateral half-width 96;
- backward allowance 48;
- forward length 544;
relative to the applicable source and direction.

Use live host anchors where possible:
- pre-repair: `donkey_cemetery_point`;
- repaired: `morgue_throw_out`.

Pre-repair evidence shows `morgue_throw_out_broken`; its repair craft replaces it with `morgue_throw_out`, so the repaired WGO is a usable branch discriminator.

### Event-driven loose-body resync

For current receiving-area occupancy, the least-sufficient host seams are:
- `DropsList.Add` after successful Body addition;
- `DropResGameObject.CollectDrop` after Body pickup marks `is_collected=true`;
- `DropsList.FromGameSave` after save reconstruction for whole-list resync/clear.

Normal big Body items do not use stack-merging removal. Known direct list-removal exceptions inspected in GK 1.407 concern invalid drops, dungeon teardown or a stone/marble save migration, not ordinary cemetery/morgue Body pickup.

This permits event-driven corpse waiting state with no recurring polling and no mod-owned persisted provenance flag.


## Corpse receiving-area closure

Keeper's Alerts follow-up research closes the reusable native geometry/lifecycle for a persistent corpse-waiting reminder on GK 1.407.

Pre-repair delivery uses the donkey WGO itself as source with Direction.Up. The donkey ObjectDefinition uses `drop_point=Auto`, and direct installed `resources.assets` inspection found no DockPoint component in the donkey prefab, so `WorldGameObject.GetDropPos()` resolves to the donkey transform. The route targets native `donkey_cemetery_point`; accepted runtime logs place the donkey at or close to that point during delivery.

Repaired delivery uses live `morgue_throw_out` with Direction.Down.

Both directional `Flow_DropBody` branches call `DropItem` with force factor 3 and `check_walls=false`; therefore wall avoidance cannot rotate the authored Up/Down direction.

Native drop physics applies an immediate 28.800001-unit offset, then a decaying kick. The largest fixed timestep assigned by inspected game code is 1/12 during sleep/wait. With the authored maximum drop-curve duration factor 1, the maximum forward displacement is approximately 488.02 world units including the initial offset.

Recommended bounded receiving corridor around the applicable native source:
- lateral half-width: 96 world units;
- backward allowance: 48;
- forward length: 544;
- Up for pre-repair, Down for repaired.

This remains substantially narrower than any whole-zone predicate.

Recommended event-driven resync:
- live add: postfix `DropsList.Add`, filtered to a successful Body addition;
- live clear: postfix `DropResGameObject.DestroyLinkedHint`, filtered to `is_collected && Body`. This is required because large-item/overhead Body pickup in `BaseCharacterComponent.TryOtherInteractions` sets `is_collected=true` directly and bypasses `CollectDrop`; both it and `CollectDrop` call `DestroyLinkedHint` after the commit;
- post-load initialization: postfix `MainGame.OnGameStartedPlaying`, after saved loose drops have been reconstructed.

Bodies do not enter stack-merging removal because `DoTryMerging` returns for `definition.is_big`.

Initial/load resync should establish persistent current state without synthesizing an arrival cue.


## Accepted closure update — 2026-09-28

This section supersedes the older BLOCKED/audio-hypothesis wording above. The production consumer is now `NikichMods/KeepersAlerts`; its repository remains the project-specific source of truth.

### Confession audio

The stock `bell_single` sound was directly A/B auditioned in the installed game against `chorus_short`. The user selected `bell_single` as the desired short confession-availability cue.

Status: **ACCEPTED**.

### Donkey drop origin runtime confirmation

Keeper's Alerts Corpse Drop Origin Probe 0.1.0 performed a read-only installed-runtime snapshot and confirmed:

- `donkey.GetDropPos()` exactly equaled `donkey.tf.position`;
- the reported drop-minus-donkey delta was `(0,0,0)`;
- the live donkey had zero DockPoints.

This independently confirms the earlier asset-derived conclusion that the pre-repair directional delivery origin is the donkey transform itself.

### Corpse lifecycle correction

The least-sufficient ordinary Body lifecycle is:

- live add: postfix `DropsList.Add`, filtered to successful Body addition;
- live clear: postfix `DropResGameObject.DestroyLinkedHint`, filtered to collected Body, because large-item/overhead Body pickup can bypass `CollectDrop` but still commits `is_collected=true` before `DestroyLinkedHint`;
- load: disarm during `DropsList.FromGameSave`, then perform one silent authoritative resync at `MainGame.OnGameStartedPlaying`.

No recurring polling and no mod-owned persisted corpse provenance are required.

### Core production gate

The core state-observation/audio/HUD/transient paths are now READY for an integrated runtime candidate. Remaining acceptance is candidate-level runtime/perceptual validation of touched behavior, especially transient/HUD placement and coexistence.
