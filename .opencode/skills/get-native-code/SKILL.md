---
name: get-native-code
description: Use when the user wants to find a specific native and get its C# code counterpart.
---

# Get Native Code

Use this skill to fetch details for a specific GTA native and generate C# code for it.

## How it works
1. It attempts to fetch the native data using `./tools/natives_tool.sh [NATIVE_NAME]`.
2. If the native does not exist, it identifies similar natives and asks the user to choose one.
3. Once a native is confirmed, it generates the C# code in `scripthookdotnet` format based on the provided data.

## Reference
See `@tools/NativesExample.md` for examples of C# code formatting.

## Example
User: "Get the code for START_PTFX_ON_OBJ"
Agent: [Fetches data, generates C# code]
