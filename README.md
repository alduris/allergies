# Rain World Allergies
Adds allergies to Rain World, with varying triggers and reactions. Customizable. Extensible.

## Downloading
You can download the mod through the [Steam Workshop]() or through the releases tab in the sidebar.

## API
This mod allows you to code your own new allergen types, reactions, and triggers. [documentation coming soon]

How does the mod work?
- There is a global list of allergies that the mod picks from to apply to the player.
- Certain things trigger the allergy system. When they trigger on a player, they loop through all active allergies on the player, and if the allergen criteria matches, it applies a reaction.
- The reaction is the consequence. It is attached to the player, and has overridable methods that activate during a physics update and during a draw update.

### Allergens
documentation coming soon

### Triggers
documentation coming soon

### Reactions
documentation coming soon