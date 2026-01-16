# Rain World Allergies
Adds allergies to Rain World, with varying triggers and reactions. Customizable. Extensible.

## Downloading
You can download the mod through the [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3647711384) or through the releases tab in the sidebar.

## API
This mod allows you to code your own new allergen types, reactions, and triggers.

How does the mod work?
- There is a global list of allergies that the mod picks from to apply to the player.
- Certain things trigger the allergy system. When they trigger on a player, they loop through all active allergies on the player, and if the allergen criteria matches, it applies a reaction.
- The reaction is the consequence. It is attached to the player, and has overridable methods that activate during a physics update and during a draw update.

### Allergens
Allergens are implemented with the `IAllergen` interface and registered via `AllergySystem.Register(IAllergen)`.

`IAllergen` has the following items which must be implemented:
* `Name`: The display name of the allergen
* `GetIcon()`: An icon to display, or null if there is none. Currently unused, but may be in the future.
* `MatchesCriteria(PhysicalObject? thing, TriggerType trigger)`: Determines if a trigger matches the criteria of the allergen.

There are several base classes of simple allergens so that it is not necessary to make your own implementation of `IAllergen`:
* `SimpleEdibleItemAllergen<T>` or `SimpleEdibleCreatureAllergen<T>` for allergens that trigger on eating a thing
* `SimpleAirborneItemAllergen<T>` or `SimpleAirborneCreatureAllergen<T>` for allergens that trigger around a thing
* `SimpleTouchItemAllergen<T>` or `SimpleTouchCreatureAllergen<T>` for allergens that trigger on touching a thing

These base classes can simply be instantiated with a type and then passed to `AllergySystem.Register(IAllergen)`.

### Triggers
Triggers are implemented via a call to `AllergySystem.TriggerAllergy(Player, PhysicalObject?, TriggerType)`. A custom `TriggerType` can be created fairly easily if needed, as it is an `ExtEnum`; this may be necessary for cases where an allergy needs to trigger without a source physical object. A reference to the player is required.

### Reactions
Reactions are implemented with the `Reaction` abstract class. You are required to implement the `Update()` and `DrawSprites()` methods and `IsStillActive` property, with additional methods that can be overridden being `InitiateSprites()` and `Destroy()`. Reactions are instances created when an `IAllergen` triggers. They are assigned to a player (you can get the realized `Player` via the `player` property and its `AbstractCreature` using the `abstractPlayer` field) and multiple reactions, including of the same type, can be applied to a player at the same time, which is something you must keep in mind while implementing. If only one instance of a particular reaction should be allowed to be active at a given time, you can keep track on your own and `Destroy` a reaction as soon as it is created. Some built-in reactions do this, including `HivesReaction`.

You may also change how frequently a reaction can be applied by setting the `setCooldown` field in the constructor. It defaults to 5 seconds. This cooldown represents the amount of time that may pass in between creating a new instance of a reaction in ticks (that is, typically 40 per second).
