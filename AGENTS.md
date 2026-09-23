# Graveyard Keeper Research — Working Contract

This repository follows the canonical global development rules in `NikichMods/DevRules`.

Before substantive technical work, read:

- `ENGINEERING_RULES.md`
- `CI_POLICY.md`
- `GIT_WORKFLOW.md`
- `PROJECT_BOOTSTRAP.md`

This local `AGENTS.md` contains only project-specific additions, constraints, and explicit exceptions.

## Project identity

- Project: **Graveyard Keeper Research**
- Repository: `NikichMods/GraveyardKeeperResearch`
- Game: **Graveyard Keeper 1.407**
- Environment: modded PC installation using BepInEx and multiple independently maintained mods
- Purpose: preserve reusable, evidence-backed knowledge about Graveyard Keeper internals and cross-mod behavior, and host diagnostic research that does not belong to one production mod.

This is a shared research/knowledge repository, not a production mod and not the canonical production repository for other mods.

## Scope

This repository has two related roles:

1. **shared knowledge base** — reusable verified facts about Graveyard Keeper 1.407 game/runtime internals, UI lifecycle, data ownership, APIs, schemas, formulas, and integration seams that may matter to multiple mods;
2. **cross-mod diagnostics** — investigations such as performance regressions, compatibility interactions, ownership disputes, or runtime behavior that is not yet attributable to one production repository.

Performance diagnostics keep the workflow:

`reproduce symptom -> isolate trigger/owner -> measure or establish evidence -> identify root cause -> fix in the owning repository -> retest the original scenario`

The repository may investigate:

- vanilla/game-engine behavior;
- UI/NGUI lifecycle and layout ownership;
- BepInEx/plugin loading and lifecycle interactions;
- data definitions, identifiers, formulas, localization seams, item/inventory behavior, FlowCanvas graphs, buffs/effects, and other reusable host internals;
- one mod in isolation;
- interactions between multiple mods;
- excessive allocations, object creation, resource scans, reflection, logging, serialization, polling, Harmony patch behavior, scene/HUD recreation, loading-time work, and event storms;
- regressions that appeared after recent mod or configuration changes.

Do not turn this repository into a grab-bag production mod. If a proven behavior or defect belongs to a specific mod, implement the production change in that mod's own repository under its own `AGENTS.md`, versioning, test-build log, and acceptance rules. Record reusable host facts here and link the owning source state/fix when useful.

## Mandatory start-of-work checks

Before substantive diagnostic or code/GitHub work:

1. inspect the current `GraveyardKeeperResearch` repository and read this `AGENTS.md`;
2. read the current global DevRules contract;
3. start with `docs/RESEARCH_INDEX.md`, then inspect the relevant canonical knowledge document;
4. inspect `docs/PERFORMANCE_EVIDENCE.md` and `docs/TEST_LOG.md` when performance/diagnostic history is relevant;
5. inspect accepted history/test evidence before repeating research that may already have been performed;
6. inspect the current repository/source/history of every mod that becomes a concrete suspect or production owner before attributing a defect or changing it;
7. read that mod repository's local `AGENTS.md` before substantive work in it;
8. prefer accepted existing evidence over repeating expensive or intrusive tests.

Do not start from chat memory when repository evidence can answer the question.

## Knowledge promotion contract

Commit history, raw logs, candidate notes, decompilation notes, and test logs are evidence archives, not the primary knowledge base.

When a result is accepted and likely to be reusable across Graveyard Keeper projects, promote it into a canonical shared document and add or update its entry in `docs/RESEARCH_INDEX.md`.

A durable shared fact should identify, when applicable:

- target game/runtime version;
- status: **fact**, **hypothesis**, **accepted result**, or another explicit evidence state;
- canonical owner/path or lifecycle boundary;
- verified behavior;
- supporting source/runtime evidence or exact source identity;
- applicability limits and what must **not** be generalized;
- known project consumers when useful.

If a finding is specific to one mod's product behavior, balance, UX, release state, or acceptance, keep the canonical copy in that mod repository instead. The shared research repository may link to it, but should not become a second competing source of truth.

Before authoring a new probe, follow:

`owning project canonical docs -> RESEARCH_INDEX/shared facts -> accepted logs/history -> fresh static inspection -> narrow probe only if still needed`

## Evidence contract


Never guess Graveyard Keeper or mod internals.

A performance claim must distinguish clearly between:

- **observed fact** — directly present in logs, measurements, source, runtime evidence, or a controlled A/B test;
- **working hypothesis** — plausible but not yet proven;
- **root cause** — supported strongly enough that a targeted change or controlled test predicts the result;
- **accepted result** — confirmed by the user's real game environment when runtime confirmation is required.

Preferred evidence order:

1. reproducible symptom and controlled comparison;
2. current source/configuration/history of the suspected owner;
3. existing logs and accepted project diagnostics;
4. direct assembly/resource/runtime inspection;
5. narrow instrumentation or probe;
6. user-supplied in-game evidence when only the running game can prove the behavior.

Do not infer causality merely because a warning/error appears near a freeze in a log. Establish temporal or behavioral correlation and, where practical, an A/B result.

## Diagnostic method

Prefer the cheapest test that can materially narrow the cause.

Typical progression:

