# Graveyard Keeper 1.407 — Fishing Runtime Internals

This document contains reusable host/runtime facts promoted from accepted Bite Countdown research.

## Native wait ownership

**Status:** accepted fact.

When `FishingGUI.ChangeState(WaitingForBite)` runs, the selected fish is resolved and the native bite wait is stored in `FishingGUI._waiting_for_bite_delay`.

`FishingGUI.UpdateWaitingForBite()` does not consume that timer while `can_take_out == false`. Once `can_take_out == true`, vanilla subtracts `Time.deltaTime` until the timer reaches zero and transitions to `WaitingForPulling`.

Runtime research verified that `FishingThrowingAnim.OnStateExit` is the initial-cast transition that flips `can_take_out` from false to true without replacing the resolved wait.

**Evidence provenance:** Bite Countdown Research Probe 0.1.1, source `ac84f65d4be020d0322609023e3fdbdc339840fb`.

## Missed-hook re-entry

**Status:** accepted runtime fact.

A missed hook can transition directly:

`WaitingForPulling -> WaitingForBite`

without another throwing animation. Vanilla resolves another fish/wait and may repeat this loop until fishing ends.

Any observer/overlay tied to bite waits must therefore re-arm from the resulting `WaitingForBite` state itself rather than assuming every wait is preceded by a fresh throw animation.

## Bobber transform and visible sprite geometry

**Status:** accepted runtime fact.

Cross-distance probing established that cast distance is already represented by the native bobber transform. The visible float is offset within that transform by the active bobber sprite geometry.

At three tested cast distances, the bobber transform position changed while the sampled waiting-sprite tight mesh geometry remained the same.

Reusable implication: an overlay that follows the visible float should derive its anchor from the current bobber transform + sprite mesh/pivot geometry rather than maintain a location/distance table.

Further cross-spot testing established that left/right facing is owned by the native transform/orientation. A residual screen-right correction calibrated on one facing should be expressed in sprite-local coordinates so the native transform mirrors it correctly.

**Evidence provenance:** Bite Countdown Anchor Probe 0.2.2 and Anchor Audit 0.3.0; accepted production orientation carried by 1.0.3.

## Gameplay isolation lesson

The accepted Bite Countdown implementation can observe native fishing state/timing and own only a visual overlay without writing fishing state, wait duration, hook duration, fish selection/quality, bait/lure data, energy, catch amount, achievements/quests, or save data.

This is project architecture rather than a new host fact, but it is a useful proven integration pattern: read the game's authoritative wait/state and render downstream instead of shadowing/reimplementing fishing mechanics.

## Applicability limits

These findings target Graveyard Keeper 1.407 fishing. They do not establish lifecycle ownership for unrelated minigames or animations.
