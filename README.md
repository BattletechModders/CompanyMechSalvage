# CompanyMechSalvage
BattleTech mod (using BTML) that let's your own mechs getting destroyed permanently and instead rewards you with partial mech parts.

## Requirements
** Warning: Uses the experimental BTML mod loader that might change, come here again to check for updates **

* install [BattleTechModLoader](https://github.com/Mpstark/BattleTechModLoader/releases) using the [instructions here](https://github.com/Mpstark/BattleTechModLoader)

## Features
- Your own mechs can be permanently destroyed when they are damaged to badly.
- You get rewarded mech parts in the same way you get them from the enemy mechs.
- Those parts are added to your inventory directly, so you have not to use your salvage points.
- It is possible to select if losing your head counts as destroyed.

Examples:
One of your Mechs get's it's CT destroyed in a mission. At the end of the mission you will be rewarded one part of that mechtype and your Mech will be removed for your bay.

One of your Mechs get's both it's legs destroyed in a mission. At the end of the mission you will be rewarded two part of that mechtype and your Mech will be removed for your bay.

You punch out because you are scared to lose your pilot, therefore your head gets destroyed. At the end of the mission you will be rewarded two part of that mechtype and your Mech will be removed for your bay.

There are also currently 2 configuration settings available:

Setting | Type | Default | Description
--- | --- | --- | ---
RecoveryChance | float | default 0 | set this to anything between 0 and 1 to set a chance that your mechs will be not destroyed even when they lose a vital part. 0 = 0%, 1 = 100%
DestroyedWithHead | boolean | default true | set this to false if you don't want to count a destroyed head(f.e. punching out) as a reason to getting your mech destroyed.

## Download

Downloads can be found on [github](https://github.com/Morphyum/CompanyMechSalvage/releases).

## Install
- After installing BTML, put everything into \BATTLETECH\mods\ folder and launch the game.
- In \BATTLETECH\mods\CompanyMechSalvage you will find the settings.json in which you can change the settings.