1. define the exact symptom and a short reproducible scenario;
2. preserve the current mod/config baseline;
3. compare against the nearest meaningful control, such as clean game, BepInEx-only, or the same setup with one suspected mod disabled;
4. use grouped/binary isolation when many mods are plausible suspects;
5. inspect source before adding instrumentation when the likely hot path is already visible;
6. if instrumentation is needed, make it narrow, bounded, and removable;
7. measure frequency and duration rather than relying only on subjective severity when practical;
8. remove/disable temporary diagnostics after the question is answered.

Avoid broad per-frame logging, stack dumping on every call, global object enumeration in recurring paths, or diagnostics that can create the very stalls being investigated.

When comparing test runs, change one material variable at a time unless the test is explicitly a coarse isolation step.

## Performance-specific rules

Treat frame-time and loading-time problems separately.

For runtime microfreezes, inspect first for work that can block the main Unity thread, including:

- broad `Resources.FindObjectsOfTypeAll` / `FindObjectsOfType` scans;
- repeated hierarchy searches;
- repeated FlowCanvas/quest graph parsing;
- frequent reflection or enumeration of large collections;
- allocations and LINQ in recurring hot paths;
- synchronous file I/O;
- excessive logging;
- repeated object/component creation/destruction;
- expensive Harmony prefixes/postfixes on high-frequency methods;
- accidentally multiplied subscriptions/timers after scene or save reloads.

These are suspect categories, not assumptions of guilt. Verify them in the actual source/runtime before concluding.

For startup/loading stalls, distinguish one-time prewarm/cache construction from recurring gameplay cost. A large one-time operation is not automatically a microfreeze root cause unless it occurs during the reported gameplay scenario or repeats unexpectedly.

## Cross-repository fix ownership

Once a root cause is assigned to a specific mod:

- switch to that mod's current repository and read its current `AGENTS.md` plus relevant docs/history;
- create the appropriate dev/fix branch according to that repository's workflow;
- implement only the proven/narrow fix there;
- use that mod's version and handoff rules;
- do not merge runtime changes to its stable branch without its required acceptance;
- record the diagnostic link/result back in this repository.

A finding in this repository does not override another repository's project contract.

## Git / version / acceptance workflow

This research repository is evidence-first and normally does not distribute a stable DLL.

- `main` may contain accepted research documentation, diagnostic scripts, and non-production tooling.
- Use `research/<topic>` for substantial experiments or temporary probes when separation improves rollback/review.
- Documentation-only evidence updates may go directly to `main` when no runtime artifact is affected.
- Do not consume production mod version numbers here.
- If a diagnostic DLL/probe must be handed to the user, give it an explicit diagnostic identity, freeze its exact source state, record its purpose and hash in `docs/TEST_LOG.md`, and never present it as a normal mod release.
- Production fixes and numbered releases belong to the owning mod repository.

## CI / resource policy

Hosted CI is normally unnecessary for this repository because most work is research, logs, docs, static inspection, and controlled in-game testing.

Do not add automatic CI or spend hosted runner minutes for routine research/bookkeeping. Use hosted CI only when a concrete diagnostic tool or reproducible build artifact genuinely needs compilation in a hosted environment and no cheaper equivalent proves the required property.

For a production mod fix, follow that mod repository's own CI policy rather than inventing a parallel build pipeline here.

## Research inputs, repository visibility, and proprietary game material

This repository is intentionally the **public canonical knowledge base**. Public visibility must not restrict what may be investigated in the working environment; it restricts what is persisted here.

Allowed research inputs, when the working environment permits them, include local/temporary inspection of:

- Graveyard Keeper assemblies and other binaries;
- decompiled code;
- localization/resources;
- generated dumps, runtime state, reflection/IL output, and extracted metadata;
- temporary diagnostic artifacts needed to answer a concrete research question.

These inputs may be inspected, transformed, compared, and used to derive evidence. They do **not** need to be committed to Git in order for the research to be valid.

Do not commit copied Graveyard Keeper assemblies, full/bulk decompiled game source, extracted proprietary assets, or other copyrighted game payloads to this repository. Preserve the durable result as minimal derived facts, identifiers, signatures, formulas, hashes, original scripts/probes, bounded diagnostic output, and conclusions needed for reproducibility.

Repository privacy is not treated as permission to store third-party proprietary payloads. Do not make this repository private merely to relax the research process.

If a concrete need later appears for a private scratch/evidence workspace (for example, sensitive local-environment data or large first-party generated diagnostics), create it separately and narrowly. It is temporary evidence storage, not a competing source of truth. Accepted reusable findings must still be promoted back into this public repository's canonical docs.

## Long-lived sources of truth

Consult and maintain:

- `AGENTS.md` — this shared-research contract;
- `docs/RESEARCH_INDEX.md` — canonical entry point for reusable cross-project research;
- `docs/GAME_INTERNALS.md` — distilled reusable Graveyard Keeper 1.407 internals;
- `docs/PERFORMANCE_EVIDENCE.md` — performance-specific verified facts, active hypotheses, ruled-out causes, and cross-project conclusions;
- `docs/TEST_LOG.md` — controlled diagnostic/runtime test history and supplied evidence;
- `docs/CHATGPT_PROJECT_INSTRUCTIONS.md` — canonical thin bootstrap for this ChatGPT Project's settings field;
- `docs/CHAT_START_PROMPT.md` — fallback bootstrap only for chats outside a correctly configured ChatGPT Project;
- the current repositories and local contracts of any mods under investigation or ownership.

When chat memory conflicts with accepted repository evidence, investigate the conflict before changing code or asking the user to repeat a test.
