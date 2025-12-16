using RWCustom;

namespace Allergies.Allergens
{
    public class SimpleTouchCreatureAllergen<T>(CreatureTemplate.Type creatureType, int iconIntData = 0) : SimpleTouchAllergen<T> where T : Creature
    {
        private readonly CreatureTemplate.Type _creatureType = creatureType;
        private readonly int _iconIntData = iconIntData;

        public override string Name =>
            (Custom.rainWorld.inGameTranslator.TryTranslate($"creaturetype-{_creatureType.value}",
                out string translated)
                ? translated
                : _creatureType.value) + " (touch)";

        public override FSprite GetIcon()
        {
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(new IconSymbol.IconSymbolData(_creatureType, null, _iconIntData)))
            {
                color = CreatureSymbol.ColorOfCreature(new IconSymbol.IconSymbolData(_creatureType, null, _iconIntData)),
            };
        }
    }
}