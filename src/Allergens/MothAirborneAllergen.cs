using Watcher;

namespace Allergies.Allergens
{
    internal class MothAirborneAllergen : IAllergen
    {
        public string Name => "Moths";

        public bool Equals(IAllergen other)
        {
            return other is MothAirborneAllergen;
        }

        public FSprite GetIcon()
        {
            var data = new IconSymbol.IconSymbolData(WatcherEnums.CreatureTemplateType.BigMoth, null, 0);
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(data))
            {
                color = CreatureSymbol.ColorOfCreature(data)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Airborne && thing is BigMoth;
        }
    }
}
