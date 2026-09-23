# Graveyard Keeper Research

Shared cross-mod research and knowledge base for **Graveyard Keeper 1.407**.

This repository stores reusable verified facts about game/runtime internals, cross-project diagnostics, and research evidence that can serve multiple NikichMods Graveyard Keeper projects. Production fixes and project-specific behavior still belong in the repository that owns them.

## Working model

1. **Global engineering baseline** — `NikichMods/DevRules` (`ENGINEERING_RULES.md`, `CI_POLICY.md`, `GIT_WORKFLOW.md`, `PROJECT_BOOTSTRAP.md`).
2. **Shared host/runtime knowledge** — this repository, beginning with `docs/RESEARCH_INDEX.md`.
3. **Owning mod contract** — the target repository's local `AGENTS.md` and canonical project docs.
4. **Fresh investigation only when needed** — inspect source/assemblies or build a narrow probe only after prior accepted research has been checked.

## Canonical shared knowledge

- `docs/RESEARCH_INDEX.md` — entry point for reusable Graveyard Keeper 1.407 research.
- `docs/GAME_INTERNALS.md` — distilled cross-project facts about game/UI/runtime behavior.
- `docs/PERFORMANCE_EVIDENCE.md` — performance-specific accepted facts, hypotheses, ruled-out causes, root causes, and cross-project conclusions.
- `docs/TEST_LOG.md` — controlled runtime tests and supplied diagnostic evidence.

Commit history, raw logs, and one-off candidate notes are evidence archives. When a result is accepted and likely to matter again, promote the durable conclusion into the appropriate canonical document and link the supporting evidence.

## Performance diagnostics

Performance investigation remains an explicit supported workflow:

`reproduce -> isolate -> measure/verify -> root cause -> narrow fix in owning repo -> retest`

`docs/CHATGPT_PROJECT_INSTRUCTIONS.md` is the canonical thin ChatGPT Project bootstrap. `docs/CHAT_START_PROMPT.md` is only a fallback for chats outside that configured Project; routine new chats inside the Project should start directly with the actual task.

## Ownership and research-material boundary

Cross-project host/runtime facts belong here when they are reusable beyond one mod. Project-specific mechanics, UX decisions, balance values, release state, build identity, and acceptance evidence stay in the owning project.

This repository remains **public by design** and serves as the canonical shared knowledge layer. That does not limit local research: assemblies, decompiled code, resources, runtime dumps, and similar host material may be inspected temporarily when the working environment permits it. The persistence rule is narrower: do not commit copied game assemblies, full/bulk decompiled game source, extracted proprietary assets, or other third-party payloads. Preserve derived facts, identifiers, signatures, formulas, hashes, bounded evidence, and original research tooling instead.

A private scratch repository should be created only for a concrete need such as sensitive environment data or large first-party diagnostics. It is not required merely to make reverse engineering easier, and it does not replace this public canonical knowledge base.
