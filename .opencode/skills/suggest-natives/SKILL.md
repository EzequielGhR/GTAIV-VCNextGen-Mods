---
name: suggest-natives
description: Use when the user wants to see a filtered list of available GTA natives (e.g., "Show me all RADIO natives").
---

# Suggest Natives

Use this skill to find specific GTA natives by filtering the full list.

## How it works
It runs `./tools/natives_tool.sh list` and pipes the output through `grep` with your search term to provide a concise list.

## Example
User: "What are the radio natives?"
Agent: [Runs `tools/natives_tool.sh list | grep "RADIO"` and presents the result]
