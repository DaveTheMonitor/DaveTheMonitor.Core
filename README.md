# Core

Core is a Total Miner mod that serves as an expanded API for other mods. The mod doesn't do anything by itself, but has several modules that each add new functionality. Mods can depend on any number of modules. All base modules are fully compatible with each other, some including extra features when used with other modules (eg. ActorEffects adding new Script functions).

Below is a list of all modules, as well as their status:

## Modules

### Script Module

Status: In Development (Available Early)

Adds full CSRScript compatibility, and allows other mods to add functions and types.

Currently available but unfinished.

### Particles Module

Status: Early Build

Adds a new particle system. Particles can be defined with C# or Json and can use textures from the mod.

### Effects Module

Status: Early Build

Allows mods to add new Actor Effects that can be given to players and NPCs. Effects can be defined with C# or Json and can use icon and background textures from the mod. The Effects module has support for the Particles module.

### Magic Module

Status: In Development

Allows mods to add items that can be used in the new custom magic system.

### Combat Module

Status: In Development

Reworks combat to be less RNG-based by making attacks deal set damage, lowered by defense.

### Skill Module

Status: In Development

Reworks skills to use a point-based system.

### Boss Module

Status: In Development

Allows certain NPCs to be "boss" enemies, which show a health bar on screen and can have special dialogue.

### Vehicle Module

Status: In Development

Allows creating vehicles that can be driven by players.

### Weather Module

Status: In Development

Allows mods to add new weather.

### Biomes Module

Status: In Development

Allows mods to add biomes that naturally generate in the world. These biomes generate based on temperature and precipitation, and can change the blocks, terrain, and decorations of the world.