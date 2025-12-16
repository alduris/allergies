using RWCustom;

namespace Allergies.Allergens
{
    public class SimpleEdibleCreatureAllergen<T>(CreatureTemplate.Type creatureType, int iconIntData = 0) : SimpleEdibleAllergen<T> where T : Creature
    {
        private readonly CreatureTemplate.Type _creatureType = creatureType;
        private readonly int _iconIntData = iconIntData;

        public override string Name =>
            (Custom.rainWorld.inGameTranslator.TryTranslate($"creaturetype-{_creatureType.value}",
                out string translated)
                ? translated
                : _creatureType.value) + " (eating)";

        public override FSprite GetIcon()
        {
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(new IconSymbol.IconSymbolData(_creatureType, null, _iconIntData)))
            {
                color = CreatureSymbol.ColorOfCreature(new IconSymbol.IconSymbolData(_creatureType, null, _iconIntData)),
            };
        }
    }
}
