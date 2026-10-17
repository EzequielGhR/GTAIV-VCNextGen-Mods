# Que hace?

Permite que Niko (o Luis o Johnny si toqueteas el codigo un poco), se convierta en ciudadano de la Republica Oriental del Uruguay, asi que puede andar caminando con su Termo y Mate (o chimarrão dijeran los vecinos gauchos del sur de brazil).
- Apreta NavDown + Sprint (Abajo + A con control) para spawnear tu termo y tu mate.
- Apreta NavLeft (Izquierda) para chupar la bombilla (yummy), todavia no hay ruidito de mate, pero cada trago recarga 2HP.
- Apreta de nuevo NavDown + Sprint para tirar el termo y el mate al piso.
- Mientras tomas mate podes caminar, correr y nadar, pero no podes cambiar de arma.
- Si, podes nadar mientras tomas mate, es muy gracioso, trata de darle un traguito mientras flotas en el agua es un cago de risa.
- Si algo interrumpe a Niko mientras toma mate, va a rajarse a puteadas.

# Como se instala

## Requisitos
Obviamente necesitas ScriptHookDotNet.dll en tu carpeta del juego, pero tambien necesitas NAudioAdvance-IV de [releases de Lorenzo3024it](https://github.com/Lorenzo3024it/NAudioAdvanced-IV/releases)

## Agrega los modelos

Los modelos a reemplazar son `amb_icecone01` y `amb_juice_bot`, asi que hace respaldo por las dudas.

Si usas `FusionFix` (recomendado a full), podes copiar `weapons.img` a `update/pc/models/cdimages` y editar ese archivo.

Voy a referenciar la ruta relativa al root de la carpeta de tu juego o a la carpeta update, vos decidis cual usas.

- Abri OpenIV, anda a `pc/models/cdimages` y abri `weapons.img`
- Busca `amb_juice_bot.wdr` y `amb_juice_bot.wtd`, hace un respaldo de ambos (click izquierdo, extraer)
- Hace lo mismo para `amb_icecone01.wdr` y `amb_icecone01.wtd`
- Ahora estas listo para meter los modelos en `models_for_openiv`. Dale click derecho al archivo wdr en OpenIV y reemplaza por los que estan en este mod.
- No borres los archivos wtd, el juego cuenta con que existan, incluso si mis modelos ya tienen las texturas embebidas.

## Agrega el script

Este script necesita ser compilado porque no he encontrado la manera de referenciar NAudioAdvanced-IV.dll en cs crudo, si alguien sabe, por favor ilustreme.

### Compilar

Podes hacerl en Visual Studio si usas Windows, si sos un usuario de linux como yo, tenes que instalar mono para usar el `csc` compiler (en arch `pacman -S mono`).

Tambien inclui [linux_build.sh](./linux_build.sh) que podes correr luego de instalar mono, va a usar `csc` o `msc` para compilar y linkear dlls.

### Instalar

Despues de compilar, copia la carpeta `MateSounds` y el dll `MateScript.net.dll` a tu carpeta scripts.

# Sobre los modelos

Son una reverenda verga porque son los primeros modelos que hice en mi vida, la primera vez que use blender en mi vida. Pero si alguien quiere mejorarlos es bienvenido.

Estan tambien los originales obj/mtl para cualquier ducho que quiera mejorarlos, pero mira que el codigo esta tweakeado para su precisa posicion y medida (Era mas facil iterar en eso que en el modelado 3d xd).

Si queres convertir tus modelos al formato de GTA4 tene en cuenta que tu unica chance es encontrarte un 3dsmax 2012 de ultra tumba con el GIMS IV plugin, y solo anda en windows... si, una paja.
