using Allergies.Triggers;
using RWCustom;

namespace Allergies.Allergens
{
    internal class SpiderBiteAllergen : IAllergen
    {
        public string Name => "Spider Bite";

        public bool Equals(IAllergen other)
        {
            return other is SpiderBiteAllergen;
        }

        public FSprite GetIcon()
        {
            var data = new IconSymbol.IconSymbolData(CreatureTemplate.Type.BigSpider, null, 0);
            return new FSprite(CreatureSymbol.SpriteNameOfCreature(data))
            {
                color = CreatureSymbol.ColorOfCreature(data)
            };
        }

        public bool MatchesCriteria(PhysicalObject? thing, TriggerType trigger)
        {
            return trigger == TriggerType.Bite && thing is BigSpider or Spider;
        }
    }
}
