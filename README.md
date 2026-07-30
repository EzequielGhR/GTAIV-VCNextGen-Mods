# GTAIV-VCNextGen-Mods
Publicly storing some basic mods I've been doing with scripthookdotnet for VC Next gen edition

## Prequisites

- You will need an ASI loader for your version of GTA 4 (VC Next Gen 1.2 uses the GTA4 1.0.7.0 keep that in mind when downloading prequisites for that).
- Also you will need Scripthook.dll and ScriptHookDotNet.dll for your version of GTA 4.
- OpenIV is needed for the models.

### If using Linux

- I recomend using lutris, makes prefix management and runners management easier.
- You will need to install dotnet 4.7.2 using winetricks.
- If you manage to install dotnet then you're golden, install scripthookdotnet and try entering the game and opening the console to make sure its all working.
- Additionally I recomend installing OpenIV in the same prefix, you can do that from lutris by running "Run an arbitrary exe inside the prefix" and running the OpenIV installer
- OpenIV installs to %the prefix%/drive_c/users/{%your_user% or "steamuser" if using proton}/AppData/Local/New Technology Studio/Apps/OpenIV
- You can run OpenIV from that prefix by again running an exe in the prefix and walking the path to OpenIV.exe
- If you have the game files on another location outside the prefix, consider doing a symlink inside the prefix to the game files, it will let you select the game
location inside open iv.

## Structure and installation

It should be a folder per mod, and I'll try to include a readme per mod, but hell it's usually just copying stuff to the scripts folder of your game

## Nerd Stuff

- You may notice almost all scripts are Tick scripts, not a single KeyDown. The reasons are two:
    - Tick Events are superior
    - I play and mod on linux, so I don't have Windows.Forms to deal with keys

- If you plan to write dotnet scripts on linux, I encourage you to use VSCode and install the C# dev kit, it makes your life super simple. And believe me I am helix guy, but still for C# this is the best way.
