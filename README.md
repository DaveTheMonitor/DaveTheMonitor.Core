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

### Weather Module

Status: In Development

Allows mods to add new weather.

### Biomes Module

Status: In Development

Allows mods to add biomes that naturally generate in the world. These biomes generate based on temperature and precipitation, and can change the blocks, terrain, and decorations of the world.