using RWCustom;

namespace Allergies.Allergens
{
    internal class PolePlantAllergen : IAllergen
    {
        public string Name => Custom.rainWorld.inGameTranslator.Translate($"creaturetype-PoleMimic");

        public bool Equals(IAllergen other)
        {
            return other is PolePlantAllergen;
        }

        public FSprite GetIcon()
        {
            var data = new IconSymbol.IconSymbolData(CreatureTemplate.Type.PoleMimic, null, 0);
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(data))
            {
                color = CreatureSymbol.ColorOfCreature(data)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return (trigger == TriggerType.Touch || trigger == TriggerType.Bite) && thing is PoleMimic;
        }
    }
}
