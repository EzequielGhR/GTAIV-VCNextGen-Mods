# What does it do

Allows Tommy (or maybe niko with some tweaks), to become a citizen of the Oriental Republic of Uruguay, so he can walk around with his Thermos and Mate/Chimarrao.
- Press NavUp + Sprint (Up + A for controllers) to spawn the thermos and mate.
- Press NavLeft (Left) to take a sip, each sip rechareges 2HP.
- Press Again NavUp + Sprint to throw them away.
- While drinking mate you can run, walk and swim but you cannot switch weapons.
- Yes you can swim while drinking mate is super funny, try taking a zip while floating on water.
- If something interrupts Tommy's (or Niko's) drinking, he will curse.

# How To Install

## Add the models

The models replace amb_icecone01 and amb_juice_bot models, so make sure to make backups just in case.
- Open OpenIV, go to pc/models/cdimages and open weapons.img
- Find amb_juice_bot.wdr and amb_juice_bot.wtd, make backup of both just in case (right click extract)
- Do the same with amb_icecone01.wdr and amb_icecone01.wtd
- Now you are ready to replace the models by the ones in models_for_openiv. Just right click the wdr file and replace by the ones in this mod.
- Do not delete the wtd files, the games counts on them existing even tho my models have the textures embedded already so they are functionally ignored.

## Add the script

Just copy MateScript.cs to your game location/scripts

# About the models

They are really low polly cause it was my first time using blender in my life, if anyone wants to improve them be my guess.
I will provide the original obj/mtl format files for anyone willing to make improvements, but keep in mind the code is tweaked for their precise
measures and position (I itereated a lot on the position cause it was easier than iterating on the model).

If you want to convert your 3d models for GTA 4, keep in mind the only possible way is 3dsmax 2012 with the GIMS IV script.. yeah.. painful.