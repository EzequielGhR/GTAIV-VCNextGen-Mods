# GTAIV-VCNextGen-Mods
Guardando algunos mods que he hecho de manera publica porque me pinta.

## Pre-requisitos

- Precisas un ASI loader para tu version de GTA4 (GTA VC Next Gen es basicamente la version 1.0.7.0 asi que todos los pre requisitos son los mismos que esa version de GTA 4)
- Vas a precisar Scripthook.dll y ScriptHookDotNet.dll para tu version de GTA 4.
- OpenIV es necesario para cargar los modelos.

### Si usas linux sos de los mios

- Recomiendo usar lutris porque te hace la vida mil veces mas facil
- Vas a tener que instalar dotnet 4.7.2 con winetricks, podes hacerlo con la gui o el terminal, lo que a vos te guste mas mi rey.
- Si logras instalar dotnet estas del otro lado, instala scripthookdotnet en tu gta, entra al juego y asegurate que podes abrir la consola sin que crashee.
- Yo recomiendo instalar OpenIV en el mismo prefijo que el juego, podes hacerlo desde lutris o seteando la env var de `WINEPREFIX` antes de correr el instalador.
- OpenIV se instala en `%el prefijo%/drive_c/users/{%tu_usuario% o "steamuser" si estas usando proton}/AppData/Local/New Technology Studio/Apps/OpenIV`
- Podes correr OpenIV desde ese prefijo de nuevo corriendo un exe a traves de lutris o con la env var seteada, y ejecutando OpenIV.exe
- Si estas modeando la version de steam que tiene los archivos por separado del prefijo, o vos tenes el juego instalado en otro prefijo por algun motivo, considera crear un symlink en el prefijo de OpenIV a los archivos del juego de GTAIV, asi podes seleccionar el exe del juego cuando configures OpenIV sin referenciar otro disco que puede dar problemas.

## Estructura e instalacion

Deberia ser una carpeta por mod, y un readme por carpeta, pero en general es copiar un script a la carpeta script de tu carpeta del juego.

## Frikada Historica

- Veras que la mayoria de los scripts son con Tick events y no KeyDown, hay dos motivos.
    - Tick Events son lo mejor que hay y la opinion de los demas me chupa un huevo.
    - Juego y moddeo en linux asi que no tengo `Windows.Forms` para usar keydowns de manera simple.

- Si planeas escribir scripts de dotnet en linux, te recomiendo instalar VSCode con la extension de C# dev kit, te hace la vida mas facil, y creeme que yo soy ultra fan de helix, pero la verdad es la verdad.
