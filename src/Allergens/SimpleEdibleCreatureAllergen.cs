using RWCustom;

namespace Allergies.Allergens
{
    public class SimpleEdibleCreatureAllergen<T>(CreatureTemplate.Type creatureType, int iconIntData = 0) : EdibleAllergen<T> where T : Creature
    {
        private readonly CreatureTemplate.Type creatureType = creatureType;
        private readonly int iconIntData = iconIntData;

        public override string Name => Custom.rainWorld.inGameTranslator.TryTranslate($"creaturetype-{creatureType.value}", out string translated) ? translated : creatureType.value;

        public override FSprite GetIcon()
        {
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(new IconSymbol.IconSymbolData(creatureType, null, iconIntData)))
            {
                color = CreatureSymbol.ColorOfCreature(new IconSymbol.IconSymbolData(creatureType, null, iconIntData)),
            };
        }
    }
}
