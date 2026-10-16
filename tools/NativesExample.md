# List all Available natives

`./natives_tool.sh list`

## Reduced list example

```
./natives_tool.sh list | grep "RADIO"  
DISABLE_FRONTEND_RADIO
ENABLE_FRONTEND_RADIO
FREEZE_RADIO_STATION
GET_PLAYER_RADIO_STATION_INDEX
GET_PLAYER_RADIO_STATION_NAME
MUTE_GAMEWORLD_AND_POSITIONED_RADIO_FOR_TV
RENDER_RADIOHUD_SPRITE_IN_LOBBY
RETUNE_RADIO_TO_STATION_INDEX
RETUNE_RADIO_TO_STATION_NAME
TURN_ON_RADIOHUD_IN_LOBBY
  
```

# Fetch specific native

`./natives_tool.sh [FUNCTION_NAME]` or `./natives_tool.sh [FUNCTION_NAME] force`

## Void Return type and no parameters

```
./natives_tool.sh DISABLE_FRONTEND_RADIO
Fetching data for 'DISABLE_FRONTEND_RADIO'. Force: False
{'function': 'DISABLE_FRONTEND_RADIO', 'return_type': 'VOID', 'return_description': '', 'parameters': None, 'description': 'This function disables the main vehicle radio. The radio will fade out instead of instantly turning off. Other radio sources like background music or the faint radio heard outside of the vehicle will still be heard. The radio can be reenabled using ENABLE_FRONTEND_RADIO.\n', 'warning_message': None}

```

## Return type with parameters

```
./natives_tool.sh START_PTFX_ON_OBJ force
Fetching data for 'START_PTFX_ON_OBJ'. Force: True
{'function': 'START_PTFX_ON_OBJ', 'return_type': 'INT', 'return_description': 'Handle to particle effect instance', 'parameters': [{'type': 'STRING', 'description': 'Particle effect name', 'raw_type': 'string'}, {'type': 'INT', 'description': 'Object id', 'raw_type': 'int'}, {'type': 'FLOAT', 'description': 'X-offset', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Y-offset', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Z-offset', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Yaw', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Pitch', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Roll', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Effect scale (1.0f = standard)', 'raw_type': 'float'}], 'description': 'Starts and sticks a  particle effect to an object. (effect follows object around)\n', 'warning_message': None}

```

## With Warnings

```
./natives_tool.sh "UPDATE_PTFX_OFFSETS" 
Fetching data for 'UPDATE_PTFX_OFFSETS'. Force: False
{'function': 'UPDATE_PTFX_OFFSETS', 'return_type': 'VOID', 'return_description': '', 'parameters': [{'type': 'INT', 'description': 'Handler of PTFX', 'raw_type': 'int'}, {'type': 'FLOAT', 'description': 'X-offset', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Y-offset', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Z-offset', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Yaw', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Pitch', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Roll', 'raw_type': 'float'}], 'description': 'Update PTFX offset. Works for all created PTFX.\n', 'warning_message': 'Expected parameters: 9. Found: 7. Documentation incomplete'}
```

## Non-existent native

```
./natives_tool.sh THIS_NATIVE_DOES_NOT_EXIST
Fetching data for 'THIS_NATIVE_DOES_NOT_EXIST'. Force: False
There was an error fetching data: The requested native does not exist. Requested: THIS_NATIVE_DOES_NOT_EXIST. Cleaned: THIS_NATIVE_DOES_NOT_EXIST
```

# Formatting a native for dotnet scripthook

## Example 1: START_PTFX_ON_OBJ

DATA:
```
{'function': 'START_PTFX_ON_OBJ', 'return_type': 'INT', 'return_description': 'Handle to particle effect instance', 'parameters': [{'type': 'STRING', 'description': 'Particle effect name', 'raw_type': 'string'}, {'type': 'INT', 'description': 'Object id', 'raw_type': 'int'}, {'type': 'FLOAT', 'description': 'X-offset', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Y-offset', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Z-offset', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Yaw', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Pitch', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Roll', 'raw_type': 'float'}, {'type': 'FLOAT', 'description': 'Effect scale (1.0f = standard)', 'raw_type': 'float'}], 'description': 'Starts and sticks a  particle effect to an object. (effect follows object around)\n', 'warning_message': None}
```

CS CODE:
```
// cigarObj is of type GTA.Object, native call uses object handle automatically
pfxHandle = GTA.Native.Function.Call<int>(
    "START_PTFX_ON_OBJ",
    "ambient_cig_smoke",
    cigarObj,
    0.125f,
    -0.02f,
    0.01f,
    0.0f,
    0.0f,
    0.0f,
    1.1f
);
```

## Example 2: IS_AMBIENT_SPEECH_PLAYING

DATA:
```
{'function': 'IS_AMBIENT_SPEECH_PLAYING', 'return_type': 'BOOL', 'return_description': 'Unknown', 'parameters': [{'type': 'INT', 'description': 'Ped Handle', 'raw_type': 'Handle'}], 'description': 'The results of this function are either unknown or untested.\n', 'warning_message': None}
```

CS CODE:
```
// ped is of type GTA.Ped, native call uses ped object by default.
GTA.Native.Function.Call<bool>("IS_AMBIENT_SPEECH_PLAYING", ped)
```

## Example 3 STOP_PTFX

DATA:
```
{'function': 'STOP_PTFX', 'return_type': 'VOID', 'return_description': '', 'parameters': [{'type': 'INT', 'description': 'Particle Effect handle', 'raw_type': 'int'}], 'description': 'Stops the specified  particle effect instance.\n', 'warning_message': None}
```

CS CODE:
```
GTA.Native.Function.Call("STOP_PTFX", ptfxHandle);
```