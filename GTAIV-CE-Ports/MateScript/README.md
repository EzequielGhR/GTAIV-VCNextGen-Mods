# What does it do

Allows Niko (or maybe Luis or Johnny with some tweaks), to become a citizen of the Oriental Republic of Uruguay, so he can walk around with his Thermos and Mate/Chimarrao.
- Press NavDown + Sprint (Down + A for controllers) to spawn the thermos and mate.
- Press NavLeft (Left) to take a sip, each sip recharges 2HP.
- Press Again NavDown + Sprint to throw them away.
- While drinking mate you can run, walk and swim but you cannot switch weapons.
- Yes you can swim while drinking mate is super funny, try taking a sip while floating on water.
- If something interrupts Tommy's Niko's drinking, he will curse.

# How To Install

## Requirements
Of course ScriptHookDotNet.dll has to be in your game directory, but also NAudioAdvance-IV dlls from [Lorenzo3024it releases](https://github.com/Lorenzo3024it/NAudioAdvanced-IV/releases)

## Add the models

The models replace `amb_icecone01` and `amb_juice_bot` models, so make sure to make backups just in case.

If you are using `FusionFix` (which I highly recomend), you can copy `weapons.img` to `update/pc/models/cdimages` and edit that file.

I will reference paths relative to the root or update folder, your choice on which to use (I recomend the update folder).

- Open OpenIV, go to `pc/models/cdimages` and open `weapons.img`
- Find `amb_juice_bot.wdr` and `amb_juice_bot.wtd`, make backup of both just in case (right click extract)
- Do the same with `amb_icecone01.wdr` and `amb_icecone01.wtd`
- Now you are ready to replace the models by the ones in `models_for_openiv`. Just right click the wdr file and replace by the ones in this mod.
- Do not delete the wtd files, the games counts on them existing even tho my models have the textures embedded already so they are functionally ignored.

## Add the script
This script requires compilation cause I have not found a way to reference NAudioAdvanced-IV.dll in plain cs, if anyone knows how to do that please ilustrate.

### Compile

You can do that in Visual Studio if on Windows, if you are a linux user like me, you need to install mono to use `csc` compiler (`pacman -S mono` in arch).

Aditionally I have a [linux_build.sh](./linux_build.sh) script you can run after installing mono, it will use `csc` or `msc` compiler and link the dlls.

### Install

After Compiling, copy `MateSounds` folder and `MateScript.net.dll` to your scripts folder.

# About the models

They are really low poly cause it was my first time using blender in my life, if anyone wants to improve them be my guest.

I will provide the original obj/mtl format files for anyone willing to make improvements, but keep in mind the code is tweaked for their precise
measures and position (I iterated a lot on the position cause it was easier than iterating on the model).

If you want to convert your 3d models for GTA 4, keep in mind the only possible way is 3dsmax 2012 with the GIMS IV script.. yeah.. painful.