# Fresh Chat Start Prompt — fallback only

This file is **not required for normal new chats inside a correctly configured ChatGPT Project**.

Canonical ChatGPT Project settings are stored in:

`docs/CHATGPT_PROJECT_INSTRUCTIONS.md`

Inside that Project, start a new chat with the actual task. The Project Instructions require ChatGPT to recover current state from GitHub and accepted evidence.

Use the fallback block below only when:
- the chat is outside the configured ChatGPT Project;
- Project Instructions are temporarily unavailable;
- you are migrating/recovering the project setup.

```text
We are continuing Graveyard Keeper 1.407 research.

Repository: NikichMods/GraveyardKeeperResearch
Global rules: NikichMods/DevRules

Before substantive work, inspect the current repository, read its AGENTS.md, DevRules, docs/RESEARCH_INDEX.md and relevant accepted evidence. If a production mod becomes the concrete owner/suspect, inspect that mod's current repository and local contract before attributing or changing behavior.

Repository evidence outranks chat memory. Use the DevRules evidence gates, avoid repeating accepted research, keep production fixes in the owning mod repository, and ask me only for runtime evidence that genuinely requires my installed game.
```
